using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on: https://catlikecoding.com/unity/tutorials/movement/

public class Player : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public float maxAcceleration = 10.0f;
    Vector3 velocity, desiredVelocity;

    bool jump;
    bool onGround;
    public float jumpHeight = 2.0f;

    Rigidbody body;

    void OnCollisionEnter (Collision collision) 
    {
		EvaluateCollision(collision);
	}

    // OnCollisionStay gets invoked each physics step as long as the collision remains alive. 
    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

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
        // If player falls below the world, restart position.
        if (transform.position.y < -3.0f)
        {
            transform.position = new Vector3(0.0f, 3.0f, 0.0f);
        }
        // Horizontal and vertical (x,z) movement.
        Vector3 playerInput = new Vector3(0,0,0);
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.z = Input.GetAxis("Vertical");
        playerInput = Vector3.ClampMagnitude(playerInput, 1.0f);
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.z) * maxSpeed;

        // Jumping (y) movement.
        jump = jump | Input.GetButtonDown("Jump");
    }

    void FixedUpdate()
    {
        velocity = body.velocity; // Take current velocty
        dynamicMovement(); // Compute new (x,z) velocity
        if(jump)
        {
            dynamicJump(); // Compute new (y) velocity
            jump = false;
        }
        body.velocity = velocity; // Assign curent velocity
        onGround = false;
    }

    void dynamicMovement()
    {
		float maxSpeedChange = maxAcceleration * Time.deltaTime;
		velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
    }

    void dynamicJump()
    {
        if(onGround)
        {
            velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        }
    }

    void EvaluateCollision(Collision collision)
    {
        // Check normal of each contact point.
        for (int i = 0; i < collision.contactCount; i++) 
        {
			Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f; // onGround is only when normal is close to ground normal.
		}
    }

    void kinematicMovement()
    {
        // Obsolete.
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
