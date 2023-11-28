using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform plateTransformPrefab;

    private List<Transform> _spawnPlateList = new();

    private void Start()
    {
        platesCounter.OnPlateSpawn += PlatesCounterOnPlateSpawn;
        platesCounter.OnPlateRemove += PlatesCounterOnPlateRemove;
    }

    private void PlatesCounterOnPlateSpawn(object sender, EventArgs e)
    {
        var plate = Instantiate(plateTransformPrefab, platesCounter.GetObjectSpawnPoint());
        float offsetY = 0.1f;

        plate.localPosition += new Vector3(0f, offsetY * _spawnPlateList.Count, 0f);
        _spawnPlateList.Add(plate);
    }
    
    private void PlatesCounterOnPlateRemove(object sender, EventArgs e)
    {
        var plate = _spawnPlateList[^1];
        _spawnPlateList.Remove(plate);
        Destroy(plate.gameObject);
    }

}
