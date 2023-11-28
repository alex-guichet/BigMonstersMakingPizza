using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
   private void Start()
   {
      KitchenGameMultiplier.Instance.OnTryingToJoinGame += KitchenGameMultiplierOnTryingToJoinGame;
      KitchenGameMultiplier.Instance.OnFailedToJoinGame += KitchenGameMultiplierOnFailedToJoinGame;
      Hide();
   }

   private void KitchenGameMultiplierOnTryingToJoinGame(object sender, EventArgs e)
   {
      Show();
   }
   
   private void KitchenGameMultiplierOnFailedToJoinGame(object sender, EventArgs e)
   {
      Hide();
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }
   
   private void Show()
   {
      gameObject.SetActive(true);
   }
   
   
   public void OnDestroy()
   {
      KitchenGameMultiplier.Instance.OnTryingToJoinGame -= KitchenGameMultiplierOnTryingToJoinGame;
      KitchenGameMultiplier.Instance.OnFailedToJoinGame -= KitchenGameMultiplierOnFailedToJoinGame;
   }
}
