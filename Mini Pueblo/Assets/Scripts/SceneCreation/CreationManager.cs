using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
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

    [SerializeField]
    GameObject playerItemUI;

    [SerializeField]
    Transform playerContentPosition; //Posición en la que mostrar los datos del jugador

    [SerializeField]
    GameObject boxItemUI;

    [SerializeField]
    Transform boxesContentPosition; //Posición en la que mostrar las casillas al crearlas

    [SerializeField]
    GameObject scrollPlayers;

    [SerializeField]
    GameObject scrollBoxes;

    [SerializeField]
    Image diceImage;

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
    int numPlayers = 0; //Numero de jugadores en caso de que la escena creada sea un juego de mesa
    int numTokens = 1; //Numero de fichas en caso de que la escena creada sea un juego de mesa
    int numBoxes = 0; //Numero de casillas en caso de que la escena creada sea un juego de mesa
    bool dice = true; //True si el juego de mesa creado tiene dado
    List<TablePlayerData> players; //Datos de los jugadores
    List<GameObject> playersUI; //Elementos UI de los jugadores
    List<TableBoxData> boxes; //Datos de los jugadores
    List<GameObject> boxesUI; //Elementos UI de los jugadores

    void Awake()
    {
        //Creo la nueva escena con el nombre insertado en el menu de creación
        sceneName = PlayerPrefs.GetString("SceneName", "Minijuego");

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

        players = new List<TablePlayerData>();
        playersUI = new List<GameObject>();
        boxes = new List<TableBoxData>();
        boxesUI = new List<GameObject>();
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
                        numPlayers = t.numPlayers;
                        numTokens = t.numTokens;
                    }
                }
            }

            //Cargo la interfaz que corresponda según el tipo de escena que se está creando
            if (sceneType == SceneType.PAINTING) {
                paintSceneOptions();
            }
            else if (sceneType == SceneType.TABLE)
            {
                tableSceneOptions();
            }
        }

        //Añado la funcionalidad a los botones
        UIManager.Instance.AddListenerToButton("returnButton", () => { yesNoUI.SetActive(true); });
        UIManager.Instance.AddListenerToButton("No", () => { SceneManager.LoadScene("MainMenu"); });
        UIManager.Instance.AddListenerToButton("Si", saveScene);
        UIManager.Instance.AddListenerToButton("Pintar", paintSceneOptions);
        UIManager.Instance.AddListenerToButton("JuegoDeMesa", tableSceneOptions);
        UIManager.Instance.AddListenerToButton("Crear", () => { StartCoroutine(createScene()); });
        UIManager.Instance.AddListenerToButton("VolverTablero", () => {
            UIManager.Instance.DisableObject("NumJugadoresText");
            UIManager.Instance.DisableObject("NumJugadores");
            UIManager.Instance.DisableObject("AddJugadores");
            UIManager.Instance.DisableObject("RemoveJugadores");
            UIManager.Instance.DisableObject("NumFichasText");
            UIManager.Instance.DisableObject("NumFichas");
            UIManager.Instance.DisableObject("AddFichas");
            UIManager.Instance.DisableObject("RemoveFichas");
            UIManager.Instance.DisableObject("VolverTablero");
            scrollPlayers.gameObject.SetActive(false);

            UIManager.Instance.EnableObject("Jugadores");
            UIManager.Instance.EnableObject("Tablero");
        });
        UIManager.Instance.AddListenerToButton("VolverCasillas", () => {
            UIManager.Instance.DisableObject("NumCasillasText");
            UIManager.Instance.DisableObject("NumCasillas");
            UIManager.Instance.DisableObject("AddCasillas");
            UIManager.Instance.DisableObject("RemoveCasillas");
            UIManager.Instance.DisableObject("VolverCasillas");
            UIManager.Instance.DisableObject("Dado");
            scrollBoxes.gameObject.SetActive(false);

            UIManager.Instance.EnableObject("Jugadores");
            UIManager.Instance.EnableObject("Tablero");
        });
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

        //Posibles datos guardados
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
        //Cambio el tipo de escenas
        sceneType = SceneType.TABLE;

        //Muestro los elementos de UI
        UIManager.Instance.DisableObject("Pintar");
        UIManager.Instance.DisableObject("TextoSeleccion");
        UIManager.Instance.DisableObject("Pintar");
        UIManager.Instance.DisableObject("JuegoDeMesa");

        UIManager.Instance.EnableObject("Jugadores");
        UIManager.Instance.EnableObject("Tablero");

        //Añado la funcionalidad de los botones
        UIManager.Instance.AddListenerToButton("Jugadores", tablePlayerSceneOptions);
        UIManager.Instance.AddListenerToButton("Tablero", tableBoardSceneOptions);
    }

    /// <summary>
    /// Muestra la interfaz para editar el tablero.
    /// </summary>
    void tableBoardSceneOptions()
    {
        //Muestro los elementos de UI
        UIManager.Instance.DisableObject("Jugadores");
        UIManager.Instance.DisableObject("Tablero");

        UIManager.Instance.EnableObject("NumCasillasText");
        UIManager.Instance.EnableObject("NumCasillas");
        UIManager.Instance.EnableObject("AddCasillas");
        UIManager.Instance.EnableObject("RemoveCasillas");
        UIManager.Instance.EnableObject("VolverCasillas");
        UIManager.Instance.EnableObject("Dado");
        scrollBoxes.gameObject.SetActive(true);

        //Añado la funcionalidad de los botones
        UIManager.Instance.AddListenerToButton("AddCasillas", () => {
            TableBoxData b = new TableBoxData();
            b.id = numBoxes;
            b.starter = false;
            b.winner = false;
            b.eat = false;
            b.tokensToWin = -1;
            b.pushBack = false;
            boxes.Add(b);

            GameObject newItem = Instantiate(boxItemUI, boxesContentPosition);
            int boxId = b.id;
            newItem.name = b.id.ToString();
            newItem.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text = boxId.ToString();
            boxesUI.Add(newItem);

            numBoxes++;
            UIManager.Instance.SetText("NumCasillas", numBoxes.ToString());
        });
        UIManager.Instance.AddListenerToButton("RemoveCasillas", () => {
            if (numBoxes > 0)
            {
                numBoxes--;
                Destroy(boxesUI[numBoxes]);
                boxesUI.RemoveAt(numBoxes);
                boxes.RemoveAt(numBoxes);
            }
            UIManager.Instance.SetText("NumCasillas", numBoxes.ToString());
        });
        UIManager.Instance.AddListenerToButton("Dado", () =>
        {
            if (dice == true)
            {
                diceImage.color = Color.white;
                dice = false;
            }
            else
            {
                diceImage.color = Color.green;
                dice = true;
            }
        });
    }

    /// <summary>
    /// Muestra la interfaz para editar los jugadores del juego de mesa.
    /// </summary>
    void tablePlayerSceneOptions()
    {
        //Muestro los elementos de UI
        UIManager.Instance.DisableObject("Jugadores");
        UIManager.Instance.DisableObject("Tablero");

        UIManager.Instance.EnableObject("NumJugadoresText");
        UIManager.Instance.EnableObject("NumJugadores");
        UIManager.Instance.EnableObject("AddJugadores");
        UIManager.Instance.EnableObject("RemoveJugadores");
        UIManager.Instance.EnableObject("NumFichasText");
        UIManager.Instance.EnableObject("NumFichas");
        UIManager.Instance.EnableObject("AddFichas");
        UIManager.Instance.EnableObject("RemoveFichas");
        UIManager.Instance.EnableObject("VolverTablero");
        scrollPlayers.gameObject.SetActive(true);

        //Posibles datos guardados
        UIManager.Instance.SetText("NumJugadores", numPlayers.ToString());
        UIManager.Instance.SetText("NumFichas", numTokens.ToString());

        //Añado la funcionalidad de los botones
        UIManager.Instance.AddListenerToButton("AddJugadores", () => { 
            TablePlayerData p = new TablePlayerData();
            p.playerType = TablePlayerType.LOCAL;
            p.id = numPlayers;
            p.tokenColor = getPlayerColor();
            players.Add(p);

            GameObject newItem = Instantiate(playerItemUI, playerContentPosition);
            int playerId = p.id;
            newItem.name = p.id.ToString();
            newItem.transform.Find("Ficha").GetComponent<RawImage>().color = p.tokenColor;
            newItem.transform.Find("TextoJugador").GetComponent<TextMeshProUGUI>().SetText($"Jugador {playerId}:");
            Transform buttonLocal = newItem.transform.Find("ButtonLocal");
            Transform buttonAI = newItem.transform.Find("ButtonIA");
            buttonLocal.GetComponent<Button>().onClick.AddListener(() =>
            {
                buttonLocal.GetComponent<Image>().color = Color.green;
                buttonAI.GetComponent<Image>().color = Color.white;

                foreach (TablePlayerData player in players)
                {
                    if (player.id == playerId)
                    {
                        player.playerType = TablePlayerType.LOCAL;
                    }
                }
            });
            buttonAI.GetComponent<Button>().onClick.AddListener(() =>
            {
                buttonAI.GetComponent<Image>().color = Color.green;
                buttonLocal.GetComponent<Image>().color = Color.white;

                foreach (TablePlayerData player in players)
                {
                    if (player.id == playerId)
                    {
                        player.playerType = TablePlayerType.IA;
                    }
                }
            });
            playersUI.Add(newItem);

            numPlayers++;
            UIManager.Instance.SetText("NumJugadores", numPlayers.ToString());
        });
        UIManager.Instance.AddListenerToButton("RemoveJugadores", () => { 
            if (numPlayers > 0)
            {
                numPlayers--;
                Destroy(playersUI[numPlayers]);
                playersUI.RemoveAt(numPlayers);
                players.RemoveAt(numPlayers);
            }
            UIManager.Instance.SetText("NumJugadores", numPlayers.ToString());
        });
        UIManager.Instance.AddListenerToButton("AddFichas", () => { numTokens++; UIManager.Instance.SetText("NumFichas", numTokens.ToString()); });
        UIManager.Instance.AddListenerToButton("RemoveFichas", () => {
            if (numTokens > 1)
                numTokens--;
            UIManager.Instance.SetText("NumFichas", numTokens.ToString());
        });
    }

    /// <summary>
    /// Devuelve un color aleatorio para las fichas de los jugadores.
    /// </summary>
    /// <returns>Color aleatorio.</returns>
    Color getPlayerColor()
    {
        if (numPlayers == 0)
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);

        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
        bool changed = false;
        bool repeat = true;

        while (repeat == true)
        {
            changed = false;
            foreach (TablePlayerData p in players)
            {
                if (similarColors(p.tokenColor, randomColor) == true)
                {
                    randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
                    changed = true; 
                    break;
                }
            }
            if (changed == false)
            {
                repeat = false;
            }
        }

        return randomColor;
    }

    /// <summary>
    /// Indica si dos colores son parecidos.
    /// </summary>
    /// <param name="a">Primer color.</param>
    /// <param name="b">Segundo color.</param>
    /// <returns>True si los dos colores se parecen, false en caso contrario.</returns>
    bool similarColors(Color a, Color b)
    {
        float tolerancia = 0.3f;
        float diferencia = Vector3.Distance(
            new Vector3(a.r, a.g, a.b),
            new Vector3(b.r, b.g, b.b)
        );

        return diferencia < tolerancia;
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