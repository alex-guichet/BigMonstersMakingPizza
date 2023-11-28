using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    [SerializeField] private ContainerCounter containerCounter;
    
    private Animator _containerCounterAnimator;
    private static readonly int OpenClose = Animator.StringToHash("OpenClose");

    private void Awake()
    {
        _containerCounterAnimator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        containerCounter.OnContainerInteraction += OnContainerInteraction;
    }

    private void OnContainerInteraction(object sender, EventArgs e)
    {
        _containerCounterAnimator.SetTrigger(OpenClose);
    }
}
