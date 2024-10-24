using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_towards : MonoBehaviour
{
    public bool toObject; //Indicates if the rotation needs to be towards an object
    public GameObject rotationObject; //Reference for the game object rotation
    public bool toMouse; //Indicates if the rotation needs to be towards the mouse

    // Update is called once per frame
    void Update()
    {
        if (toObject == true)
        {
            float anguloRad = Mathf.Atan2(rotationObject.transform.position.y - transform.position.y, rotationObject.transform.position.x - transform.position.x);
            float anguloDeg = (180 / Mathf.PI) * anguloRad;
            transform.rotation = Quaternion.Euler(0, 0, anguloDeg);
        }
        else
        {
            Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mouseScreenPosition.x <= transform.position.x)
            {
                mouseScreenPosition.x = transform.position.x + (transform.position.x - mouseScreenPosition.x);
                mouseScreenPosition.y = transform.position.y + (transform.position.y - mouseScreenPosition.y);
            }
            float anguloRad = Mathf.Atan2(mouseScreenPosition.y - transform.position.y, mouseScreenPosition.x - transform.position.x);
            float anguloDeg = (180 / Mathf.PI) * anguloRad;
            transform.rotation = Quaternion.Euler(0, 0, anguloDeg);
        }
    }
}
