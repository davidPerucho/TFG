using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public GameData()
    {
        playerPosition = new Vector3(3.62f, 0.78f, -5.53f);
        playerRotation = Quaternion.identity;
    }
}