using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;
using UnityEngine.Serialization;

public class SpeedBoost : Powerup
{
    [SerializeField] private float boostWaterSpeed;
    [SerializeField] private float boostRowSpeed;
    [SerializeField] private float duration;

    private Player player;
    private MeshRenderer[] meshes;
    private bool activated;
    private float maxDuration;

    protected override void Start()
    {
        base.Start();
        meshes = GetComponentsInChildren<MeshRenderer>();
        maxDuration = duration;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    public void FixedUpdate()
    {
        if (!activated)
        {
            return;
        }
        duration -= Time.fixedDeltaTime;
        if (duration <= 0)
        {
            boat.ResetMovementForces();
            activated = false;
            player.StopSpeedVFX(false);
            base.Deactivate();
        }
    }
    
    protected override void PowerupActivation()
    {
        meshes.ForEach(mesh => mesh.enabled = false);
        boat.ResetMovementForces();
        boat.AddMovementForces(boostWaterSpeed, boostRowSpeed);
        activated = true;
        duration = maxDuration;
        player.StartSpeedVFX();
        AudioManager.Instance.PlayBoostAudio();
    }

    public override void Reset()
    {
        base.Reset();
        meshes.ForEach(mesh => mesh.enabled = true);
        boat.ResetMovementForces();
        activated = false;
        player.StopSpeedVFX(true);
    }
}
