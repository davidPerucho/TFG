using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Clase encargada de la creación de nuevas escenas.
/// </summary>
public class CreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject yesNoUI;

    string sceneName; //Nombre de la escena que se está creando
    string sceneSavePath; //Dirección donde guardar la escena
    string creationSavePath; //Dirección donde se guardan los datos de creación de la escena
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
    SceneType sceneType; //Tipo de escena
    string characterIndex; //Indice del personaje
    string locationIndex; //Localización del personaje

    void Awake()
    {
        //Creo la nueva escena con el nombre insertado en el menu de creación
        sceneName = PlayerPrefs.GetString("SceneName", "Minijuego");
        //createdScene = SceneManager.CreateScene(sceneName);

        //Creo la ruta de guardado para la escena
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Scenes/"))) //Comprobar que el directorio existe
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Scenes/"));
            Debug.Log($"El directorio no existía, pero se ha creado");
        }
        sceneSavePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneName + ".json");
        creationSavePath = Path.Combine(Application.persistentDataPath, "Managers/" + "ScenesOnCreation.json");

        characterIndex = PlayerPrefs.GetString("SelectedNPC", "1");
        locationIndex = PlayerPrefs.GetString("SelectedLocation", "1");
    }

    void Start()
    {
        //cargo los datos e inicializo los elementos de la UI
        if (jsonExists() == true)
        {
            string sceneTheme = "";

            if (File.Exists(creationSavePath))
            {
                string json = File.ReadAllText(creationSavePath);
                CreatedScenes createdScenesList = JsonUtility.FromJson<CreatedScenes>(json);

                foreach (PaintingSceneData p in createdScenesList.paintingScenes)
                {
                    if (p.sceneName == sceneName)
                    {
                        characterIndex = p.characterIndex;
                        locationIndex = p.locationIndex;
                        sceneTheme = p.sceneThemeEnglish;
                    }
                }
            }

            sceneType = SceneType.PAINTING;

            UIManager.Instance.DisableObject("Pintar");
            UIManager.Instance.DisableObject("TextoSeleccion");

            UIManager.Instance.EnableObject("TextoPrompt");
            UIManager.Instance.EnableObject("InputPrompt");
            UIManager.Instance.EnableObject("Crear");
            UIManager.Instance.SetInputValue("InputPrompt", sceneTheme);
        }

        UIManager.Instance.AddListenerToButton("returnButton", () => { yesNoUI.SetActive(true); });
        UIManager.Instance.AddListenerToButton("No", () => { SceneManager.LoadScene("MainMenu"); });
        UIManager.Instance.AddListenerToButton("Si", saveScene);
        UIManager.Instance.AddListenerToButton("Pintar", paintSceneOptions);
        UIManager.Instance.AddListenerToButton("Crear", createScene);
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
        if (sceneType == SceneType.PAINTING)
        {
            try
            {
                PaintingSceneData sceneData = new PaintingSceneData();
                sceneData.sceneName = sceneName;
                sceneData.sceneThemeEnglish = UIManager.Instance.GetInputValue("InputPrompt");
                sceneData.characterIndex = characterIndex;
                sceneData.locationIndex = locationIndex;
                CreatedScenes createdScenesList = new CreatedScenes();
                string json;

                if (File.Exists(creationSavePath))
                {
                    json = File.ReadAllText(creationSavePath);
                    createdScenesList = JsonUtility.FromJson<CreatedScenes>(json);

                    foreach (PaintingSceneData p in createdScenesList.paintingScenes)
                    {
                        if (p.sceneName == sceneName)
                        {
                            createdScenesList.paintingScenes.Remove(p);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(creationSavePath));
                }

                createdScenesList.paintingScenes.Add(sceneData);
                json = JsonUtility.ToJson(createdScenesList, true);
                File.WriteAllText(creationSavePath, json);
                Debug.Log("Datos guardados en: " + creationSavePath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Devuelve true si ya existe un json de la escena.
    /// </summary>
    /// <returns>True si existe el json de la escena false si no.</returns>
    bool jsonExists()
    {
        if (File.Exists(creationSavePath))
        {
            string json = File.ReadAllText(creationSavePath);
            CreatedScenes createdScenesList = JsonUtility.FromJson<CreatedScenes>(json);

            foreach (PaintingSceneData p in createdScenesList.paintingScenes)
            {
                if (p.sceneName == sceneName)
                {
                    return true;
                }
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

    /// <summary>
    /// Muestra la interfaz para crear una actividad de pintura.
    /// </summary>
    void paintSceneOptions()
    {
        sceneType = SceneType.PAINTING;

        UIManager.Instance.DisableObject("Pintar");
        UIManager.Instance.DisableObject("TextoSeleccion");

        UIManager.Instance.EnableObject("TextoPrompt");
        UIManager.Instance.EnableObject("InputPrompt");
        UIManager.Instance.EnableObject("Crear");
    }

    /// <summary>
    /// Crea la escena y almacena los datos.
    /// </summary>
    void createScene()
    {
        createCharacter(characterIndex, int.Parse(locationIndex), PlayerPrefs.GetString("CharacterPhrase", "Vamos a jugar."), sceneName);

        if (sceneType == SceneType.PAINTING)
        {
            PaintingSceneData data = new PaintingSceneData();
            data.characterIndex = characterIndex;
            data.locationIndex = locationIndex;
            data.sceneType = sceneType;
            data.sceneName = sceneName;
            data.sceneThemeEnglish = UIManager.Instance.GetInputValue("InputPrompt");

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(sceneSavePath, json);
            Debug.Log("Datos guardados en: " + sceneSavePath);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
