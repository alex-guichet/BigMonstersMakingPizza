using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/RecipeListSo", fileName = "RecipeListSo")]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSoList;
}
