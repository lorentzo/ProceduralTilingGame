using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectToFindController : MonoBehaviour
{
    public GameObject player;
    Rigidbody rb;
    bool isCurrentlyColliding;

    void OnCollisionEnter(Collision col) {
        isCurrentlyColliding = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (computeDistanceToPlayer() < 3.0f)
        {
            rb.useGravity = true;
        }
        if (isCurrentlyColliding)
        {
            rb.isKinematic = true;
        }
    }

    float computeDistanceToPlayer()
    {
        Vector3 PlayerCo = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);
        Vector3 ObjectTOFindCo = new Vector3(transform.position.x, 0.0f, transform.position.z);
        return Vector3.Distance(PlayerCo, ObjectTOFindCo);
    }
}
