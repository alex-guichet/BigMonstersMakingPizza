using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawn;
    public event EventHandler OnPlateRemove;

    [SerializeField] private KitchenObjectSO kitchenObjectSo;
    [SerializeField] private float spawnPlateTime = 4f;
    [SerializeField] private float spawnPlateNumberMax = 4f;

    private float _spawnPlateTimer;
    private float _spawnPlateNumber;
    
    
    private void Update()
    {
        
        if (!IsServer)
            return;
        
        if (!GameManager.Instance.IsGamePlaying())
            return;
        
        if (_spawnPlateNumber >= spawnPlateNumberMax)
            return;

        _spawnPlateTimer -= Time.deltaTime;
        if (_spawnPlateTimer < 0f)
        {
            _spawnPlateTimer = spawnPlateTime;
            SpawnPlateServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        OnPlateSpawn?.Invoke(this, EventArgs.Empty);
        _spawnPlateNumber++;
    }
    
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject() && _spawnPlateNumber > 0)
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
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
        _spawnPlateNumber--;
        OnPlateRemove?.Invoke(this, EventArgs.Empty);
    }
}
