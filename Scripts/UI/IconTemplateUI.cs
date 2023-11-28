using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTemplateUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetIconImage(Sprite iconImage)
    {
        icon.sprite = iconImage;
    }
}
