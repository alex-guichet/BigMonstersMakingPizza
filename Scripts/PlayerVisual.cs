using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer mouthMeshRenderer;

    private Material material;

    private void Awake()
    {
        material = new Material(bodyMeshRenderer.material);
    }

    private void Start()
    {
        bodyMeshRenderer.material = material;
        mouthMeshRenderer.material = material;
    }

    public void SetMaterialColor(Color color)
    {
        material.color = color;
    }
}
