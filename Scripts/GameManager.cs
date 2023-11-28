using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum GameState
{
    Waiting,
    CountDown,
    Playing,
    GameOver
}

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    public static GameManager Instance;

    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalTogglePause;
    public event EventHandler OnMultiPlayerTogglePause;

    public Dictionary<ulong, bool> playerReadyDictionary = new();
    public Dictionary<ulong, bool> playerPausedDictionary = new();

    private NetworkVariable<GameState> _gameState = new();

    private NetworkVariable<float> _countDownTimer = new(3f);
    private NetworkVariable<float> _playingTimer = new();
    
    private NetworkVariable<bool> _isGamePaused = new();
    
    private float _playingTimerMax = 90f;
    
    private bool _isLocalGamePaused;
    private bool _isLocalPlayerReady;
    private bool _testGamePaused;

    private void Awake()
    {
        Instance = this;
        _gameState.Value = GameState.Waiting;
    }

    public override void OnNetworkSpawn()
    {
        _gameState.OnValueChanged += GameStateOnValueChanged;
        _isGamePaused.OnValueChanged += IsGamePausedOnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += GameManagerOnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnLoadEventCompleted;
        }
    }

    private void SceneManagerOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void GameManagerOnClientDisconnectCallback(ulong clientId)
    {
        _testGamePaused = true;
    }

    private void IsGamePausedOnValueChanged(bool previousValue, bool newValue)
    {
        if (_isGamePaused.Value)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        
        OnMultiPlayerTogglePause?.Invoke(this, EventArgs.Empty);
    }

    private void GameStateOnValueChanged(GameState previousState, GameState newState)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        GameInput.Instance.OnPause += GameInputOnPause;
        GameInput.Instance.OnInteractAction += GameManagerOnInteractAction;
    }

    private void GameManagerOnInteractAction(object sender, EventArgs e)
    {
        if (_gameState.Value == GameState.Waiting)
        {
            _isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            _gameState.Value = GameState.CountDown;
        }
    }

    private void GameInputOnPause(object sender, EventArgs e)
    {
        TogglePause();
    }

    private void Update()
    {
        if(!IsServer)
            return;
        
        switch (_gameState.Value)
        {
            case GameState.Waiting:
                break;
            case GameState.CountDown:
                _countDownTimer.Value -= Time.deltaTime;
                if (_countDownTimer.Value < 0f)
                {
                    _playingTimer.Value = _playingTimerMax;
                    _gameState.Value = GameState.Playing;
                }
                break;
            case GameState.Playing:
                _playingTimer.Value -= Time.deltaTime;
                if (_playingTimer.Value < 0f)
                {
                    _gameState.Value = GameState.GameOver;
                }
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void LateUpdate()
    {
        if (_testGamePaused)
        {
            _testGamePaused = false;
            TestGamePaused();
        }
    }
    public bool IsWaiting()
    {
        return _gameState.Value == GameState.Waiting;
    }
    
    public bool IsGamePlaying()
    {
        return _gameState.Value == GameState.Playing;
    }
    
    public bool IsGameCountdown()
    {
        return _gameState.Value == GameState.CountDown;
    }
    
    public bool IsGameOver()
    {
        return _gameState.Value == GameState.GameOver;
    }

    public float GetCountDownTimer()
    {
        return _countDownTimer.Value;
    }

    public float GetPlayingTimerNormalized()
    {
        return _playingTimer.Value / _playingTimerMax;
    }

    public bool IsGamePaused()
    {
        return _isLocalGamePaused;
        
    }public bool IsMultiPlayerGamePaused()
    {
        return _isGamePaused.Value;
    }

    public void TogglePause()
    {
        _isLocalGamePaused = !_isLocalGamePaused;
        
        if (_isLocalGamePaused)
        {
            PauseGameServerRpc();
        }
        else
        {
            UnPauseGameServerRpc();
        }
        OnLocalTogglePause?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc (RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePaused();
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePaused();
    }

    private void TestGamePaused()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                _isGamePaused.Value = true;
                return;
            }
        }
        
        _isGamePaused.Value = false;
    }
    
    
    public bool IsLocalPlayerReady()
    {
        return _isLocalPlayerReady;
    }
    
    

}
