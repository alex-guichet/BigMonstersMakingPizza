using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class KitchenGameLobby : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    
    private Lobby joinedLobby;

    private float _heartBeatTimer;
    private float _listLobbiesTimer;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentification();
    }

    private async void InitializeUnityAuthentification()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 100000000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            _heartBeatTimer -= Time.deltaTime;
            if (_heartBeatTimer <= 0f)
            {
                float heartBetTimerMax = 15f;
                _heartBeatTimer = heartBetTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplier.MAX_NUMBER_PLAYER - 1);
            return allocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,
                KitchenGameMultiplier.MAX_NUMBER_PLAYER,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            KitchenGameMultiplier.Instance.StartHost();
            Loader.LoadNetworkScene(SceneName.CharacterSelectionScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            KitchenGameMultiplier.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            KitchenGameMultiplier.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            
            KitchenGameMultiplier.Instance.StartClient();
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    
    public async void KickPlayer(string playerId)
    {
        if (joinedLobby != null)
        {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    private void HandlePeriodicListLobbies()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == SceneName.LobbyScene.ToString()) {
            
            _listLobbiesTimer -= Time.deltaTime;
            if (_listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                _listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }
    
}
