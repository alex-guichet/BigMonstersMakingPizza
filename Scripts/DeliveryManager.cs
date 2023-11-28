using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeAdded;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;


    public static DeliveryManager Instance;

    [SerializeField] private RecipeListSO recipeListSo;
    [SerializeField] private int maxDeliveryNumber = 4;
    [SerializeField] private float deliveryTime = 5f;

    private float _deliveryTimer = 2f;
    private List<RecipeSO> _waitingDeliveryListSo = new();

    private int _deliverySuccessfulNumber;
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (!GameManager.Instance.IsGamePlaying())
            return;
        
        if (_waitingDeliveryListSo.Count >= maxDeliveryNumber)
            return;

        _deliveryTimer -= Time.deltaTime;
        if (_deliveryTimer < 0f)
        {
            int randomIndex = Random.Range(0, recipeListSo.recipeSoList.Count);
            SpawnRecipeClientRpc(randomIndex);
            _deliveryTimer = deliveryTime;
        }
    }

    [ClientRpc]
    private void SpawnRecipeClientRpc(int recipeIndex)
    {
        var recipeSo = recipeListSo.recipeSoList[recipeIndex];
        _waitingDeliveryListSo.Add(recipeSo);
        OnRecipeAdded?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverPlate(PlateKitchenObject plateKitchenObject)
    {
        for(int i = 0; i < _waitingDeliveryListSo.Count; i++)
        {
            var waitingRecipe = _waitingDeliveryListSo[i];
            
            if (waitingRecipe.kitchenObjectSoList.Count != plateKitchenObject.GetKitchenObjectSoList().Count)
                continue;

            bool plateContentMatchesRecipe = true;
            foreach (var kitchenObjectSoWaitingRecipe in waitingRecipe.kitchenObjectSoList)
            {
                bool ingredientFound = false;
                
                foreach (var kitchenObjectSoPlate in plateKitchenObject.GetKitchenObjectSoList())
                {
                    if (kitchenObjectSoWaitingRecipe == kitchenObjectSoPlate)
                    {
                        ingredientFound = true;
                        break;
                    }
                }
                
                if (!ingredientFound)
                {
                    plateContentMatchesRecipe = false;
                }
            }

            if (plateContentMatchesRecipe)
            {
                DeliverCorrectRecipeServerRpc(i);
                return;
            }
        }
        
        DeliverInCorrectRecipeServerRpc();
    }

    [ServerRpc (RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingDeliveryListSoIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingDeliveryListSoIndex);
    }
    
    
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingDeliveryListSoIndex)
    {
        _waitingDeliveryListSo.RemoveAt(waitingDeliveryListSoIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        _deliverySuccessfulNumber++;
    }
    
    
    [ServerRpc (RequireOwnership = false)]
    private void DeliverInCorrectRecipeServerRpc()
    {
        DeliverInCorrectRecipeClientRpc();
    }
    
    
    [ClientRpc]
    private void DeliverInCorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingList()
    {
        return _waitingDeliveryListSo;
    }

    public int GetDeliverySuccessfulNumber()
    {
        return _deliverySuccessfulNumber;
    }
}
