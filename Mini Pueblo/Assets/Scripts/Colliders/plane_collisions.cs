using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class plane_collisions : MonoBehaviour
{
    int tutorialIterations = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlaneBlock")
        {
            Destroy(gameObject);

            if (FindAnyObjectByType<points_system>().points >= 20)
            {
                DataPersitence.instance.saveGame();
                SceneManager.LoadScene("Hub");
            }
            else
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        }
        if (collision.gameObject.tag == "PlaneBlockTutorial")
        {
            FindAnyObjectByType<block_spawnUP_tutorial>().stopBlocks();
            FindAnyObjectByType<block_spawnDown_tutorial>().stopBlocks();
            Destroy(gameObject);

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlaneBlockTutorial")
        {
            FindAnyObjectByType<text_tutorial>().nextText();
            FindAnyObjectByType<hand_animation>().change_animation();

            tutorialIterations++;
            if (tutorialIterations >= 4)
            {
                DataPersitence.instance.saveGame();
                SceneManager.LoadScene("PaperPlane");
            }
        }
    }
}
