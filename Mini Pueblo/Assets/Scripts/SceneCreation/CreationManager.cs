using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Clase encargada de la creación de nuevas escenas.
/// </summary>
public class CreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject yesNoUI;

    [SerializeField]
    GameObject colorBook;

    [SerializeField]
    GameObject abstractStyle;

    [SerializeField]
    GameObject cubist;

    [SerializeField]
    GameObject freePainting;

    [SerializeField]
    GameObject numberPainting;

    private readonly string apiKey = "AIzaSyCt94fTBRR6J-kO3XHo8WkC8aAGKIyqedI";
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
    PaintingStyle paintingStyle; //Estilo que se deasea que tengan las imágenes generadas
    PaintingSceneType paintingSceneType; //Tipo de actividad de pintar

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
                        sceneType = SceneType.PAINTING;
                        characterIndex = p.characterIndex;
                        locationIndex = p.locationIndex;
                        sceneTheme = p.sceneThemeEnglish;
                        paintingStyle = p.paintingStyle;
                        paintingSceneType = p.paintingSceneType;
                    }
                }
                foreach (TableSceneData t in createdScenesList.tableScenes)
                {
                    if (t.sceneName == sceneName)
                    {
                        sceneType = SceneType.TABLE;
                        characterIndex = t.characterIndex;
                        locationIndex = t.locationIndex;
                    }
                }
            }

            //Cargo la interfaz que corresponda según el tipo de escena que se está creando
            if (sceneType == SceneType.PAINTING) {
                paintSceneOptions();
                UIManager.Instance.DisableObject("Pintar");
                UIManager.Instance.DisableObject("JuegoDeMesa");
                UIManager.Instance.DisableObject("TextoSeleccion");

                UIManager.Instance.EnableObject("TextoPrompt");
                UIManager.Instance.EnableObject("InputPrompt");
                UIManager.Instance.EnableObject("Crear");
                UIManager.Instance.SetInputValue("InputPrompt", sceneTheme);

                if (paintingStyle == PaintingStyle.COLORBOOK)
                {
                    colorBook.GetComponent<RawImage>().color = Color.green;
                }
                else if (paintingStyle == PaintingStyle.CUBIST)
                {
                    cubist.GetComponent<RawImage>().color = Color.green;
                }
                else if (paintingStyle == PaintingStyle.ABSTRACT)
                {
                    abstractStyle.GetComponent<RawImage>().color = Color.green;
                }

                if (paintingSceneType == PaintingSceneType.NORMAL)
                {
                    freePainting.GetComponent<RawImage>().color = Color.green;
                }
                else if (paintingSceneType == PaintingSceneType.NUMBERS)
                {
                    numberPainting.GetComponent<RawImage>().color = Color.green;
                }
            }
            else if (sceneType == SceneType.TABLE)
            {
                tableSceneOptions();
                UIManager.Instance.DisableObject("Pintar");
                UIManager.Instance.DisableObject("JuegoDeMesa");
                UIManager.Instance.DisableObject("TextoSeleccion");
            }
        }

        //Añado la funcionalidad a los botones
        UIManager.Instance.AddListenerToButton("returnButton", () => { yesNoUI.SetActive(true); });
        UIManager.Instance.AddListenerToButton("No", () => { SceneManager.LoadScene("MainMenu"); });
        UIManager.Instance.AddListenerToButton("Si", saveScene);
        UIManager.Instance.AddListenerToButton("Pintar", paintSceneOptions);
        UIManager.Instance.AddListenerToButton("JuegoDeMesa", tableSceneOptions);
        UIManager.Instance.AddListenerToButton("Crear", () => { StartCoroutine(createScene()); });
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
                sceneData.paintingStyle = paintingStyle;
                sceneData.paintingSceneType = paintingSceneType;
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
        if (sceneType == SceneType.TABLE)
        {
            try
            {
                TableSceneData sceneData = new TableSceneData();
                sceneData.sceneName = sceneName;
                sceneData.characterIndex = characterIndex;
                sceneData.locationIndex = locationIndex;
                CreatedScenes createdScenesList = new CreatedScenes();
                string json;

                if (File.Exists(creationSavePath))
                {
                    json = File.ReadAllText(creationSavePath);
                    createdScenesList = JsonUtility.FromJson<CreatedScenes>(json);

                    foreach (TableSceneData t in createdScenesList.tableScenes)
                    {
                        if (t.sceneName == sceneName)
                        {
                            createdScenesList.tableScenes.Remove(t);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(creationSavePath));
                }

                createdScenesList.tableScenes.Add(sceneData);
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
            foreach (TableSceneData t in createdScenesList.tableScenes)
            {
                if (t.sceneName == sceneName)
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
        //Añado el tipo de escena
        sceneType = SceneType.PAINTING;

        //Muestro los elementos de UI
        UIManager.Instance.DisableObject("Pintar");
        UIManager.Instance.DisableObject("TextoSeleccion");
        UIManager.Instance.DisableObject("JuegoDeMesa");

        UIManager.Instance.EnableObject("TextoPrompt");
        UIManager.Instance.EnableObject("InputPrompt");
        UIManager.Instance.EnableObject("ColorBook");
        UIManager.Instance.EnableObject("ColorBookText");
        UIManager.Instance.EnableObject("Cubist");
        UIManager.Instance.EnableObject("CubistText");
        UIManager.Instance.EnableObject("Abstract");
        UIManager.Instance.EnableObject("AbstractText");
        UIManager.Instance.EnableObject("TextoEstilo");
        UIManager.Instance.EnableObject("TextoLibre");
        UIManager.Instance.EnableObject("Libre");
        UIManager.Instance.EnableObject("TextoNumeros");
        UIManager.Instance.EnableObject("Numeros");
        UIManager.Instance.EnableObject("Crear");

        //Añado la funcionalidad de los botones
        colorBook.GetComponent<Button>().onClick.AddListener(() =>
        {
            paintingStyle = PaintingStyle.COLORBOOK;
            colorBook.GetComponent<RawImage>().color = Color.green;
            abstractStyle.GetComponent<RawImage>().color = Color.white;
            cubist.GetComponent<RawImage>().color = Color.white;
        });
        abstractStyle.GetComponent<Button>().onClick.AddListener(() =>
        {
            paintingStyle = PaintingStyle.ABSTRACT;
            abstractStyle.GetComponent<RawImage>().color = Color.green;
            colorBook.GetComponent<RawImage>().color = Color.white;
            cubist.GetComponent<RawImage>().color = Color.white;
        });
        cubist.GetComponent<Button>().onClick.AddListener(() =>
        {
            paintingStyle = PaintingStyle.CUBIST;
            cubist.GetComponent<RawImage>().color = Color.green;
            abstractStyle.GetComponent<RawImage>().color = Color.white;
            colorBook.GetComponent<RawImage>().color = Color.white;
        });
        freePainting.GetComponent<Button>().onClick.AddListener(() =>
        {
            paintingSceneType = PaintingSceneType.NORMAL;
            freePainting.GetComponent<RawImage>().color = Color.green;
            numberPainting.GetComponent<RawImage>().color = Color.white;
        });
        numberPainting.GetComponent<Button>().onClick.AddListener(() =>
        {
            paintingSceneType = PaintingSceneType.NUMBERS;
            numberPainting.GetComponent<RawImage>().color = Color.green;
            freePainting.GetComponent<RawImage>().color = Color.white;
        });
    }

    /// <summary>
    /// Muestra la interfaz para crear un juego de mesa.
    /// </summary>
    void tableSceneOptions()
    {
        UIManager.Instance.DisableObject("Pintar");
        UIManager.Instance.DisableObject("TextoSeleccion");
        UIManager.Instance.DisableObject("Pintar");
        UIManager.Instance.DisableObject("JuegoDeMesa");
        sceneType = SceneType.TABLE;
    }

    /// <summary>
    /// Crea la escena y almacena los datos.
    /// </summary>
    IEnumerator createScene()
    {
        string responseText = "Animal"; //Tema por defecto de la escena

        //URL de la API de Google Translate
        string url = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}";

        // Cuerpo de la petición
        string jsonData = $"{{\"q\": \"{UIManager.Instance.GetInputValue("InputPrompt")}\", \"source\": \"es\", \"target\": \"en\"}}";

        //Creo la solicitud HTTP
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        //Verifico la respuesta
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            responseText = request.downloadHandler.text;

            TranslateResponse response = JsonUtility.FromJson<TranslateResponse>(responseText);
            if (response != null && response.data.translations.Length > 0)
            {
                responseText = response.data.translations[0].translatedText;
            }
        }

        //Creo el personaje
        createCharacter(characterIndex, int.Parse(locationIndex), PlayerPrefs.GetString("CharacterPhrase", "Vamos a jugar."), sceneName);

        //Guardo los datos de la escena creada
        if (sceneType == SceneType.PAINTING)
        {
            PaintingSceneData data = new PaintingSceneData();
            data.characterIndex = characterIndex;
            data.locationIndex = locationIndex;
            data.sceneType = sceneType;
            data.sceneName = sceneName;
            data.sceneThemeSpanish = UIManager.Instance.GetInputValue("InputPrompt");
            data.sceneThemeEnglish = responseText;
            data.paintingStyle = paintingStyle;
            data.paintingSceneType = paintingSceneType;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(sceneSavePath, json);
            Debug.Log("Datos guardados en: " + sceneSavePath);
        }
        else if (sceneType == SceneType.TABLE)
        {
            TableSceneData data = new TableSceneData();
            data.characterIndex = characterIndex;
            data.locationIndex = locationIndex;
            data.sceneType = sceneType;
            data.sceneName = sceneName;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(sceneSavePath, json);
            Debug.Log("Datos guardados en: " + sceneSavePath);
        }

        //Guardo el tipo de escena y vuelvo al menu principal
        string typesPath = Path.Combine(Application.persistentDataPath, "Scenes/ScenesTypes.json");
        ScenesTypes scenesTypes = new ScenesTypes();
        string jsonTypes = "";
        if (File.Exists(typesPath))
        {
            jsonTypes = File.ReadAllText(typesPath);
            scenesTypes = JsonUtility.FromJson<ScenesTypes>(jsonTypes);
        }
        SceneTuple tuple = new SceneTuple();
        tuple.name = sceneName;
        tuple.type = sceneType;
        scenesTypes.scenes.Add(tuple);
        jsonTypes = JsonUtility.ToJson(scenesTypes, true);
        File.WriteAllText(typesPath, jsonTypes);

        SceneManager.LoadScene("MainMenu");
    }

    //Clases necesarias para obtener el texto traducido
    [System.Serializable]
    private class TranslateResponse
    {
        public TranslationData data;
    }

    [System.Serializable]
    private class TranslationData
    {
        public Translation[] translations;
    }

    [System.Serializable]
    private class Translation
    {
        public string translatedText;
    }
}