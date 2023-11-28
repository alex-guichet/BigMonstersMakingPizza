using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingCounterVisual : MonoBehaviour
{
    [SerializeField] private RollingCounter rollingCounter;
    
    private Animator _rollingCounterAnimator;
    private static readonly int Roll = Animator.StringToHash("Roll");

    private void Awake()
    {
        _rollingCounterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        rollingCounter.OnRoll += OnRoll;
    }

    private void OnRoll(object sender, EventArgs e)
    {
        _rollingCounterAnimator.SetTrigger(Roll);
    }
}
