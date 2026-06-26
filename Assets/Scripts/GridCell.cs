using JetBrains.Annotations;
using System;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer fillRenderer;

    public void SetColor(Color color)
    {
        fillRenderer.material.color = color;
    }

    public void SetVisible(bool visible)
    {
        fillRenderer.enabled = visible;
    }

}
