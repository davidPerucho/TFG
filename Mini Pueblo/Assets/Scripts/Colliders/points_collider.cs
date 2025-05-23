using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que se encarga de gestionar los puntos y el número de vidas del juego.
/// </summary>
public class PointsCollider : MonoBehaviour
{
    [HideInInspector]
    public bool hit = false;
    public static PointsCollider instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton PointsCollider.");
            return;
        }

        instance = this;
    }

    /// <summary>
    /// Función que se llama cuando hay una colisión.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            //Compruebo si la lata ha sido golpeada para restar o no vidas
            if (hit == false)
            {
                LifeUI.instance.decreaseLife();

                if (LifeUI.instance.numLifes == 0)
                {
                    HighScore.instance.showHighScores(PointsSystem.instance.points);
                }
            }
            else
            {
                hit = false;
            }
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "OnePoint")
        {
            //LifeUI.instance.increaseLife();
            Destroy(collision.gameObject);
            PointsSystem.instance.addPoints(1);
        }
        else if (collision.gameObject.tag == "TwoPoints")
        {
            //LifeUI.instance.increaseLife();
            Destroy(collision.gameObject);
            PointsSystem.instance.addPoints(2);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
