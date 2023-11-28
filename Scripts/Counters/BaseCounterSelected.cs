using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseCounterSelected : MonoBehaviour
{
    
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject selectedCounterVisual;

    private void Start()
    {
        if (Player.localInstance != null)
        {
            Player.localInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += PlayerOnPlayerSpawned; 
        }
    }

    private void PlayerOnPlayerSpawned(object sender, EventArgs e)
    {
        if (Player.localInstance != null)
        {
            Player.localInstance.OnSelectedCounterChanged -= OnSelectedCounterChanged;
            Player.localInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
    }

    private void OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgument e) {
        
        if (e.selectedCounter == baseCounter) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show()
    {
        if (selectedCounterVisual == null){
            
            Debug.Log(gameObject.name);
        }
        else
        {
            selectedCounterVisual.SetActive(true);
        }
    }
    
    private void Hide()
    {
        if (selectedCounterVisual == null){
            
            Debug.Log(gameObject.name);
        }
        else
        {
            selectedCounterVisual.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawned -= PlayerOnPlayerSpawned;
    }
}
