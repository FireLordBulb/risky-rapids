using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Interactable
{
    private Vector3 startPosition;
    protected override void Start()
    {
        base.Start();
        startPosition = transform.position;
    }
    protected override void Interact(Collider other)
    {
        gameObject.SetActive(false);
        GameManager.Instance.Coins++;
        AudioManager.Instance.PlayCoinAudio();
    }

    public override void ResetInteractable()
    {
        base.ResetInteractable();
        transform.position = startPosition;
    }
}
