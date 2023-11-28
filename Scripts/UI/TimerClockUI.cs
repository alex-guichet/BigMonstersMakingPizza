using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TimerClockUI : MonoBehaviour
{

    [SerializeField] private Image timerClockUI;
    [SerializeField] private Button pauseButton;

    private void Start()
    {
        pauseButton.onClick.AddListener( () =>
        {
            GameManager.Instance.TogglePause();
        });
        
        GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
        Hide();
    }

    private void GameManagerOnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            Show();
        }else
        {
            Hide();
        }
    }

    private void Update()
    {
        timerClockUI.fillAmount = 1f - GameManager.Instance.GetPlayingTimerNormalized();
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
