using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnTrash;
    
    public static void ResetStaticData()
    {
        OnTrash = null;
    }
    
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
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
        OnTrash?.Invoke(this, EventArgs.Empty);
    }
}
