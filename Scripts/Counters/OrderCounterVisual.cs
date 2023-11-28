using System;
using UnityEngine;

public class OrderCounterVisual : MonoBehaviour
{
   private Animator animatorOrder;
   
   private static readonly int Add = Animator.StringToHash("Add");
   private static readonly int OrderSuccess = Animator.StringToHash("OrderSuccess");
   private static readonly int OrderFailed = Animator.StringToHash("OrderFailed");

   private void Start()
   {
      animatorOrder = GetComponent<Animator>();
      DeliveryManager.Instance.OnRecipeAdded += DeliveryManagerOnRecipeAdded;
      DeliveryManager.Instance.OnRecipeCompleted += DeliveryOnRecipeCompleted;
      DeliveryManager.Instance.OnRecipeFailed += DeliveryOnRecipeFailed;
   }

   private void DeliveryOnRecipeFailed(object sender, EventArgs e)
   {
      animatorOrder.SetTrigger(OrderFailed);
   }

   private void DeliveryOnRecipeCompleted(object sender, EventArgs e)
   {
      animatorOrder.SetTrigger(OrderSuccess);
   }

   private void DeliveryManagerOnRecipeAdded(object sender, EventArgs e)
   {
      animatorOrder.SetTrigger(Add);
   }
}
