using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase encargada de generar objetos al hacer click.
/// </summary>
public class click_spawner : MonoBehaviour
{
    public GameObject spawnObject; //El objeto que se quiere crear
    public bool onReleased; //True si se quiere crear el objeto al soltar el click
    public bool onPressed; //True si se quiere crear el objeto al presionar el click
    public bool continuousSpawn; //True si se quiere generar objetos de manera continua mientras se esté presionando
    public float timeToSpawn; //El tiempo de generación cuando continuousSpawn es true
    bool pressed = false; //True si se está haciendo click
    float remainingTime = 0; //El tiempo que falta para generar el proximo objeto cuando continuousSpawn es true

    public float timeBetweenSpawns = 0; //Tiempo mínimo que tiene que pasar entre un spawn y el siguiente

    float lastSpawn = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pressed = true;
            if (onPressed == true && continuousSpawn == false)
            {
                if ((lastSpawn == 0 || (Time.time - lastSpawn) >= timeBetweenSpawns) && Time.timeScale != 0)
                {
                    Instantiate(spawnObject, transform.position, Quaternion.identity);
                    lastSpawn = Time.time;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
            if (onReleased == true)
            {
                if ((lastSpawn == 0 || (Time.time - lastSpawn) >= timeBetweenSpawns) && Time.timeScale != 0)
                {
                    Instantiate(spawnObject, transform.position, Quaternion.identity);
                    lastSpawn = Time.time;
                }
            }
        }

        if (pressed == true && onPressed == true && continuousSpawn == true)
        {
            if (remainingTime <= 0 && Time.timeScale != 0)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
                remainingTime = timeToSpawn;
            }
        }

        remainingTime -= Time.deltaTime;
    }
}
