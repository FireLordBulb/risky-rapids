using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Interactable
{
    [SerializeField] private SO_ObstacleData obstacleData;
    [SerializeField] private long vibrateLenght;
    private BoatPhysics boat;
    protected override void Interact(Collider other)
    {
        boat = other.GetComponentInParent<BoatPhysics>();
        ApplyKnockbackToBoat();
        
        float collisionIntensity = boat.Speed/boat.TopSpeed;
        float damage = Mathf.Lerp(obstacleData.minimumDamage, obstacleData.maximumDamage, collisionIntensity);
        damage = Mathf.Round(damage);
        boat.GetComponent<Player>().DecreaseHealth(damage);
        
        TriggerHapticFeedback();
        PlayAudio();
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;
        ApplyKnockbackToBoat();
    }
    private void ApplyKnockbackToBoat()
    {
        Vector3 collisionNormal = boat.transform.position - transform.position;
        boat.AddKnockBack(obstacleData.knockback, collisionNormal, obstacleData.minSpeedScalar);
    }
    private void TriggerHapticFeedback()
    {
        HapticFeedbackManager.CustomVibration(vibrateLenght);
    }
    private void PlayAudio()
    {
        AudioManager.Instance.PlayCollisionStoneAudio();
    }
    
}
