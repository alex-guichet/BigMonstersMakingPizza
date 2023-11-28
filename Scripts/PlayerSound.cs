using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private float footStepTimeMax = 0.1f;

    public static event EventHandler OnWalk;
    
     private float _footStepTimer;
     private Player _player;

     public static void ResetStaticData()
     {
         OnWalk = null;
     }
     
     private void Awake()
     {
         _player = GetComponent<Player>();
     }
     
     
    private void Update()
    {
        if (!_player.isWalking)
            return;
        
        _footStepTimer -= Time.deltaTime;
        if (_footStepTimer < 0f)
        {
            _footStepTimer = footStepTimeMax;
            OnWalk.Invoke(this,EventArgs.Empty);
        }
    }
}
