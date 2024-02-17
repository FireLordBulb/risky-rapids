using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        foreach (ParticleSystem particle in particles)
        {
            particle.Simulate(0, true, true);
            particle.Play();
        }
    }
}
