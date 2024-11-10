using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterTalk : MonoBehaviour
{
    public string characterPhrase;
    public string sceneName;
    Animator characterAnimator;
    Transform playerTransform;
    Quaternion initialRotation;
    public float rotationSpeed = 2;
    bool rotateBack = false;
    bool rotatePlayer = false;

    void Start()
    {
        initialRotation = transform.rotation;
        characterAnimator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    void Update()
    {
        if (rotateBack == true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * rotationSpeed);

            // Stop further rotation
            if (Quaternion.Angle(transform.rotation, initialRotation) < 0.1f)
            {
                transform.rotation = initialRotation;
                rotateBack = false;
            }
        }

        if (rotatePlayer == true)
        {
            //Rotate towards the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            // Stop further rotation
            if (Quaternion.Angle(transform.rotation, rotation) < 0.1f)
            {
                transform.rotation = rotation;
                rotatePlayer = false;
            }
        }
    }

    //Asings the character phrase to the text on the dialog manager
    public void talk()
    {
        //Stop previus rotation
        rotateBack = false;

        //Start talking animation
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("talking", true);
        }

        //Rotar hacia el jugador
        rotatePlayer = true;

        StartCoroutine(FindAnyObjectByType<dialog_manager>().showUIWithDelay(characterPhrase, loadScene, cancelTalk));
    }

    //Loads the scene that has the name sceneName
    public void loadScene()
    {
        SceneManager.LoadScene(sceneName);
        DataPersitence.instance.saveGame();
    }

    //Ends the talk with the player
    public void cancelTalk()
    {
        //Cancel talking animation
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("talking", false);
        }

        //Cancela la rotación hacia el jugador
        rotatePlayer = false;

        //Rotates the character towards its normal rotation
        rotateBack = true;
    }
}
