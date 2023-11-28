using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/KitchenObject", fileName = "KitchenObjectSO")]
public class KitchenObjectSO : ScriptableObject
{
    public KitchenObject prefab;
    public Sprite sprite;
    public string name;
    public KitchenObjectSO kitchenObjectSoCooked;
}
