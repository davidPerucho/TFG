using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject yesNoUI;

    [SerializeField]
    GameObject[] characterPrefabs;

    Scene createdScene; //Escena creada
    string sceneSavePath; //Dirección donde guardar la escena
    Vector3[] locationCoordinates = { new Vector3(-4.985f, -0.11f, 9.2435f), new Vector3(18.265f, -0.11f, 11.573f), new Vector3(27.105f, 0.49f, 5.383f), new Vector3(-12.32f, -0.11f, 4.47f), new Vector3(0.915f, 0.28f, -9.537f), new Vector3(12.705f, 0.47f, -5.847f), new Vector3(27.315f, 0.29f, -12.407f) }; //Coordenadas de las localizaciones
    Dictionary<string, float> characterYCoordinate = new Dictionary<string, float> //Almacenamiento de las coordenadas Y de los personajes
    {
        { "1", 0f },
        { "2", 0.05f }, //Gnomo
        { "3", 0.11f }, //Hombre normal
        { "4", 0f },
        { "5", 0f },
        { "6", 0f },
        { "7", 0f }
    };

    void Awake()
    {
        //Creo la nueva escena con el nombre insertado en el menu de creación
        string sceneName = PlayerPrefs.GetString("SceneName", "Minijuego");
        createdScene = SceneManager.CreateScene(sceneName);
        addCamera();

        //Creo la ruta de guardado para la escena
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Scenes/"))) //Comprobar que el directorio existe
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Scenes/"));
            Debug.Log($"El directorio no existía, pero se ha creado");
        }
        sceneSavePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneName + ".json");

        string characterIndex = PlayerPrefs.GetString("SelectedNPC", "1");
        string locationIndex = PlayerPrefs.GetString("SelectedLocation", "1");

        if (jsonExists(sceneName) == false)
        {
            createCharacter(characterIndex, int.Parse(locationIndex), PlayerPrefs.GetString("CharacterPhrase", "Vamos a jugar."), characterPrefabs[int.Parse(characterIndex) - 1]);
        }
    }

    void Start()
    {
        //Agrego funciones de UI
        UIManager.Instance.AddListenerToButton("returnButton", () => { yesNoUI.SetActive(true); });
        UIManager.Instance.AddListenerToButton("No", () => { SceneManager.LoadScene("MainMenu"); });
        UIManager.Instance.AddListenerToButton("Si", saveScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Guarda un json que representa la escena creada.
    /// </summary>
    void saveScene()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(sceneSavePath));
            string dataToStore = JsonUtility.ToJson(createdScene, true);

            using (FileStream stream = new FileStream(sceneSavePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            SceneManager.LoadScene("MainMenu");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Añade una cámara a la escena.
    /// </summary>
    void addCamera()
    {
        //Creo la cámara
        GameObject cameraObject = new GameObject("Camera");
        Camera cameraComponent = cameraObject.AddComponent<Camera>();

        cameraComponent.clearFlags = CameraClearFlags.Skybox; //Configuro la vista
        cameraComponent.backgroundColor = Color.blue; //Configuro el color de fondo
        cameraComponent.orthographic = false;
        cameraObject.tag = "MainCamera"; //Añado un tag para que Unity la reconozca como la cámara principal

        SceneManager.MoveGameObjectToScene(cameraObject, createdScene); //Añado la cámara a la escena
    }

    /// <summary>
    /// Devuelve true si ya existe un json de una escena con el nombre pasado por argumento.
    /// </summary>
    /// <param name="sceneName">Nombre de la escena que se quiere crear.</param>
    /// <returns>True si existe el json de la escena false si no</returns>
    bool jsonExists(string sceneName)
    {
        string scenesPath = Path.Combine(Application.persistentDataPath, "Scenes/");
        if (!Directory.Exists(scenesPath))
        {
            Directory.CreateDirectory(scenesPath);
            Debug.Log($"El directorio no existía, pero se ha creado: {scenesPath}");

            return false;
        }

        string[] sceneFiles = Directory.GetFiles(scenesPath, "*.json", SearchOption.AllDirectories);

        foreach (string scene in sceneFiles)
        {
            if (Path.GetFileNameWithoutExtension(scene) == sceneName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Crea el personaje seleccionado en el menú dentro del entorno 3D.
    /// </summary>
    /// <param name="cIndex">Index del personaje.</param>
    /// <param name="lIndex">Index de la localización.</param>
    /// <param name="characterPhrase">Frase del personaje.</param>
    /// <param name="characterPrefab">Instancia del personaje.</param>
    void createCharacter(string cIndex, int lIndex, string characterPhrase, GameObject characterPrefab)
    {
        //Obtengo la posición del personaje
        Vector3 characterPosition = new Vector3(locationCoordinates[lIndex].x, locationCoordinates[lIndex].y + characterYCoordinate[cIndex], locationCoordinates[lIndex].z);

        //Creo el personaje en el entorno 3D
        Debug.Log(characterPhrase + " " + PlayerPrefs.GetString("SceneName", "Minijuego"));

        //Asigno la frase y la escena al personaje
    }
}
