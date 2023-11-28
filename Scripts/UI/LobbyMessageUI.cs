using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI responseMessageText;
    
    
    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        KitchenGameMultiplier.Instance.OnFailedToJoinGame += KitchenGameMultiplierOnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobbyOnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobbyOnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobbyOnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobbyOnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobbyOnJoinFailed;
        Hide();
    }

    private void KitchenGameLobbyOnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join Lobby...");
    }

    private void KitchenGameLobbyOnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Could not find Lobby to quick join !");
    }

    private void KitchenGameLobbyOnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining Lobby..");
    }

    private void KitchenGameLobbyOnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Fail to create Lobby !");
    }

    private void KitchenGameLobbyOnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void KitchenGameMultiplierOnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }
    
    private void ShowMessage(string message)
    {
        Show();
        closeButton.Select();
        responseMessageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    
    {
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        KitchenGameMultiplier.Instance.OnFailedToJoinGame -= KitchenGameMultiplierOnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobbyOnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobbyOnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobbyOnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobbyOnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobbyOnJoinFailed;
    }
}
