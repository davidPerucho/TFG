using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

/// <summary>
/// Esta clase se encarga de almacenar las conversaciones y acciones de los NPC.
/// </summary>
public class CharacterTalk : MonoBehaviour, IDataPersistence
{
    public string characterPhrase; //Frase del NPC
    public string sceneName; //Nombre de la scena (minijuego/actividad) de la que se encraga el NPC
    Animator characterAnimator; //Animador del NPC
    Transform playerTransform; //Posición y rotación del jugador
    Quaternion initialRotation; //Rotación inicial del NPC
    public float rotationSpeed = 2; //Velocidad de rotación del NPC
    bool rotateBack = false; //True si el personaje está volviendo a la rotación orignal
    bool rotatePlayer = false; //True si el NPC está rotando en dirección al jugador
    public string characterSex; //Sexo del NPC
    AudioClip characterVoice = null; //Audio con la frase del NPC
    string voicePath; //Ruta hacia el archivo .wav con el audio del personaje
    static readonly List<string> sceneList = new List<string> { "DynamicScene", "Hub", "MainMenu", "MandalaPainting", "PaperPlane", "SceneCreation", "ShootingGame", "TutorialPaperPlane" }; //Lista de nombres de escenas ya existentes
    bool ttsEnabled = true; //True si están abilitadas las voces de los personajes

    void Start()
    {
        voicePath = Path.Combine(Application.persistentDataPath, "Voices/" + sceneName + ".wav");
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
                Debug.LogWarning("No se ha generado el archivo " + voicePath);
            }
        }
    }

    void Update()
    {
        if (rotateBack == true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * rotationSpeed);

            //Si la rotación está practicamente terminada se finaliza
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

            //Si la rotación está practicamente terminada se finaliza
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
        //Baja el volumen de la música de fondo
        SoundManager.instance.volumeMusicDown();

        //Para la rotación previa
        rotateBack = false;

        //Inicia la animación de Habla del NPC
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("talking", true);
        }

        //Rotar hacia el jugador
        rotatePlayer = true;

        //Retrasa la aparición del texto
        StartCoroutine(FindAnyObjectByType<dialog_manager>().showUIWithDelay(characterPhrase, loadScene, cancelTalk));

        //Lanza el audio con la frase del personaje
        if (characterVoice != null && ttsEnabled == true)
        {
            SoundManager.instance.addSFX(transform, characterVoice, 0.9f);
        }
    }

    /// <summary>
    /// Carga la escena con la que está relacionado el NPC.
    /// </summary>
    public void loadScene()
    {
        if (sceneList.Contains(sceneName) == false)
        {
            PlayerPrefs.SetString("SceneToLoad", sceneName);
            PlayerPrefs.Save();
            
            string typesPath = Path.Combine(Application.persistentDataPath, "Scenes/ScenesTypes.json");
            string jsonTypes = File.ReadAllText(typesPath);
            ScenesTypes scenesTypes = JsonUtility.FromJson<ScenesTypes>(jsonTypes);

            foreach (SceneTuple scene in scenesTypes.scenes)
            {
                if (scene.name == sceneName)
                {
                    if (scene.type == SceneType.PAINTING)
                    {
                        DataPersitence.instance.saveGame();
                        SceneManager.LoadScene("NormalPainting");
                    }
                    else if (scene.type == SceneType.TABLE)
                    {
                        DataPersitence.instance.saveGame();
                        SceneManager.LoadScene("TableGame");
                    }
                    else
                    {
                       loadExternalScene();
                    }
                }
            }
        }
        else
        {
            if (sceneName == "MandalaPainting")
            {
                PlayerPrefs.SetString("SceneToLoad", "");
                PlayerPrefs.Save();
            }
            DataPersitence.instance.saveGame();
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Carga una escena externa al proyecto.
    /// </summary>
    void loadExternalScene()
    {
        string externalCatalogPath = "";

        //Obtengo los datos de la escena tanto el catálogo como los ajustes
        if (Application.platform == RuntimePlatform.Android)
        {
            externalCatalogPath = System.IO.Path.Combine(Application.persistentDataPath, $"{sceneName}/Android/StandaloneWindows64/catalog_1.0.json");
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            externalCatalogPath = System.IO.Path.Combine(Application.persistentDataPath, $"{sceneName}/Windows/StandaloneWindows64/catalog_1.0.json");
        }

        //Cargo la escena
        Addressables.LoadContentCatalogAsync(externalCatalogPath).Completed += (catalogHandle) =>
        {
            if (catalogHandle.Status == AsyncOperationStatus.Succeeded)
            {
                //Cargo los prefabs que usa la escena
                foreach (var key in Addressables.ResourceLocators.SelectMany(l => l.Keys).Distinct())
                {
                    if (key is string keyStr && keyStr.EndsWith(".prefab"))
                    {
                        Addressables.LoadAssetAsync<GameObject>(keyStr).Completed += handle =>
                        {
                            if (handle.Status == AsyncOperationStatus.Succeeded)
                            {
                                GameObject prefab = handle.Result;
                                string prefabName = keyStr.Replace(".prefab", "").Split("/").Last();

                                PrefabDictionary.instance.loadedPrefabs.Add(prefabName, prefab);
                                Debug.Log($"Prefab '{prefabName}' cargado y agregado.");
                            }
                            else
                            {
                                Debug.LogError($"Error al cargar el prefab con clave: {keyStr}");
                            }
                        };
                    }
                }

                //Guardo datos y cargo la nueva escena
                DataPersitence.instance.saveGame();
                Addressables.LoadSceneAsync($"Assets/Scenes/{sceneName}.unity", LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene("Hub");
                return;
            }
        };

        //PlayerPrefs.SetString("PrefabName", sceneName);
        //PlayerPrefs.Save();
        //DataPersitence.instance.saveGame();
        //SceneManager.LoadScene("DynamicScene");
    }

    /// <summary>
    /// Finaliza la conversación del NPC.
    /// </summary>
    public void cancelTalk()
    {
        //Finaliza la animación de habla
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("talking", false);
        }

        //Vuelve a subir el volumen de la música a lo normal
        SoundManager.instance.volumeMusicUp();

        //Cancela la rotación hacia el jugador
        rotatePlayer = false;

        //Rota al NPC a su rotación normal
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

    /// <summary>
    /// Obtego el dato guardado del menu de opciones.
    /// </summary>
    /// <param name="data">Datos del juego.</param>
    public void loadData(GameData data)
    {
        ttsEnabled = data.characterVoices;
    }

    public void saveData(ref GameData data){}
}
