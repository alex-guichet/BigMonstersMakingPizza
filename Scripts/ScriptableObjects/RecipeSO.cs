using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/RecipeSo", fileName = "RecipeSo")]
public class RecipeSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSoList;
    public string recipeName;
}
