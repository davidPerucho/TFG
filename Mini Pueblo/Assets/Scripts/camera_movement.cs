using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class camera_movement : MonoBehaviour
{
    public GameObject player; //Player 
    float initialZ; //The z axis value for when the zoom in starts
    public float distanceToZoom = 0.9f; //The distance between the player and the clicked npc at wich the camera starts to zoom in
    Vector3 cameraPosition; //The position that the camera should have without zoom
    [SerializeField]
    Vector3 offset = new Vector3(0, 4.84f, 5.53f - 10.58f); //The offset between the player and the camera when there is no zoom
    Vector3 zoomOffset = new Vector3(0, 2.99f, -7.95f); //The offset between the player and the camera when there is zoom
    Vector3 zoomDirection; //The direction that the camera will follow during the zoom
    Vector3 lastZoomOut; //Last position of the camera before starting to zoom

    public float zoomSpeed = 3.2f; //Speed of the camera zoom
    [HideInInspector]
    public bool zoomInNow = false; //True when the zoom in process has to start
    [HideInInspector]
    public bool zoomOutNow = false; //True when the zoom out process has to start
    [HideInInspector]
    public bool zoomInFinished = false; //True when the zoom in has finished

    // Start is called before the first frame update
    void Start()
    {
        lastZoomOut = transform.position;
        zoomDirection = new Vector3(0, transform.position.y - zoomOffset.y, transform.position.z - zoomOffset.z).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        cameraPosition = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, player.transform.position.z + offset.z);

        if ((zoomInNow == true && distance(player.transform.position, player.GetComponent<NavMeshAgent>().destination) < distanceToZoom) || zoomOutNow == true)
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
            transform.position = cameraPosition;
            initialZ = transform.position.z;
            lastZoomOut = transform.position;
        }
    }

    void zoomIn()
    {
        zoomDirection = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - zoomOffset.y, transform.position.z - zoomOffset.z).normalized;

        if (transform.position.y > zoomOffset.y && transform.position.z < zoomOffset.z)
        {
            transform.position = new Vector3(transform.position.x - zoomDirection.x * zoomSpeed * Time.deltaTime, transform.position.y - zoomDirection.y * zoomSpeed * Time.deltaTime, transform.position.z - zoomDirection.z * zoomSpeed * Time.deltaTime);
        }
        else
        {
            zoomInFinished = true;
        }
    }

    void zoomOut()
    {
        zoomDirection = new Vector3(cameraPosition.x - transform.position.x, cameraPosition.y - transform.position.y, cameraPosition.z - transform.position.z).normalized;

        if (transform.position.y <= cameraPosition.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + zoomDirection.y * zoomSpeed * Time.deltaTime, transform.position.z + zoomDirection.z * zoomSpeed * Time.deltaTime);
        }
        else
        {
            FindAnyObjectByType<player_movement>().stop = false;
            zoomOutNow = false;
        }
    }

    float distance(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2) + Mathf.Pow(pos2.y - pos1.y, 2));
    }
}
