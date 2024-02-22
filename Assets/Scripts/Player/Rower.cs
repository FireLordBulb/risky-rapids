using System;
using UnityEngine;

[Serializable]
public class Rower
{
    [SerializeField] private Renderer bodyMesh;
    [SerializeField] private Renderer[] hairStyleMeshes;
    private Renderer activeHairStyle;
    public void SetActiveHairStyle(CharacterMesh mesh)
    {
        if (activeHairStyle != null)
        {
            activeHairStyle.gameObject.SetActive(false);
        }
        activeHairStyle = hairStyleMeshes[(int)mesh];
        activeHairStyle.gameObject.SetActive(true);
    }
    public void SetMaterial(Material material)
    {
        activeHairStyle.material = material;
        bodyMesh.material = material;
    }
}
