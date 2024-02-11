using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on: https://catlikecoding.com/unity/tutorials/movement/

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    // Camera follows the player.
    public GameObject player;
    public float distanceToPlayer = 10.0f;
    public float maxDistanceToPlayer = 20.0f;
    public float minDistanceToPlayer = 5.0f;
    public float distanceDeltaSensitivity = 0.5f;
    private Vector3 focusPoint;
    public float focusRadius = 1.0f;

    void Awake()
    {
        focusPoint = player.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        float desiredDistanceToPlayer = distanceToPlayer;
        if (Input.GetKey("e"))
        {
            desiredDistanceToPlayer += distanceDeltaSensitivity;
            if (desiredDistanceToPlayer > maxDistanceToPlayer)
            {
                desiredDistanceToPlayer = maxDistanceToPlayer;
            }
        }
        if (Input.GetKey("q"))
        {
            desiredDistanceToPlayer -= distanceDeltaSensitivity;
            if (desiredDistanceToPlayer < minDistanceToPlayer)
            {
                desiredDistanceToPlayer = minDistanceToPlayer;
            }
        }
        distanceToPlayer = desiredDistanceToPlayer;
    }

    // Player is the focus. It can be moved in Update. Therefore, Camera is adapted in LateUpdate.
    void LateUpdate()
    {
        UpdateFocusPoint();
        Vector3 lookDirection = transform.forward; // Camera's forward is already defined by inspector
        transform.localPosition = focusPoint - lookDirection * distanceToPlayer;
    }

    void UpdateFocusPoint()
    {
        Vector3 targetPoint = player.transform.position;
        if (focusRadius > 0.0f)
        {
            // Move camera only if target point (player) is outside of focus.
            float targetPtFocusPtDistance = Vector3.Distance(targetPoint, focusPoint);
            if (targetPtFocusPtDistance > focusRadius)
            {
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, focusRadius / targetPtFocusPtDistance);
            }
        }
        else
        {
            focusPoint = targetPoint;
        }
    }
}
