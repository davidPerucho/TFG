using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
    public string characterSex; //Sexo del NPC
    AudioClip characterVoice = null; //Audio con la frase del NPC
    string voicePath; //Ruta hacia el archivo .wav con el audio del personaje
    static readonly List<string> sceneList = new List<string> { "DynamicScene", "Hub", "MainMenu", "MandalaPainting", "PaperPlane", "SceneCreation", "ShootingGame", "TutorialPaperPlane" }; //Lista de nombres de escenas ya existentes

    void Start()
    {
        voicePath = Path.Combine(Application.dataPath, "Voices/" + gameObject.name + ".wav");
        initialRotation = transform.rotation;
        characterAnimator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        //Si el audio no esta cargado lo cargamos
        if (characterVoice == null)
        {
            if (File.Exists(voicePath))
            {
                StartCoroutine(getVoiceAudioClip());
            }
            else
            {
                Debug.LogWarning("No se ha generado el archivo " + gameObject.name + ".wav");
            }
        }
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
        //Baja el volumen de la m�sica de fondo
        SoundManager.instance.volumeMusicDown();

        //Para la rotaci�n previa
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

        //Lanza el audio con la frase del personaje
        SoundManager.instance.addSFX(transform, characterVoice, 0.9f);
    }

    /// <summary>
    /// Carga la escena con la que est� relacionado el NPC.
    /// </summary>
    public void loadScene()
    {
        if (sceneList.Contains(sceneName) == false)
        {
            PlayerPrefs.SetString("SceneToLoad", sceneName);
            PlayerPrefs.Save();

            DataPersitence.instance.saveGame();
            SceneManager.LoadScene("DynamicScene");
        }
        else
        {
            DataPersitence.instance.saveGame();
            SceneManager.LoadScene(sceneName);
        }
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

        //Vuelve a subir el volumen de la m�sica a lo normal
        SoundManager.instance.volumeMusicUp();

        //Cancela la rotaci�n hacia el jugador
        rotatePlayer = false;

        //Rota al NPC a su rotaci�n normal
        rotateBack = true;
    }

    /// <summary>
    /// Crea un objeto AudioClip a partir del wav almacenado en la ruta voicePath.
    /// </summary>
    IEnumerator getVoiceAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + voicePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                characterVoice = DownloadHandlerAudioClip.GetContent(www);
                characterVoice.LoadAudioData();
            }
            else
            {
                Debug.LogError("Error cargando audio: " + www.error);
            }
        }
    }
}
