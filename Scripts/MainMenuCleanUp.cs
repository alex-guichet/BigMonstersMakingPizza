using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        
        if (KitchenGameMultiplier.Instance != null)
        {
            Destroy(KitchenGameMultiplier.Instance.gameObject);
        }
        
        if (KitchenGameLobby.Instance != null)
        {
            Destroy(KitchenGameLobby.Instance.gameObject);
        }
    }
}
