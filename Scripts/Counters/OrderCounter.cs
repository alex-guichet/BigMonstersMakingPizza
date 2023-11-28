using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OrderCounter : BaseCounter
{

   public static OrderCounter Instance;
   public event EventHandler OnInteract;

   private void Awake()
   {
      Instance = this;
   }
   
   public override void Interact(Player player){
      OnInteract?.Invoke(this, EventArgs.Empty);
   }
}
