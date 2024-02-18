using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected virtual void Start()
    {
        GameManager.Instance.CacheInteractable(this);
    }
    public virtual void ResetInteractable()
    {
        gameObject.SetActive(true);
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Interact(other);
        }
    }
    protected abstract void Interact(Collider other);
}
