using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class initial_force : MonoBehaviour
{
    public float force; //The force that its going to be added on the strat
    public float distanceToDestroy; //The distance at wich the object will be destroyed
    public bool touchDirection; //If true the force will be applied in the direction of the touch
    public bool objectDirection; //If true the force will be applied in the directio of an object
    public GameObject targetObject; //The object that will mark the direction of the force
    public bool specificDirecton; //If true the force will apply in a specific direction
    public float directionX; //Specific direction X
    public float directionY; //Specfic direction Y
    Rigidbody2D rb;
    Vector2 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        Vector2 direction;

        if (specificDirecton == true)
        {
            direction = new Vector2(directionX, directionY).normalized;
        }
        else if (touchDirection == true)
        {
            Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mouseScreenPosition.x <= transform.position.x)
            {
                mouseScreenPosition.x = transform.position.x + (transform.position.x - mouseScreenPosition.x);
                mouseScreenPosition.y = transform.position.y + (transform.position.y - mouseScreenPosition.y);
            }
            direction = new Vector2(mouseScreenPosition.x - transform.position.x, mouseScreenPosition.y - transform.position.y).normalized;
        }
        else
        {
            direction = new Vector2(targetObject.transform.position.x - transform.position.x, targetObject.transform.position.y - transform.position.y).normalized;
        }

        rb.AddForce(direction*force);
    }

    // Update is called once per frame
    void Update()
    {
        if (getDistanceToSpawn() >= distanceToDestroy)
        {
            Destroy(gameObject);
        }
    }

    //Returns the distance from the object to the spawn position
    float getDistanceToSpawn()
    {
        return Mathf.Sqrt(Mathf.Pow(initialPosition.x - transform.position.x, 2) + Mathf.Pow(initialPosition.y - transform.position.y, 2));
    }
}
