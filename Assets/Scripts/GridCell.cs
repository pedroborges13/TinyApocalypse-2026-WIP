using JetBrains.Annotations;
using System;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer fillRenderer;
    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }
    
    public void SetColor(Color color)
    {
        fillRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", color);
        fillRenderer.SetPropertyBlock(propertyBlock);
    }
}
