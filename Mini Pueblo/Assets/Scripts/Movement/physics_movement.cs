using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physics_movement : MonoBehaviour
{
    public Vector2 movementDirection; //The direction in wich the object is going to move
    public float maxSpeed; //The speed in qich the object is going to move
    public float force; //The amount of force that will be applied to the object
    Vector2 movementVector; //The movement vector created from the movementX and movementY variables
    Rigidbody2D rb;
    float currentSpeed;
    bool pressed = false;
    float defaultForce;

    // Start is called before the first frame update
    void Start()
    {
        defaultForce = force;
        rb = GetComponent<Rigidbody2D>();
        movementVector = movementDirection.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = Mathf.Sqrt(rb.velocity.x*rb.velocity.x + rb.velocity.y*rb.velocity.y);

        if (Input.GetMouseButtonDown(0))
        {
            pressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
        }
    }

    private void FixedUpdate()
    {
        if (pressed == true)
        {
            if (currentSpeed > maxSpeed)
            {
                force = 0;
            }
            else
            {
                force = defaultForce;
            }

            rb.AddForce(new Vector2(0, force));
        }
    }
}
