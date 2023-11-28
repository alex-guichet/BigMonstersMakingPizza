using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSoArray;
    
    public static event EventHandler OnAnyCut ;
    public event EventHandler OnCut ;
    
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    
    private int _cuttingProgress;

    public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    
    public override void Interact(Player player){
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasCuttingRecipeSo(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    
                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
            else
            {
                //Player Doesn't have a kitchen object
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                InteractLogicPlaceObjectOnCounterServerRpc();
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            }
        }
    }
    
    public override void InteractAlternate(Player player){
        
        if (HasKitchenObject() && HasCuttingRecipeSo(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateServerRpc();
            CuttingProgressServerRpc();
        }
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void InteractAlternateServerRpc()
    {
        if (HasKitchenObject() && HasCuttingRecipeSo(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateClientRpc();
        }
    }
    
    [ClientRpc]
    private void InteractAlternateClientRpc()
    {
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        
        _cuttingProgress++;
            
        var cuttingRecipeSo = GetCuttingRecipeSo(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)_cuttingProgress/cuttingRecipeSo.cuttingProgressMax
        });
    }

    [ServerRpc (RequireOwnership = false)]
    private void CuttingProgressServerRpc()
    {
        if (!HasKitchenObject() && !HasCuttingRecipeSo(GetKitchenObject().GetKitchenObjectSO()))
            return;
        
        var cuttingRecipeSo = GetCuttingRecipeSo(GetKitchenObject().GetKitchenObjectSO());
        
        if (_cuttingProgress >= cuttingRecipeSo.cuttingProgressMax)
        {
            var cutKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(cutKitchenObjectSo, this);
        }
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }
    
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        _cuttingProgress = 0;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }
    
    private bool HasCuttingRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        var cuttingRecipeSo = GetCuttingRecipeSo(inputKitchenObject);

        return cuttingRecipeSo != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObject)
    {
        var cuttingRecipeSo = GetCuttingRecipeSo(inputKitchenObject);

        return cuttingRecipeSo != null ? cuttingRecipeSo.output : null;
    }

    private CuttingRecipeSO GetCuttingRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        foreach (var cuttingRecipeSo in cuttingRecipeSoArray)
        {
            if (inputKitchenObject == cuttingRecipeSo.input)
            {
                return cuttingRecipeSo;
            }
        }
        return null;
    }

}
