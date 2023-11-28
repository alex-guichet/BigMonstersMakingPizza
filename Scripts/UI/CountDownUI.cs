using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownTimerText;

    private int _previousCountDownTimer;

    private Animator _countDownAnimator;
    private static readonly int CountDownTrigger = Animator.StringToHash("CountDownTrigger");


    private void Awake()
    {
        _countDownAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
        Hide();
    }

    private void GameManagerOnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameCountdown())
        {
            Show();
        }else
        {
            Hide();
        }
    }

    private void Update()
    {
        int countDownTimer = Mathf.CeilToInt(GameManager.Instance.GetCountDownTimer());

        if (countDownTimer != _previousCountDownTimer)
        {
            _previousCountDownTimer = countDownTimer;
            countDownTimerText.text = countDownTimer.ToString();
            _countDownAnimator.SetTrigger(CountDownTrigger);
            SoundManager.Instance.PlayBellSound();
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
