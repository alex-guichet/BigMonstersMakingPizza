using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] protected KitchenObjectSO kitchenObjecSO;
    
    public event EventHandler OnContainerInteraction;
    
    public override void Interact(Player player){

        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjecSO, player);
            InteractServerRpc();
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
