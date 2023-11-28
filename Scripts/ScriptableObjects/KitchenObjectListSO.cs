using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/KitchenObjectListSO", fileName = "KitchenObjectListSO")]
public class KitchenObjectListSO : ScriptableObject
{
    public List<KitchenObjectSO> KitchenObjectSoList;
}
