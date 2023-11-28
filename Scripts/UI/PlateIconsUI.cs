using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
   [SerializeField] private PlateKitchenObject plateKitchenObject;
   [SerializeField] private Transform iconTemplateTransform;

   private void Awake()
   {
      iconTemplateTransform.gameObject.SetActive(false);
   }
   
   private void Start()
   {
      plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnIngredientAdded;
      plateKitchenObject.OnEnterOven += PlateKitchenObjectOnEnterOven;
      plateKitchenObject.OnExitOven += PlateKitchenObjectOnExitOven;
   }

   private void PlateKitchenObjectOnEnterOven(object sender, EventArgs e)
   {
      transform.gameObject.SetActive(false);
   }
   
   private void PlateKitchenObjectOnExitOven(object sender, EventArgs e)
   {
      transform.gameObject.SetActive(true);
   }

   private void PlateKitchenObjectOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
   {
      foreach (Transform child in transform)
      {
         if (child == iconTemplateTransform)
            continue;
         Destroy(child.gameObject);
      }
      
      foreach (var kitchenObjectSo in plateKitchenObject.GetKitchenObjectSoList())
      {
         var iconTemplate = Instantiate(iconTemplateTransform, transform);
         iconTemplate.GetComponent<IconTemplateUI>().SetIconImage(kitchenObjectSo.sprite);
         iconTemplate.gameObject.SetActive(true);
      }
   }
}
