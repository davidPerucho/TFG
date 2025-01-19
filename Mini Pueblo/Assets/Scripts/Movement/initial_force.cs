using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Clase que se encarga de manejar el código destinado a aplicar una fuerza inicial sobre un objeto.
/// </summary>
public class initial_force : MonoBehaviour
{
    public float force; //La furza añadida al instanciar el objeto
    public float distanceToDestroy; //Distancia del origen a la que el objeto será destruido
    public bool touchDirection; //True si se quiere que el objeto vaya en la dirección del ratón
    public bool objectDirection; //True si se quiere que el objeto vaya en la dirección de un objeto
    public GameObject targetObject; //El objeto hacia el que se dirigirá la fuerza si objectDirection es true
    public float directionX; //Dirección específica en X
    public float directionY; //Dirección específica en Y
    public bool specificX = false; //True si se quiere una dirección X específica
    public bool specificY = false; //True si se quiere una dirección Y específica
    Rigidbody2D rb; //RigidBody del objeto
    Vector2 initialPosition; //Posición original del objeto

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        Vector2 direction;

        if (touchDirection == true)
        {
            Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            /**if (mouseScreenPosition.x <= transform.position.x)
            {
                mouseScreenPosition.x = transform.position.x + (transform.position.x - mouseScreenPosition.x);
                mouseScreenPosition.y = transform.position.y + (transform.position.y - mouseScreenPosition.y);
            }**/

            if (specificX == true)
            {
                mouseScreenPosition.x = directionX;
            }
            if (specificY == true)
            {
                mouseScreenPosition.y = directionY;
            }

            direction = new Vector2(mouseScreenPosition.x - transform.position.x, mouseScreenPosition.y - transform.position.y).normalized;
        }
        else
        {
            Vector2 objectDirection = new Vector2(targetObject.transform.position.x - transform.position.x, targetObject.transform.position.y - transform.position.y);
            if (specificX == true)
            {
                objectDirection.x = directionX;
            }
            if (specificY == true)
            {
                objectDirection.y = directionY;
            }
            direction = objectDirection.normalized;
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

    /// <summary>
    /// Devielve la distancia a la que se encuentra el objeto de su punto de partida.
    /// </summary>
    /// <returns>Distancia con el punto de origen.</returns>
    float getDistanceToSpawn()
    {
        return Mathf.Sqrt(Mathf.Pow(initialPosition.x - transform.position.x, 2) + Mathf.Pow(initialPosition.y - transform.position.y, 2));
    }
}
