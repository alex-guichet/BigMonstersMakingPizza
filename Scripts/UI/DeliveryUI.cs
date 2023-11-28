using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI deliveryText;
   [SerializeField] private Image backGroundImage;
   [SerializeField] private Image deliveryImage;
   
   [SerializeField] private Color successColor;
   [SerializeField] private Color failedColor;
   
   [SerializeField] private Sprite successSprite;
   [SerializeField] private Sprite failedSprite;

   private Animator _deliveryUIAnimator;
   private static readonly int TriggerDelivery = Animator.StringToHash("TriggerDelivery");

   private void Awake()
   {
      _deliveryUIAnimator = GetComponent<Animator>();
   }
   
   private void Start()
   {
      DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
      DeliveryManager.Instance.OnRecipeCompleted += DeliveryManagerOnRecipeCompleted;
      
      gameObject.SetActive(false);
   }

   private void DeliveryManagerOnRecipeFailed(object sender, EventArgs e)
   {
      gameObject.SetActive(true);
      backGroundImage.color = failedColor;
      deliveryImage.sprite = failedSprite;
      deliveryText.text = "Delivery \n Failed";
      _deliveryUIAnimator.SetTrigger(TriggerDelivery);
   }
   
   private void DeliveryManagerOnRecipeCompleted(object sender, EventArgs e)
   {
      gameObject.SetActive(true);
      backGroundImage.color = successColor;
      deliveryImage.sprite = successSprite;
      deliveryText.text = "Delivery \n Success";
      _deliveryUIAnimator.SetTrigger(TriggerDelivery);
   }
}
