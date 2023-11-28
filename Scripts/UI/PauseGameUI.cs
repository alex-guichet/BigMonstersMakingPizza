using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseGameUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;
    
    private void Start()
    { 
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
        });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(SceneName.MainMenuScene);
        });
        
        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            OptionsUI.Instance.Show(Show);
        });
        
        GameManager.Instance.OnLocalTogglePause += GameManagerOnLocalTogglePause;
        Hide();
    }
    
    private void GameManagerOnLocalTogglePause(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePaused())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    } 
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
