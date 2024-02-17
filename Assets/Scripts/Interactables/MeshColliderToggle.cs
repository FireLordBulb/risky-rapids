using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderToggle : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider;
    private void Awake()
    {
        meshCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        meshCollider.enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        meshCollider.enabled = false;
    }
}
