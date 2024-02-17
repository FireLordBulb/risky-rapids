using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FinishLine : InteractableClass, IInteractable
{
    public override void OnCollisionDetected()
    {
        GameManager.Instance.EndGame();
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (CheckIfCoinComponent(other)) return;
        
        var player = other.GetComponentInParent<Player>();
        if (player)
        {
            AudioManager.Instance.PlayCompletedLevelAudio();
            OnCollisionDetected();
        }
    }
}
