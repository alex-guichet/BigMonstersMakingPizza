using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createPrivateLobby;
    [SerializeField] private Button createPublicLobby;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    
    void Start()
    {
        createPrivateLobby.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        
        createPublicLobby.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });

        createPrivateLobby.interactable = false;
        createPublicLobby.interactable = false;

        lobbyNameInputField.onValueChanged.AddListener((string inputFieldText) =>
        {
            if (inputFieldText != String.Empty)
            {
                createPrivateLobby.interactable = true;
                createPublicLobby.interactable = true;
            }
            else
            {
                createPrivateLobby.interactable = false;
                createPublicLobby.interactable = false;
            }
        });
            
        closeButton.onClick.AddListener(Hide);
        
        Hide();
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        createPublicLobby.Select();
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
