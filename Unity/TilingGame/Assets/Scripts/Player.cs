using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on: https://catlikecoding.com/unity/tutorials/movement/

public class Player : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public float maxAcceleration = 10.0f;
    Vector3 velocity, desiredVelocity;

    Rigidbody body;

    void Awake () {
		body = GetComponent<Rigidbody>();
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerInput = new Vector3(0,0,0);
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.z = Input.GetAxis("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1.0f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.z) * maxSpeed;
    }

    void FixedUpdate()
    {
        dynamicMovement();
    }

    void dynamicMovement()
    {
        velocity = body.velocity;

		float maxSpeedChange = maxAcceleration * Time.deltaTime;
		velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

		body.velocity = velocity;
    }

    void kinematicMovement()
    {
        // Can be used in Update() and if GameObject doesn't have Rigid Body attached.
        
        Vector3 playerInput = new Vector3(0,0,0);
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.z = Input.GetAxis("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1.0f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.z) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        transform.position += displacement;
    }
}
