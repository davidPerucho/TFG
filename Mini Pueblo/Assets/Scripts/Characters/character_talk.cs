using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public string characterSex; //Sexo del NPC
    AudioClip characterVoice = null; //Audio con la frase del NPC
    string voicePath; //Ruta hacia el archivo .wav con el audio del personaje
    AudioSource voiceSource; //Fuente de origen de la voz del personaje

    void Start()
    {
        voicePath = Path.Combine(Application.persistentDataPath, "Voices/" + gameObject.name + ".wav");
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
        //Si el audio no esta cargado lo cargamos
        if (characterVoice == null)
        {   
            if (File.Exists(voicePath))
            {
                getVoiceAudioClip();
            }
            else
            {
                Debug.LogWarning("No se ha generado el archivo " + gameObject.name + ".wav");
            }
        }

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
        voiceSource = SoundManager.instance.addSFX(transform, characterVoice, 0.9f);
    }

    /// <summary>
    /// Carga la escena con la que est� relacionado el NPC.
    /// </summary>
    public void loadScene()
    {
        DataPersitence.instance.saveGame();
        SceneManager.LoadScene(sceneName);
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
    private void getVoiceAudioClip()
    {
        //Cargamos los bytes
        byte[] voiceBytes = File.ReadAllBytes(voicePath);

        //Leemos los bytes del archivo
        using (MemoryStream streamMemory = new MemoryStream(voiceBytes))
        using (BinaryReader readerBynary = new BinaryReader(streamMemory))
        {
            //Salta los bytes del encabezado del wav
            readerBynary.ReadBytes(44);

            /*
             * Creamos un array con el tama�o de los datos del audio quitando el encabezado (44 bytes) y teniendo en cuenta que el reader
             * lee los bytes de dos en dos
            */
            float[] voiceFloat = new float[(voiceBytes.Length - 44) / 2];

            //Pasamos los datos a byte a float y los almacenamos en audioData
            for (int i = 0; i < voiceFloat.Length; i++)
                voiceFloat[i] = readerBynary.ReadInt16() / Int16.MaxValue;

            //Creamos el objeto audio clip y le insertamos los datos
            characterVoice = AudioClip.Create(gameObject.name, voiceFloat.Length, 1, 44100, false);
            characterVoice.SetData(voiceFloat, 0);
        }
    }
}
