using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que se encarga de gestionar las colisiones de las latas.
/// </summary>
public class CanCollider : MonoBehaviour
{

    void Start()
    {
        PointsCollider.instance.hit = false;
    }

    /// <summary>
    /// Funci�n que se llama cuando hay una colisi�n.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            PointsCollider.instance.hit = true;
        }
    }
}
