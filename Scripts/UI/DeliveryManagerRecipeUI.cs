using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerRecipeUI : MonoBehaviour
{
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private TextMeshProUGUI recipeTitle;
    [SerializeField] private Image iconTemplate;

    private void Start()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetVisual(RecipeSO recipeSo)
    {
        recipeTitle.text = recipeSo.recipeName;

        foreach (Transform child in recipeContainer)
        {
            if (child == iconTemplate.transform)
                continue;
            Destroy(child);
        }

        foreach (var kitchenObjectSo in recipeSo.kitchenObjectSoList)
        {
            var iconImage = Instantiate(iconTemplate, recipeContainer);
            iconImage.sprite = kitchenObjectSo.sprite;
            iconImage.gameObject.SetActive(true);
        }
    }
}
