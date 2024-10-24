using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simple_spawner : MonoBehaviour
{
    public GameObject spawnObject; //The object that its going to be spawned
    public Vector2 spawnPoint; //The coordinates in wich the object is going to spawn
    public bool randomSpawner; //If true the spawn cordinates of the objects will be random
    public Vector2 rangeY;  //The range of Y values at wich the object could be spawned
    public Vector2 rangeX;  //The range of X values at wich the object could be spawned
    public bool multipleSpawns; //If true there will an spawn each secondsBetweenSpawns
    public bool randomTime; //If true the time between spawns will be random
    public Vector2 rangeTime; //The range of time till next stop
    public float secondsBetweenSpawns;  //The time between spawns
    public bool dynamicDifficulty; //The time of spawn will get sorter as the player scores more points
    public int pointsToIncrease; //The amount of points needed to increase the dificulty
    int lastIncrease = 0; //The last time the difficulty was increased

    // Start is called before the first frame update
    void Start()
    {
        if (multipleSpawns == false)
        {
            if (randomSpawner == true)
            {
                Instantiate(spawnObject, new Vector3(Random.Range(rangeX.x, rangeX.y), Random.Range(rangeY.x, rangeY.y), transform.position.z), Quaternion.identity);
            }
            else
            {
                Instantiate(spawnObject, new Vector3(spawnPoint.x, spawnPoint.y, transform.position.z), Quaternion.identity);
            }
        }
        else
        {
            StartCoroutine(spawner());
        }
    }

    void Update()
    {
        if (dynamicDifficulty == true && pointsToIncrease <= (FindAnyObjectByType<points_system>().points - lastIncrease))
        {
            lastIncrease = FindAnyObjectByType<points_system>().points;
            if (secondsBetweenSpawns > 4)
            {
                secondsBetweenSpawns--;
            }
        }
    }

    IEnumerator spawner()
    {
        while (true)
        {
            if (randomSpawner == true)
            {
                Instantiate(spawnObject, new Vector3(Random.Range(rangeX.x, rangeX.y), Random.Range(rangeY.x, rangeY.y), transform.position.z), Quaternion.identity);
            }
            else
            {
                Instantiate(spawnObject, new Vector3(spawnPoint.x, spawnPoint.y, transform.position.z), Quaternion.identity);
            }

            if (randomTime == true)
            {
                yield return new WaitForSeconds(Random.Range(rangeTime.x, rangeTime.y));
            }
            else
            {
                yield return new WaitForSeconds(secondsBetweenSpawns);
            }
        }
    }
}
