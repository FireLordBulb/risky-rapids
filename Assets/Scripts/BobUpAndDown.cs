using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{
    [SerializeField] private float peak;
    [SerializeField] private float cycle;
    [SerializeField] private bool randomizeStart;
    private float current;
    private float previousOffset;
    void Start()
    {
        current = randomizeStart ? Random.Range(0, cycle) : cycle;
    }
    void Update()
    {
        current -= Time.deltaTime;
        float angle = 2 * Mathf.PI * current/cycle;
        float offset = peak * Mathf.Sin(angle);
        transform.position += (offset-previousOffset) * transform.up;
        previousOffset = offset;
    }
}
