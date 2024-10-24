using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_spawner : MonoBehaviour
{
    public GameObject spawnObject; //The object that will be spawned
    public bool onReleased; //If true the object will spawn when the screen is released
    public bool onPressed; //If true the object will spawn when the screen is pressed
    public bool continuousSpawn; //If the object will continue spawning while the screen is pressed
    public float timeToSpawn; //Time till the next spawn
    bool pressed = false; //True if the screen is pressed
    float remainiingTime = 0; //Time remaining till the next spawn

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pressed = true;
            if (onPressed == true && continuousSpawn == false)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
            if (onReleased == true)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
            }
        }

        if (pressed == true && onPressed == true && continuousSpawn == true)
        {
            if (remainiingTime <= 0)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
                remainiingTime = timeToSpawn;
            }
        }

        remainiingTime -= Time.deltaTime;
    }
}
