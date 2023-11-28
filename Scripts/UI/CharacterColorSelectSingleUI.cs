using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorIndex;
    [SerializeField] private Image buttonImage;
    [SerializeField] private GameObject selectedObject;


    private void Awake()
    {
        buttonImage.GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplier.Instance.ChangePlayerColor(colorIndex);
        });
    }
    
    private void Start()
    {
        buttonImage.color = KitchenGameMultiplier.Instance.GetPlayerColor(colorIndex);
        KitchenGameMultiplier.Instance.OnPlayerDataListChanged += KichenGameMultiplierOnPlayerDataListChanged;
        SetCurrentSelected();
    }

    private void KichenGameMultiplierOnPlayerDataListChanged(object sender, EventArgs e)
    {
        SetCurrentSelected();
    }

    private void SetCurrentSelected()
    {
        selectedObject.SetActive(KitchenGameMultiplier.Instance.GetLocalPlayerData().colorId == colorIndex);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplier.Instance.OnPlayerDataListChanged -= KichenGameMultiplierOnPlayerDataListChanged;
    }
}
