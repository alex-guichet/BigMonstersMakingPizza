using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientRemoved;
    public event EventHandler OnEnterOven;
    public event EventHandler OnExitOven;

    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO KitchenObjectSo;

        public OnIngredientAddedEventArgs(KitchenObjectSO kitchenObjectSo)
        {
            KitchenObjectSo = kitchenObjectSo;
        }
    }
    
    [SerializeField] private List<KitchenObjectSO> kitchenObjectListAllowedOnPlate;
    private List<KitchenObjectSO> _kitchenObjectSoList = new();

    public void TurnOffUIIcons()
    {
        OnEnterOven?.Invoke(this, EventArgs.Empty);
    }
    
    public void TurnOnUIIcons()
    {
        OnExitOven?.Invoke(this, EventArgs.Empty);
    }
    
    public bool TryAddIngredients(KitchenObjectSO kitchenObjectSo)
    {
        if (_kitchenObjectSoList.Contains(kitchenObjectSo))
            return false;

        if (!kitchenObjectListAllowedOnPlate.Contains(kitchenObjectSo)) 
            return false;

        AddIngredientServerRpc(KitchenGameMultiplier.Instance.GetKitchenObjectSoIndex(kitchenObjectSo));
        
        return true;
    }
    
    public void AddIngredientsWithoutAllowedOnPlateCheck(KitchenObjectSO kitchenObjectSo)
    {
        if (_kitchenObjectSoList.Contains(kitchenObjectSo))
            return;
        
        AddIngredientServerRpc(KitchenGameMultiplier.Instance.GetKitchenObjectSoIndex(kitchenObjectSo));
    }
    
    public bool TryRemoveIngredients(KitchenObjectSO kitchenObjectSo)
    {
        if (!_kitchenObjectSoList.Contains(kitchenObjectSo))
            return false;
        
        RemoveIngredientServerRpc(KitchenGameMultiplier.Instance.GetKitchenObjectSoIndex(kitchenObjectSo));
        
        return true;
    }

    [ServerRpc (RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSoIndex)
    {
        AddIngredientClientRpc(kitchenObjectSoIndex);
    }
    
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSoIndex)
    {
        KitchenObjectSO kitchenObjectSo = KitchenGameMultiplier.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
        _kitchenObjectSoList.Add(kitchenObjectSo);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs(kitchenObjectSo));
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void RemoveIngredientServerRpc(int kitchenObjectSoIndex)
    {
        RemoveIngredientClientRpc(kitchenObjectSoIndex);
    }
    
    [ClientRpc]
    private void RemoveIngredientClientRpc(int kitchenObjectSoIndex)
    {
        KitchenObjectSO kitchenObjectSo = KitchenGameMultiplier.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
        _kitchenObjectSoList.Remove(kitchenObjectSo);
        OnIngredientRemoved?.Invoke(this, new OnIngredientAddedEventArgs(kitchenObjectSo));
    }

    public List<KitchenObjectSO> GetKitchenObjectSoList()
    {
        return _kitchenObjectSoList;
    }

    public bool HasIngredientOnPlate(KitchenObjectSO kitchenObjectSoToFind)
    {
        foreach (KitchenObjectSO kitchenObjectSo in _kitchenObjectSoList)
        {
            if (kitchenObjectSo == kitchenObjectSoToFind)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool HasAllowedKitchenObjectOnPlate()
    {
        foreach (var kitchenObjectOnPlate in _kitchenObjectSoList)
        {
            bool ingredientFound = false;
            foreach(var kitchenObjectAllowedOnPlate in kitchenObjectListAllowedOnPlate)
            {
                if (kitchenObjectOnPlate == kitchenObjectAllowedOnPlate)
                {
                    ingredientFound = true;
                    break;
                }
            }

            if (!ingredientFound)
            {
                return false;
            }
        }
        return true;
    }
    
}
