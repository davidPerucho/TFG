using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class points_collider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "OnePoint")
        {
            Destroy(collision.gameObject);
            FindAnyObjectByType<points_system>().addPoints(1);
            if (FindAnyObjectByType<points_system>().points >= 7)
            {
                SceneManager.LoadScene("Hub");
            }
        }
        else if (collision.gameObject.tag == "TwoPoints")
        {
            Destroy(collision.gameObject);
            FindAnyObjectByType<points_system>().addPoints(2);
            if (FindAnyObjectByType<points_system>().points >= 7)
            {
                SceneManager.LoadScene("Hub");
            }
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
