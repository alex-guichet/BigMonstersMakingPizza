using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    private IKitchenObjectParent _kitchenObjectParent;
    private FollowTransform _followTransform;
    
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSo;
    }
    
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    
    [ClientRpc]
    public void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        if (_kitchenObjectParent != null)
        {
            _kitchenObjectParent.ClearKitchenObject();
        }

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject networkObject);
        IKitchenObjectParent kitchenObjectParent = networkObject.GetComponent<IKitchenObjectParent>();
        
        _kitchenObjectParent = kitchenObjectParent;
        
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError(kitchenObjectParent.GetKitchenObject() + " Already here");
        }
        
        kitchenObjectParent.SetKitchenObject(this);
        _followTransform.SetTargetTransform(kitchenObjectParent.GetObjectSpawnPoint());
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    public void ClearKitchenObject()
    {
        _kitchenObjectParent.ClearKitchenObject();
    }
    
    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return _kitchenObjectParent;
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiplier.Instance.SpawnKitchenObject(kitchenObjectSo, kitchenObjectParent);
    }
    
    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMultiplier.Instance.DestroyKitchenObject(kitchenObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }

        plateKitchenObject = null;
        return false;
    }

    private void Awake()
    {
        _followTransform = GetComponent<FollowTransform>();
    }

}
