using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterTalk : MonoBehaviour
{
    public string characterPhrase;
    public string sceneName;
    public float distanceToTalk = 0.7f;
    Animator characterAnimator;
    Transform playerTransform;

    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    void Update()
    {
        if (characterAnimator != null)
        {
            if(distance(playerTransform.position, transform.position) <= distanceToTalk)
            {
                characterAnimator.SetBool("talking", true);
            }
            else {
                characterAnimator.SetBool("talking", false);
            }
        }
    }

    //Asings the character phrase to the text on the dialog manager
    public void talk()
    {
        FindAnyObjectByType<dialog_manager>().showText(characterPhrase, loadScene);
    }

    //Loads the scene that has the name sceneName
    public void loadScene()
    {
        SceneManager.LoadScene(sceneName);
        DataPersitence.instance.saveGame();
    }

    float distance(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2) + Mathf.Pow(pos2.y - pos1.y, 2));
    }
}
