using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class KitchenGameMultiplier : NetworkBehaviour
{
    [SerializeField] private KitchenObjectListSO kitchenObjectListSo;
    [SerializeField] private List<Color> playerColorList;
        
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataListChanged;
    
    public const int MAX_NUMBER_PLAYER = 4;
    public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplier";
    
    public static KitchenGameMultiplier Instance;

    private NetworkList<PlayerData> playerDataList;

    private string playerName;

    public static bool isMultiplayer;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));
        playerDataList = new();
        playerDataList.OnListChanged += PlayerDataListOnListChanged;
    }

    private void Start()
    {
        if (!isMultiplayer)
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignedIn += AuthenticationServiceOnSignedIn;
            }
            else
            {
                AuthenticationServiceOnSignedIn();
            }
        }
    }

    private void AuthenticationServiceOnSignedIn()
    {
        StartHost();
        Loader.LoadNetworkScene(SceneName.GameScene);
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    private void PlayerDataListOnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataList.Add(new PlayerData()
        {
            clientId = clientId,
            colorId = GetFirstUnusedColor()
        });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }


    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != SceneName.CharacterSelectionScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "The game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_NUMBER_PLAYER)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "The game is full";
            return;
        }
        
        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        
        var playerData = playerDataList[playerDataIndex];
        
        playerData.playerName = (FixedString64Bytes)playerName;

        playerDataList[playerDataIndex] = playerData;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        
        var playerData = playerDataList[playerDataIndex];
        
        playerData.playerId = (FixedString64Bytes)playerId;

        playerDataList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].clientId == clientId)
            {
                playerDataList.RemoveAt(i);
            }
        }
    }
    
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSoIndex(kitchenObjectSo), kitchenObjectParent.GetNetworkObject());
    }
    
    
    [ServerRpc (RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSo = GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (kitchenObjectParent.HasKitchenObject())
            return;
        
        KitchenObject kitchenObject = Instantiate(kitchenObjectSo.prefab);
        
        var networkObjectNetworkObject = kitchenObject.GetComponent<NetworkObject>(); 
        networkObjectNetworkObject.Spawn(true);

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSoIndex(KitchenObjectSO kitchenObjectSo)
    {
        return kitchenObjectListSo.KitchenObjectSoList.IndexOf(kitchenObjectSo);
    }
    
    
    public KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSoIndex)
    {
        return kitchenObjectListSo.KitchenObjectSoList[kitchenObjectSoIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        NetworkObject kitchenObjectNetworkObject = kitchenObject.GetComponent<NetworkObject>();
        DestroyKitchenObjectServerRpc(kitchenObjectNetworkObject);
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

        if (kitchenObjectNetworkObject == null)
            return;
        
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        
        ClearKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }
    
    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        KitchenObject kitchenObject = kitchenObjectParentNetworkObject.GetComponent<KitchenObject>();
        
        kitchenObject.ClearKitchenObject();
    }

    public int GetPlayerConnectedCount()
    {
        return playerDataList.Count;
    }

    public PlayerData GetPlayerData(int playerIndex)
    {
        return playerDataList[playerIndex];
    }
    
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataList[playerIndex];
    }
    
    public ulong GetClientIdFromPlayerIndex(int playerIndex)
    {
        return playerDataList[playerIndex].clientId;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in playerDataList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }

        return default;
    }
    
    public PlayerData GetLocalPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i< playerDataList.Count; i++ )
        {
            if (playerDataList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public Color GetPlayerColor(int playerIndex)
    {
        return playerColorList[playerIndex];
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in playerDataList)
        {
            if (playerData.colorId == colorId)
                return false;
        }

        return true;
    }

    private int GetFirstUnusedColor()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void ChangePlayerColor(int colorIndex)
    {
        ChangePlayerColorServerRpc(colorIndex);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerColorServerRpc(int colorIndex, ServerRpcParams serverRpcParams = default )
    {
        if (!IsColorAvailable(colorIndex))
            return;

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        
        var playerData = playerDataList[playerDataIndex];
        
        playerData.colorId = colorIndex;

        playerDataList[playerDataIndex] = playerData;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
    
    public void SetLoaderTargetScene()
    {
        SetLoaderTargetSceneServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetLoaderTargetSceneServerRpc()
    {
        SetLoaderTargetSceneClientRpc(Loader.GetIndexOfSceneName(Loader.GetSceneTarget()));
    }
    
    [ClientRpc]
    private void SetLoaderTargetSceneClientRpc(int sceneIndex)
    {
        Loader.SetSceneTarget(Loader.GetSceneNameFromIndex(sceneIndex));
    }
}
