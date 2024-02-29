using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishLine : Interactable
{
    protected override void Interact(Collider other)
    {
        AudioManager.Instance.PlayCompletedLevelAudio();
        GameManager.Instance.CompleteLevel();
    }
}
