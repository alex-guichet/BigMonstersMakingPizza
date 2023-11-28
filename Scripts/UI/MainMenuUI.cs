using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button playSinglePlayerButton;
    [SerializeField] private Button quitButton;


    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        
        playMultiplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplier.isMultiplayer = true;
            Loader.LoadScene(SceneName.LobbyScene);
        });
        
        playSinglePlayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplier.isMultiplayer = false;
            Loader.LoadScene(SceneName.LobbyScene);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }
    
    
}
