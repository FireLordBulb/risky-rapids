using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [Header("Boatskin")]
    [SerializeField] private List<MeshRenderer> boatMaterials;
    [SerializeField] private List<MeshRenderer> oarMaterials;
    [Header("BoatSkinMaterial")]
    [SerializeField] private Material currentBoatMaterial;
    [SerializeField] private Material defaultBoatMaterial;
    [SerializeField] private Material speedPaddleMaterial;
    [Header("Magnet")]
    [SerializeField] private GameObject magnetObject;
    [SerializeField] private CoinMagnet magnet;
    [Header("Armor")]
    [SerializeField] private GameObject armorObject;
    
    
    public void ActivateMagnet()
    {
        magnetObject.SetActive(true);
        magnet.LevelUpMagnet();
    }
    
    public void ActivateArmor()
    {
        armorObject.SetActive(true);
    }
    public void ApplyBoatMaterial(Material material)
    {
      
        
        currentBoatMaterial = material == null ? defaultBoatMaterial : material;
        foreach (MeshRenderer boatMesh in boatMaterials)
        {
            boatMesh.material = currentBoatMaterial;
        }
        UpdatePaddleMaterial();
    }

    public void UpdatePaddleMaterial()
    {
        Material material = UpgradeHolder.Instance.GetUpgradeLevel(UpgradeType.Paddle) == 0 ? defaultBoatMaterial : speedPaddleMaterial;
        foreach (MeshRenderer oarMesh in oarMaterials)
        {
            oarMesh.material = material;
        }
        
    }
    private void OnApplicationQuit()
    {
        foreach (MeshRenderer mesh in boatMaterials)
        {
            mesh.material = defaultBoatMaterial;
        }
        foreach (MeshRenderer mesh in oarMaterials)
        {
            mesh.material = defaultBoatMaterial;
        }
    }
}
