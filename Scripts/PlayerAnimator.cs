using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
   private Animator _animator;
   private Player _player;
   
   private static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
   private const string IS_WALKING = "IsWalking";

   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _player = GetComponentInParent<Player>();
   }

   private void Start()
   {
      _player.OnPickUp += PlayerOnPickUp;
      _player.OnDrop += PlayerOnDrop;
   }

   private void PlayerOnPickUp(object sender, EventArgs e)
   {
      _animator.SetBool(IsCarrying, true);
   }
   
   private void PlayerOnDrop(object sender, EventArgs e)
   {
      _animator.SetBool(IsCarrying, false);
   }

   private void Update()
   {
      if (!IsOwner)
         return;
      
      _animator.SetBool(IS_WALKING, _player.isWalking);
   }
   
}
