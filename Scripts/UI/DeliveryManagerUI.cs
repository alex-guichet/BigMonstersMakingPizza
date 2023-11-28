using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
   [SerializeField] private Transform recipeContainer;
   [SerializeField] private DeliveryManagerRecipeUI recipeTemplate;

   private void Awake()
   {
      recipeTemplate.gameObject.SetActive(false);
   }
   
   private void Start()
   {
      OrderCounter.Instance.OnInteract += OrderCounterOnInteract;
      DeliveryManager.Instance.OnRecipeAdded += DeliveryManagerOnRecipeAdded;
      DeliveryManager.Instance.OnRecipeCompleted += DeliveryManagerOnRecipeCompleted;
      transform.gameObject.SetActive(false);
   }

   private void OrderCounterOnInteract(object sender, EventArgs e)
   {
      transform.gameObject.SetActive(!transform.gameObject.activeInHierarchy);
   }

   private void UpdateVisual()
   {
      foreach (Transform child in recipeContainer)
      {
         if (child == recipeTemplate.transform)
            continue;
         Destroy(child.gameObject);
      }

      foreach (var recipeSO in DeliveryManager.Instance.GetWaitingList())
      {
         var recipe = Instantiate(recipeTemplate, recipeContainer);
         recipe.gameObject.SetActive(true);
         recipe.SetVisual(recipeSO);
      }
   }
   
   private void DeliveryManagerOnRecipeAdded(object sender, EventArgs e)
   {
      UpdateVisual();
   }
   
   private void DeliveryManagerOnRecipeCompleted(object sender, EventArgs e)
   {
      UpdateVisual();
   }
}
