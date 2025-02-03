using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public ObjectWithProbability[] spawnObjects; //Array with diferent objects and their possibilities of spawning
    GameObject spawnedObject; //The last object to be spawned. Null if there is no object

    // Start is called before the first frame update
    void Start()
    {
        float probabilitiesSum = 0;

        for (int i = 0; i < spawnObjects.Length; i++)
        {
            if (spawnObjects[i].probability > 1)
            {
                Debug.LogError("The probability of spawn of an object can't be greater than one.");
            }

            probabilitiesSum += spawnObjects[i].probability;
        }

        if (probabilitiesSum > 1)
        {
            Debug.LogError("The sum of the probabilities can't be greater than one.");
            spawnedObject = new GameObject("Error");
        }
        else if (probabilitiesSum != 1)
        {
            Debug.LogError("The sum of the probabolities must add up to one.");
            spawnedObject = new GameObject("Error");
        }
        else
        {
            spawnedObject = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedObject == null)
        {
            spawn();
        }
    }

    void spawn()
    {
        float lastNumber = 0;
        float randomNumber = Random.Range(0, 100);
        foreach (ObjectWithProbability spawnObject in spawnObjects)
        {
            float num = 100 * spawnObject.probability;
            if (num + lastNumber >= randomNumber)
            {
                spawnedObject = Instantiate(spawnObject.gameObject);
                break;
            }
            else
            {
                lastNumber = num;
            }
        }
    }
}
