using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SauceCounter : BaseCounter
{
    [SerializeField] protected KitchenObjectSO kitchenObjecSO;
    [SerializeField] protected KitchenObjectSO[] kitchenObjecSORequiredOnPlate;
    
    public event EventHandler OnContainerInteraction;
    
    public override void Interact(Player player){

        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                bool isIngredientMissing = false;
                foreach (var kitchenObjectSoRequired in kitchenObjecSORequiredOnPlate)
                {
                    if (!plateKitchenObject.HasIngredientOnPlate(kitchenObjectSoRequired))
                    {
                        isIngredientMissing = true;
                        break;
                    }
                }

                if (!isIngredientMissing)
                {
                    plateKitchenObject.TryAddIngredients(kitchenObjecSO);
                }
                
            }
            //InteractServerRpc();
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }
    
    [ClientRpc]
    private void InteractClientRpc()
    {
        OnContainerInteraction?.Invoke(this, EventArgs.Empty);
    }
}
