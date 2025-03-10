using System;
using UnityEngine;

[Serializable]
public class Rower
{
    [SerializeField] private Renderer bodyMesh;
    [SerializeField] private Renderer[] hairStyleMeshes;
    private MaterialData materialData;
    private Renderer activeHairStyle;
    private RowerMesh currentMesh;
    private RowerColor currentColor;

    public void Initialize(MaterialData data)
    {
        materialData = data;
        activeHairStyle = hairStyleMeshes[0];
        currentMesh = 0;
        currentColor = 0;
    }
    public void SetMesh(RowerMesh mesh)
    {
        currentMesh = mesh;
        if (activeHairStyle != null)
        {
            activeHairStyle.gameObject.SetActive(false);
        }
        activeHairStyle = hairStyleMeshes[(int)currentMesh];
        activeHairStyle.gameObject.SetActive(true);
        SetColor(currentColor);
    }
    public void SetColor(RowerColor color)
    {
        currentColor = color;
        Material material = materialData.GetMaterial(currentMesh, currentColor);
        activeHairStyle.material = material;
        bodyMesh.material = material;
    }
}
