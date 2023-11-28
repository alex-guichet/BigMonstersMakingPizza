using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneUI : MonoBehaviour
{
    
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button codeJoinButton;
    [SerializeField] private CreateLobbyUI createLobbyUI;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    [SerializeField] private GameObject backGround;
    
    
    private void Start()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.LoadScene(SceneName.MainMenuScene);
        });
        
        createLobbyButton.onClick.AddListener(() =>
        {
            createLobbyUI.Show();
        });
        
        quickJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });
        
        codeJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(lobbyCodeInputField.text);
        });

        playerNameInputField.text = KitchenGameMultiplier.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            KitchenGameMultiplier.Instance.SetPlayerName(newText);
        });
        
        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobbyOnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
        lobbyTemplate.gameObject.SetActive(false);

        if (KitchenGameMultiplier.isMultiplayer)
        {
            backGround.SetActive(false);
        }
        
        playerNameInputField.onValueChanged.AddListener((string inputFieldText) =>
        {
            if (inputFieldText != String.Empty)
            {
                createLobbyButton.interactable = true;
                quickJoinButton.interactable = true;
                codeJoinButton.interactable = true;
            }
            else
            {
                createLobbyButton.interactable = false;
                quickJoinButton.interactable = false;
                codeJoinButton.interactable = false;
            }
        });
        
        codeJoinButton.interactable = false;
        lobbyCodeInputField.onValueChanged.AddListener((string inputFieldText) =>
        {
            if (inputFieldText != String.Empty)
            {
                codeJoinButton.interactable = true;
            }
            else
            {
                codeJoinButton.interactable = false;
            }
        });
    }

    private void KitchenGameLobbyOnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate)
                continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate,lobbyContainer );
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobbyOnLobbyListChanged;
    }
}
