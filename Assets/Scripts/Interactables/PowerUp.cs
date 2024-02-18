using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : Interactable
{
    [SerializeField] private float maxDuration;
    protected BoatPhysics Boat;
    private bool activated;
    private float duration;

    protected override void Start()
    {
        base.Start();
        Boat = GameObject.FindGameObjectWithTag("Player").GetComponent<BoatPhysics>();
    }
    public override void ResetInteractable()
    {
        base.ResetInteractable();
        activated = false;
    }
    public void FixedUpdate()
    {
        if (!activated)
        {
            return;
        }
        duration -= Time.fixedDeltaTime;
        if (0 < duration)
        {
            return;
        }
        gameObject.SetActive(false);
        Unapply();
    }
    protected override void Interact(Collider other)
    {
        Apply();
    }

    protected virtual void Apply()
    {
        activated = true;
        duration = maxDuration;
    }

    protected virtual void Unapply()
    {
        activated = false;
    }
}
