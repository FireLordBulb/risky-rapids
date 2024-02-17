using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    
    
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, rotationSpeed * Time.deltaTime);    
    }
}
