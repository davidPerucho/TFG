using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que se encarga de gestionar los puntos y el número de vidas del juego.
/// </summary>
public class PointsCollider : MonoBehaviour
{
    public bool hit = false; //True si la bala a golpeado la lata
    public static PointsCollider instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton LifeUI.");
            return;
        }

        instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            hit = false;
        }
    }

    /// <summary>
    /// Función que se llama cuando hay una colisión.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (hit == false)
            {
                LifeUI.instance.decreaseLife();
                Destroy(collision.gameObject);

                if (LifeUI.instance.numLifes == 0)
                {
                    DataPersitence.instance.saveGame();
                    SceneManager.LoadScene("Hub");
                }
            }
        }

        if (collision.gameObject.tag == "OnePoint")
        {
            LifeUI.instance.increaseLife();
            Destroy(collision.gameObject);
            FindAnyObjectByType<points_system>().addPoints(1);
            if (FindAnyObjectByType<points_system>().points >= 7)
            {
                DataPersitence.instance.saveGame();
                SceneManager.LoadScene("Hub");
            }
        }
        else if (collision.gameObject.tag == "TwoPoints")
        {
            LifeUI.instance.increaseLife();
            Destroy(collision.gameObject);
            FindAnyObjectByType<points_system>().addPoints(2);
            if (FindAnyObjectByType<points_system>().points >= 7)
            {
                DataPersitence.instance.saveGame();
                SceneManager.LoadScene("Hub");
            }
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
