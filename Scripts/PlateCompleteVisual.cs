using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct KitchenObjectSo_Ingredient
{
    public KitchenObjectSO KitchenObjectSo;
    public GameObject IngredientGameObject;
}

public class PlateCompleteVisual : MonoBehaviour
{
    
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private KitchenObjectSo_Ingredient[] kitchenObjectSoIngredientArray;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnIngredientAdded;
        plateKitchenObject.OnIngredientRemoved += PlateKitchenObjectOnIngredientRemoved;
        
        foreach (var kitchenObjectIngredient in kitchenObjectSoIngredientArray)
        {
            kitchenObjectIngredient.IngredientGameObject.SetActive(false);
        }
    }

    private void PlateKitchenObjectOnIngredientRemoved(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (var kitchenObjectIngredient in kitchenObjectSoIngredientArray)
        {
            if (kitchenObjectIngredient.KitchenObjectSo == e.KitchenObjectSo)
            {
                kitchenObjectIngredient.IngredientGameObject.SetActive(false);
            }
        }
    }

    private void PlateKitchenObjectOnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (var kitchenObjectIngredient in kitchenObjectSoIngredientArray)
        {
            if (kitchenObjectIngredient.KitchenObjectSo == e.KitchenObjectSo)
            {
                kitchenObjectIngredient.IngredientGameObject.SetActive(true);
            }
        }
    }

    /*
    public List<KitchenObject> GetKitchenObjectsActiveInPlate()
    {
        List<KitchenObject> activeKitchenObjectsInPlate = new();
        
        foreach (Transform kitchenObjectTransform in transform)
        {
            if (kitchenObjectTransform.gameObject.activeInHierarchy)
            {
                KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
                activeKitchenObjectsInPlate.Add(kitchenObject);
            }
        }
        return activeKitchenObjectsInPlate;
    }

    public bool HasIngredientsActive()
    {
        return transform.childCount != 0;
    }
    */
}
