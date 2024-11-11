using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Esta clase se encarga de almacenar las conversaciones y acciones de los NPC.
/// </summary>
public class CharacterTalk : MonoBehaviour
{
    public string characterPhrase; //Frase del NPC
    public string sceneName; //Nombre de la scena (minijuego/actividad) de la que se encraga el NPC
    Animator characterAnimator; //Animador del NPC
    Transform playerTransform; //Posici�n y rotaci�n del jugador
    Quaternion initialRotation; //Rotaci�n inicial del NPC
    public float rotationSpeed = 2; //Velocidad de rotaci�n del NPC
    bool rotateBack = false; //True si el personaje est� volviendo a la rotaci�n orignal
    bool rotatePlayer = false; //True si el NPC est� rotando en direcci�n al jugador

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

            //Si la rotaci�n est� practicamente terminada se finaliza
            if (Quaternion.Angle(transform.rotation, initialRotation) < 0.1f)
            {
                transform.rotation = initialRotation;
                rotateBack = false;
            }
        }

        if (rotatePlayer == true)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            //Si la rotaci�n est� practicamente terminada se finaliza
            if (Quaternion.Angle(transform.rotation, rotation) < 0.1f)
            {
                transform.rotation = rotation;
                rotatePlayer = false;
            }
        }
    }

    /// <summary>
    /// Muetra en pantalla, con retraso, la frase del NPC.
    /// </summary>
    public void talk()
    {
        //Stop previus rotation
        rotateBack = false;

        //Inicia la animaci�n de Habla del NPC
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("talking", true);
        }

        //Rotar hacia el jugador
        rotatePlayer = true;

        //Retrasa la aparici�n del texto
        StartCoroutine(FindAnyObjectByType<dialog_manager>().showUIWithDelay(characterPhrase, loadScene, cancelTalk));
    }

    /// <summary>
    /// Carga la escena con la que est� relacionado el NPC.
    /// </summary>
    public void loadScene()
    {
        SceneManager.LoadScene(sceneName);
        DataPersitence.instance.saveGame();
    }

    /// <summary>
    /// Finaliza la conversaci�n del NPC.
    /// </summary>
    public void cancelTalk()
    {
        //Finaliza la animaci�n de habla
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("talking", false);
        }

        //Cancela la rotaci�n hacia el jugador
        rotatePlayer = false;

        //Rotates the character towards its normal rotation
        rotateBack = true;
    }
}
