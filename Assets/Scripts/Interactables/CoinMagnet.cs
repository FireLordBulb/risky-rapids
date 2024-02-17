using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Serialization;

public class CoinMagnet : MonoBehaviour
{
    
    [SerializeField] 
    private bool IsDragging;

    [SerializeField]
    private float currentStrenght, magnetStrenght,magnetPullStrengPerUpgrade;

    [SerializeField] 
    private float magnetSize;

    [SerializeField] 
    private float magnetSizePerLevel;

    [SerializeField] 
    private int magnetLevel = 0, CoinsCollected;

    private Stats stats;

    [SerializeField] private SphereCollider magnetCollider;

    private void Start()
    {
        magnetCollider = GetComponent<SphereCollider>();
        magnetSize = GetComponent<SphereCollider>().radius;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            if (magnetLevel > 0)
            {
                DragObject(other.gameObject);
            }
        }
    }

    private void DragObject(GameObject dragedObject)
    {
        if (IsDragging)
        {
            //Debug.Log("dragging");
            dragedObject.transform.position = Vector3.MoveTowards(dragedObject.transform.position, transform.position, MagnetScaling(magnetStrenght, dragedObject) * Time.deltaTime);   
        }
        
    }

    private float MagnetScaling(float strenght, GameObject gameObject)
    {
        strenght = 
        currentStrenght = strenght/(Vector3.Distance(gameObject.transform.position, transform.position));
        return currentStrenght;
    }

    public void LevelUpMagnet()
    {
        magnetLevel = UpgradeHolder.Instance.GetUpgradeLevel(UpgradeType.Magnet);
        AdjustSizeMagnet();
        AdjustStrenghtMagnet();
    }

    public void AdjustSizeMagnet()
    {
        magnetSize += magnetSizePerLevel * magnetLevel;
        magnetCollider.radius = magnetSize;
    }

    public void AdjustStrenghtMagnet()
    {
        magnetStrenght += magnetPullStrengPerUpgrade * magnetLevel;
    }
}
