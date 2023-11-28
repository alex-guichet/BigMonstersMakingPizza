using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter instance { get; private set; }
    
    private void Awake()
    {
        instance = this;
    }
    
    public override void Interact(Player player){
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverPlate(plateKitchenObject);
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }
}
