using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class HostDisconnectedUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    
    private void Start()
    {
        mainMenuButton.onClick.AddListener((() =>
        {
            Loader.LoadScene(SceneName.MainMenuScene);
        }));
        
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
        Hide();
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManagerOnClientDisconnectCallback;
        }
    }
}