using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : InteractableClass
{
    [SerializeField] private SO_ObstacleData obstacleData;
    [SerializeField] private long vibrateLenght;
    private BoatPhysics boat;
    public override void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        boat = other.GetComponentInParent<BoatPhysics>();
        ApplyKnockbackToBoat();
        
        float collisionIntensity = boat.Rigidbody.velocity.magnitude/boat.TopSpeed;
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
        //If..
    }
    
}
