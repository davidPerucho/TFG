using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance { get; private set; } //Singleton
    public float speedMultiplier = 1f;
    public float maxSpeedMultiplier = 2.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void increaseSpeed(float speed)
    {
        speedMultiplier += speed;
    }
}
