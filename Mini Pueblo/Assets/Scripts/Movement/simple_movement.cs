using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simple_movement : MonoBehaviour
{
    public Vector2 movementDirection; //Direction of the movement
    public float distanceToDestroy; //At what distance from the origin do you want the object to destroy its self
    public bool randomSpeed; //If true the speed at wich the objects moves will be a random value
    public float maxSpeed;  //Maximum speed of the object
    public float minSpeed;  //Minimum speed of the object
    public float movementSpeed; //The speed in qich the object is going to move if the speed is not random
    Vector2 movementVector; //The movement vector created from the movementX and movementY variables
    public bool dynamicDifficulty; //The speed will increase as the player scores more points
    public int pointsToIncrease; //The amount of points needed to increase the difficulty
    float speed;
    Vector2 initialPosition;
    int lastIncrease; //The last time the difficulty was increased

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        movementVector = movementDirection.normalized;

        if(randomSpeed == true)
        {
            speed = Random.Range(minSpeed, maxSpeed);
        }
        else
        {
            speed = movementSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + movementVector.x * speed * Time.deltaTime, transform.position.y + movementVector.y * speed * Time.deltaTime, transform.position.z);

        if (getDistanceToSpawn() >= distanceToDestroy)
        {
            Destroy(gameObject);
        }

        if (dynamicDifficulty == true)
        {
            if (pointsToIncrease <= (FindAnyObjectByType<points_system>().points - lastIncrease))
            {
                lastIncrease = FindAnyObjectByType<points_system>().points;
                DifficultyManager.instance.increaseSpeed(0.1f);
            }
            
            speed = movementSpeed * DifficultyManager.instance.speedMultiplier;
        }
    }

    //Returns the distance from the object to the spawn position
    float getDistanceToSpawn()
    {
        return Mathf.Sqrt(Mathf.Pow(initialPosition.x-transform.position.x, 2) + Mathf.Pow(initialPosition.y - transform.position.y, 2));
    }
}
