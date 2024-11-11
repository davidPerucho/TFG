using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de almacenar los datos de jugo que se quieren guardar.
/// </summary>
[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    public GameData()
    {
        playerPosition = new Vector3(3.62f, 0.78f, -5.53f);
        playerRotation = Quaternion.identity;
    }
}