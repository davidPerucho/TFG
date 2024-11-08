using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
        }

        // Stop further rotation
        if (Quaternion.Angle(transform.rotation, initialRotation) < 0.1f)
        {
            transform.rotation = initialRotation;
            rotateBack = false;
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

        //Rotate towards the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        FindAnyObjectByType<dialog_manager>().showText(characterPhrase, loadScene, cancelTalk);
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

        //Rotates the character towards its normal rotation
        rotateBack = true;
    }
}
