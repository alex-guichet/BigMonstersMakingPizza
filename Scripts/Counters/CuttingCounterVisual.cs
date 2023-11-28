using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    
    private Animator _cuttingCounterAnimator;
    private static readonly int Cut = Animator.StringToHash("Cut");

    private void Awake()
    {
        _cuttingCounterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCut += OnCut;
    }

    private void OnCut(object sender, EventArgs e)
    {
        _cuttingCounterAnimator.SetTrigger(Cut);
    }
}
