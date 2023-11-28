using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    [SerializeField] protected Transform kitchenObjectSpawnPoint;
    
    protected KitchenObject KitchenObject;
    
    public virtual void Interact(Player player){}
    
    public virtual void InteractAlternate(Player player){}
    
    public Transform GetObjectSpawnPoint()
    {
        return kitchenObjectSpawnPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        KitchenObject = kitchenObject;
    }
   
    public KitchenObject GetKitchenObject()
    {
        return KitchenObject;
    }
   
    public void ClearKitchenObject()
    {
        KitchenObject = null;
    }
   
    public bool HasKitchenObject()
    {
        return KitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }
}
