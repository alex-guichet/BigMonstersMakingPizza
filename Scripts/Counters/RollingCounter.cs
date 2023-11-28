using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class RollingCounter : BaseCounter, IHasProgress
{
    [SerializeField] private RollingRecipeSO[] rollingRecipeSoArray;
    
    public static event EventHandler OnAnyRoll ;
    public event EventHandler OnRoll ;
    
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    
    private int _rollingProgress;

    public static void ResetStaticData()
    {
        OnAnyRoll = null;
    }
    
    public override void Interact(Player player){
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRollingRecipeSo(player.GetKitchenObject().GetKitchenObjectSO()))
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
        
        if (HasKitchenObject() && HasRollingRecipeSo(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateServerRpc();
            RollingProgressServerRpc();
        }
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void InteractAlternateServerRpc()
    {
        if (HasKitchenObject() && HasRollingRecipeSo(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateClientRpc();
        }
    }
    
    [ClientRpc]
    private void InteractAlternateClientRpc()
    {
        OnRoll?.Invoke(this, EventArgs.Empty);
        OnAnyRoll?.Invoke(this, EventArgs.Empty);
        
        _rollingProgress++;
            
        var rollingRecipeSo = GetRollingRecipeSo(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)_rollingProgress/rollingRecipeSo.rollingProgressMax
        });
    }

    [ServerRpc (RequireOwnership = false)]
    private void RollingProgressServerRpc()
    {
        if (!HasKitchenObject() && !HasRollingRecipeSo(GetKitchenObject().GetKitchenObjectSO()))
            return;
        
        var cuttingRecipeSo = GetRollingRecipeSo(GetKitchenObject().GetKitchenObjectSO());
        
        if (_rollingProgress >= cuttingRecipeSo.rollingProgressMax)
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
        _rollingProgress = 0;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }
    
    private bool HasRollingRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        var rollingRecipeSo = GetRollingRecipeSo(inputKitchenObject);

        return rollingRecipeSo != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObject)
    {
        var rollingRecipeSo = GetRollingRecipeSo(inputKitchenObject);

        return rollingRecipeSo != null ? rollingRecipeSo.output : null;
    }

    private RollingRecipeSO GetRollingRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        foreach (var rollingRecipeSo in rollingRecipeSoArray)
        {
            if (inputKitchenObject == rollingRecipeSo.input)
            {
                return rollingRecipeSo;
            }
        }
        return null;
    }

}
