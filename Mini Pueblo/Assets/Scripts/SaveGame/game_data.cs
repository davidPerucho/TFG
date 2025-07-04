using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de almacenar los datos de jugo que se quieren guardar.
/// </summary>
[System.Serializable]
public class GameData
{
    //Variables jugador
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    //Variables tutorial
    public bool tutorial;

    //Variables sistema de puntos
    public List<int> pointsShootingGame;

    //Variables del menu de opciones
    public bool characterVoices;
    public float musicVolume;
    public float sfxVolume;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    public GameData()
    {
        playerPosition = new Vector3(3.62f, 0.78f, -5.53f);
        playerRotation = Quaternion.identity;
        tutorial = true;
        pointsShootingGame = new List<int>();
        characterVoices = true;
        musicVolume = 1f;
        sfxVolume = 1f;
    }
}