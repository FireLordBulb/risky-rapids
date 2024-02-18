using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;
using UnityEngine.Serialization;

public class SpeedBoost : PowerUp
{
    [SerializeField] private float boostWaterSpeed;
    [SerializeField] private float boostRowSpeed;

    private Player player;
    private MeshRenderer[] meshes;
    
    protected override void Start()
    {
        base.Start();
        meshes = GetComponentsInChildren<MeshRenderer>();
        player = Boat.GetComponent<Player>();
    }
    public override void ResetInteractable()
    {
        base.ResetInteractable();
        meshes.ForEach(mesh => mesh.enabled = true);
        player.ActiveSpeedBoosts = 0;
        Boat.ResetMovementForces();
        player.StopSpeedVFX(true);
    }
    protected override void Apply()
    {
        base.Apply();
        meshes.ForEach(mesh => mesh.enabled = false);
        print($"ActiveSpeedBoosts: {player.ActiveSpeedBoosts}");
        if (player.ActiveSpeedBoosts == 0)
        {
            Boat.AddMovementForces(boostWaterSpeed, boostRowSpeed);
            player.StartSpeedVFX();
            print("speed");
        }
        player.ActiveSpeedBoosts++;
        AudioManager.Instance.PlayBoostAudio();
    }
    protected override void Unapply()
    {
        base.Unapply();
        player.ActiveSpeedBoosts--;
        if (player.ActiveSpeedBoosts == 0)
        {
            Boat.ResetMovementForces();
            player.StopSpeedVFX(false);
        }
    }
}
