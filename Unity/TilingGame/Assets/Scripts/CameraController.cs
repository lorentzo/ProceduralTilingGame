using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera follows the player.
    public GameObject player;
    Transform originalCameraTransform;
    public float distanceToPlayer = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        originalCameraTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        Vector3 translationDelta = player.transform.position - transform.position;
        translationDelta.y = originalCameraTransform.position.y;
        Debug.Log("Camera transformD: " + translationDelta);
        transform.position += translationDelta;
        */
        transform.LookAt(player.transform);
        //transform.position = new Vector3(player.transform.position.x + distanceToPlayer, distanceToPlayer, player.transform.position.y + distanceToPlayer);
    }
}
