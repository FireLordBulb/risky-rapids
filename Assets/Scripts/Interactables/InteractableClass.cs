using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableClass : MonoBehaviour, IInteractable
{
    protected virtual void Start()
    {
        Register();
    }

    public virtual void OnCollisionDetected(){}
    public virtual void OnCollisionDetected(Player player){}
    public virtual void OnCollisionDetected(Player player, Vector3 collisionPoint){}
    
    public virtual void Reset()
    {
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false); 
    }

    public void Register()
    {
        GameManager.Instance.CacheInteractable(this);
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        if (CheckIfCoinComponent(other)) return;
        
        var player = other.GetComponentInParent<Player>();
        if (player == null) return;
        OnCollisionDetected();
    }

    protected bool CheckIfCoinComponent(Collider colliderObject)
    {
        return colliderObject.CompareTag("CoinComponents");
    }
}
