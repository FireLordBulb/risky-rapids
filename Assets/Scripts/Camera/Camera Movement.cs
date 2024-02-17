using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    [Tooltip("The object with the player script will automatically populate this on Start() with the player.")]
    public Transform targetToFollow;
    
    [Tooltip("If this is close to 0 then the camera will slowly catch up to the player. " +
             "If it is 1 then it the camera will catch up instantely to the player")]
    public float smoothCameraMoveSpeed = 0.2f;
    
    [Tooltip("This is how far behing the player the camera will be. Y for up and down, Z in minus to to behind the player")]
    public Vector3 distanceCameraToPlayer = new Vector3(0,3.5f,-5);
    
    public void Start()
    {
        targetToFollow = FindObjectOfType<Player>().transform;
    }
    public void LateUpdate()
    {
        Vector3 desiredPosition = targetToFollow.position + distanceCameraToPlayer;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothCameraMoveSpeed);
        transform.position = smoothedPosition;
    }
}
