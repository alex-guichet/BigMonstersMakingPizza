using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    private Dictionary<ulong, bool> playerReadyDictionary = new();

    public static CharacterSelectReady Instance;

    public event EventHandler OnPlayerReadyUpdate;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
        
    [ServerRpc (RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

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
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetworkScene(SceneName.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnPlayerReadyUpdate?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        if (playerReadyDictionary.ContainsKey(clientId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
