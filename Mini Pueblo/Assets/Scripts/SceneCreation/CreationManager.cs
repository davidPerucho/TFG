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

    Scene createdScene; //Escena creada
    string sceneSavePath; //Dirección donde guardar la escena
    Vector3[] locationCoordinates = { new Vector3(-4.985f, -0.11f, 9.2435f), new Vector3(18.265f, -0.11f, 11.573f), new Vector3(27.105f, 0.49f, 5.383f), new Vector3(-12.32f, -0.11f, 4.47f), new Vector3(0.915f, 0.28f, -9.537f), new Vector3(12.705f, 0.47f, -5.847f), new Vector3(27.315f, 0.29f, -12.407f) }; //Coordenadas de las localizaciones
    float[] characterYRotation = { 180f, 180f, 180f, 137f, 137f, 180f, 275f }; //Rotación de los personajes en el eje Y
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
    Dictionary<string, string> characterSex = new Dictionary<string, string> //Almacenamiento de el sexo de los personajes
    {
        { "1", "H" },
        { "2", "H" }, //Gnomo
        { "3", "H" }, //Hombre normal
        { "4", "H" },
        { "5", "H" },
        { "6", "H" },
        { "7", "H" }
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
            createCharacter(characterIndex, int.Parse(locationIndex), PlayerPrefs.GetString("CharacterPhrase", "Vamos a jugar."), sceneName);
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
    /// Guarda los datos del personaje seleccionado en el menú para ser creado posterioemente dentro del entorno 3D.
    /// </summary>
    /// <param name="cIndex">Index del personaje.</param>
    /// <param name="lIndex">Index de la localización.</param>
    /// <param name="characterPhrase">Frase del personaje.</param>
    /// <param name="sceneName">Nombre de la escena a la que llama el personaje.</param>
    void createCharacter(string cIndex, int lIndex, string characterPhrase, string sceneName)
    {
        //Obtengo la posición del personaje
        Vector3 characterPosition = new Vector3(locationCoordinates[lIndex - 1].x, locationCoordinates[lIndex - 1].y + characterYCoordinate[cIndex], locationCoordinates[lIndex - 1].z);

        //Guardo los datos del personaje en un json para inicializarlo más tarde
        CharacterData character = new CharacterData
        {
            coordinates = characterPosition,
            phrase = characterPhrase,
            cIndex = int.Parse(cIndex) - 1,
            lIndex = lIndex,
            sex = characterSex[cIndex],
            yRotation = characterYRotation[lIndex - 1],
            scene = sceneName
        };

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Managers/"))) //Comprobar que el directorio existe
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Managers/"));
            Debug.Log($"El directorio no existía, pero se ha creado");
        }

        string filePath = Path.Combine(Application.persistentDataPath, "Managers/" + "NewCharacters.json");
        CharacterList characterList = new CharacterList();
        string json = "";
        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
            characterList = JsonUtility.FromJson<CharacterList>(json);

            foreach (CharacterData c in characterList.characters)
            {
                if (c.lIndex == lIndex)
                {
                    characterList.characters.Remove(c);
                    break;
                }
            }

            characterList.characters.Add(character);
        }
        else
        {
            characterList.characters.Add(character);
        }
        json = JsonUtility.ToJson(characterList, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Datos guardados en: " + filePath);
    }
}
