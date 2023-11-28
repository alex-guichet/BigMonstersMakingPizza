using System;
using UnityEngine;

public class MultiPlayerPauseGameUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnMultiPlayerTogglePause += GameManagerOnMultiPlayerTogglePause;
        Hide();
    }
    
    private void GameManagerOnMultiPlayerTogglePause(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsMultiPlayerGamePaused())
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
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
