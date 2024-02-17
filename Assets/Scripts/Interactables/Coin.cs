using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : InteractableClass
{
    private Vector3 currentPos;
    protected override void Start()
    {
        base.Start();
        currentPos = transform.position;
    }

    public override void OnCollisionDetected()
    {
        gameObject.SetActive(false);
        GameManager.Instance.Coins++;
        AudioManager.Instance.PlayCoinstAudio();
    }

    public override void Activate()
    {
        base.Activate();
        transform.position = currentPos;
    }
}
