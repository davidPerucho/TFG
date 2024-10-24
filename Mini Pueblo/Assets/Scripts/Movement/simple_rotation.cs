using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simple_rotation : MonoBehaviour
{
    public bool randomRotation; //If true the direction of rotation will be random
    public bool rotationRight; //If true the object rotation will be positive
    public bool rotationLeft; //If true the object rotation will be negative
    public float rotationSpeed; //The speed at wich the object will rotate
    public bool rotationAxisX; //If true the object will rotate around the X axis
    public bool rotationAxisY; //If true the object will rotate around the Y axis
    public bool rotationAxisZ; //If true the object will rotate around the Z axis
    float rotationAngle; //How much will the object rotate in each frame

    // Start is called before the first frame update
    void Start()
    {
        //Error check
        if (rotationRight == false && rotationLeft == false && randomRotation == false)
        {
            Debug.LogError("A type of rotation must be selected");
        }

        if (rotationAxisX == false && rotationAxisY == false && rotationAxisZ == false)
        {
            Debug.LogError("At least one axis of rotation must be selected");
        }


        //Initialize rotation angle
        if (randomRotation == true)
        {
            float rand = Random.Range(-1, 1);

            if (rand >= 0)
            {
                rotationAngle = -1 * rotationSpeed;
            }
            else
            {
                rotationAngle = 1 * rotationSpeed;
            }
        }
        else
        {
            if (rotationRight == true)
            {
                rotationAngle = -1 * rotationSpeed;
            }
            else
            {
                rotationAngle = 1 * rotationSpeed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorRotation = Vector3.zero;

        if (rotationAxisX)
        {
            vectorRotation.x = rotationAngle * rotationSpeed * Time.deltaTime;
        }
        if (rotationAxisY)
        {
            vectorRotation.y = rotationAngle * rotationSpeed * Time.deltaTime;
        }
        if (rotationAxisZ)
        {
            vectorRotation.z = rotationAngle * rotationSpeed * Time.deltaTime;
        }

        transform.Rotate(vectorRotation, Space.World);
    }
}
