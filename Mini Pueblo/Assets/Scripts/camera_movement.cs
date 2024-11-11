using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Clase que se encarga de manejar el movimiento de la cámara en el entorno 3D (Hub).
/// </summary>
public class camera_movement : MonoBehaviour
{
    public GameObject player; //El objeto que representa al jugador 
    float initialZ; //El valor en el eje z antes de que la cámara comience el zoomIn
    Vector3 cameraPosition; //La posición en la que la cámara se encuentra an cada moment (sin zoom)

    [SerializeField]
    Vector3 offset = new Vector3(0, 4.84f, 5.53f - 10.58f); //El offset entre la cámara y el jugador cuando no hay zoom

    Vector3 zoomOffset = new Vector3(0, 2.99f, -7.95f); //El offset entre la cámara y el jugador cuando hay zoom
    Vector3 zoomInDirection; //La dirección que seguirá la cámara durante el zoomIn
    Vector3 zoomOutDirection; //La dirección que seguirá la cámara durente el zoomOut

    public float zoomSpeed = 3.2f; //Velocidad del zoom de la cámara

    [HideInInspector]
    public bool zoomInNow = false; //Es true cuando se esta realizando el proceso de zoomIn

    [HideInInspector]
    public bool zoomOutNow = false; //Es true cuando se esta realizando el proceso de zoomOut

    [HideInInspector]
    public bool zoomInFinished = false; //Es true cuando ha terminado el proceso de zoomIn

    // Start is called before the first frame update
    void Start()
    {
        zoomInDirection = new Vector3(0, transform.position.y - zoomOffset.y, transform.position.z - zoomOffset.z).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        cameraPosition = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, player.transform.position.z + offset.z);

        if (zoomInNow == true || zoomOutNow == true)
        {
            if (zoomInNow == true)
            {
                zoomOffset.z = initialZ + 2.63f;
                zoomIn();
            }
            else if (zoomOutNow == true)
            {
                zoomOut();
            }
        }
        else
        {
            //En caso de no haber zoom la posición de la cámara es la posición del jugador más el offset
            transform.position = cameraPosition;
            initialZ = transform.position.z;
        }
    }

    /// <summary>
    /// Hacerca la camará en dirección al jugador para enmarcar mejor la conversación.
    /// </summary>
    void zoomIn()
    {
        zoomInDirection = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - zoomOffset.y, transform.position.z - zoomOffset.z).normalized;

        //Si se llega a la posición deseada el zoomIn termina
        if (transform.position.y > zoomOffset.y && transform.position.z < zoomOffset.z)
        {
            transform.position = new Vector3(transform.position.x - zoomInDirection.x * zoomSpeed * Time.deltaTime, transform.position.y - zoomInDirection.y * zoomSpeed * Time.deltaTime, transform.position.z - zoomInDirection.z * zoomSpeed * Time.deltaTime);
        }
        else
        {
            zoomInFinished = true;
            zoomOutDirection = new Vector3(cameraPosition.x - transform.position.x, cameraPosition.y - transform.position.y, cameraPosition.z - transform.position.z).normalized;
        }
    }

    /// <summary>
    /// Aleja la cámara desde la posición en la que se encuentra tras la función zoomIn hasta su posición normal.
    /// </summary>
    void zoomOut()
    {
        //Si se llega a la posición deseada el zoomOut termina
        if (transform.position.y < cameraPosition.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + zoomOutDirection.y * zoomSpeed * Time.deltaTime, transform.position.z + zoomOutDirection.z * zoomSpeed * Time.deltaTime);
        }
        else
        {
            FindAnyObjectByType<player_movement>().stop = false;
            zoomOutNow = false;
        }
    }
}
