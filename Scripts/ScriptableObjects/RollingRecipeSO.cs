using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/RollingRecipeSO", fileName = "RollingRecipeSO")]
public class RollingRecipeSO : ScriptableObject
{
   public KitchenObjectSO input;
   public KitchenObjectSO output;
   public int rollingProgressMax;
}
