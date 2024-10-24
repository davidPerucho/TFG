using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class camera_movement : MonoBehaviour
{
    public GameObject player;
    float initialZ;
    float distanceToZoom = 0.9f;
    Vector3 offset = new Vector3(0, 4.84f, 5.53f - 10.58f);
    Vector3 zoomOffset = new Vector3(0, 2.99f, -7.95f);
    Vector3 zoomDirection;
    Vector3 lastZoomOut;

    float zoomSpeed = 3.2f;
    public bool zoomInNow = false;
    public bool zoomOutNow = false;
    public bool zoomInFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        lastZoomOut = transform.position;
        zoomDirection = new Vector3(0, transform.position.y - zoomOffset.y, transform.position.z - zoomOffset.z).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if ((zoomInNow == true && distance(player.transform.position, player.GetComponent<NavMeshAgent>().destination) < distanceToZoom) || zoomOutNow == true)
        {
            if (zoomInNow == true)
            {
                zoomOffset.z = initialZ + 2.63f;
                zoomIn();
            }
            if (zoomOutNow == true)
            {
                zoomOut();
            }
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, player.transform.position.z + offset.z);
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
        zoomDirection = new Vector3(0, lastZoomOut.y - transform.position.y, lastZoomOut.z - transform.position.z).normalized;

        if (transform.position.y < lastZoomOut.y && transform.position.z > lastZoomOut.z)
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
