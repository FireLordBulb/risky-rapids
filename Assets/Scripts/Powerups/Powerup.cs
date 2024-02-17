using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : InteractableClass
{
    protected BoatPhysics boat;

    protected override void Start()
    {
        base.Start();
        boat = GameObject.FindGameObjectWithTag("Player").GetComponent<BoatPhysics>();
    }

    public override void OnCollisionDetected()
    {
        base.OnCollisionDetected();
        PowerupActivation();
    }
   

    protected abstract void PowerupActivation();
}
