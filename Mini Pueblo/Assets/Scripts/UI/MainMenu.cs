using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


/// <summary>
/// Clase encargada del manejo de la UI y las funcionalidades del menu principal y de la creación de escenas.
/// </summary>
public class MainMenu : MonoBehaviour
{
    FileDataManager fileDataManager; //Manejador de datos guardados del juego
    RawImage[] characters; //Almacena las imágenes de los personajes
    Image[] locations; //Almacena el color del boton de los personajes
    int screenindex = 0; //Index que indica la parte del menu que se está mostrando
    int characterIndex = 0; //Index del personaje seleccionado
    int locationIndex = 0; //Index de la localización
    static readonly List<string> sceneList = new List<string> { "DynamicScene", "Hub", "MainMenu", "MandalaPainting", "PaperPlane", "SceneCreation", "ShootingGame", "TutorialPaperPlane" }; //Lista de nombres de escenas ya existentes

    [SerializeField]
    GameObject characterSelection; //Selector de personajes

    [SerializeField]
    GameObject locationSelection; //Selector de personajes

    [SerializeField]
    GameObject terrainImage; //Imagen del terreno

    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.AddListenerToButton("Jugar", play);
        UIManager.Instance.AddListenerToButton("Continuar", continueCreation);
        UIManager.Instance.AddListenerToButton("Crear", continueCreation);
        UIManager.Instance.AddListenerToButton("Volver", returnToMenu);
        UIManager.Instance.AddListenerToButton("CrearMenu", openCreateSceneinterface);
        fileDataManager = new FileDataManager(Application.persistentDataPath, "datos.json");
        UIManager.Instance.AddListenerToButton("Nueva", newGame);
        if (fileDataManager.file_exists() == false) 
        {
            UIManager.Instance.DisableObject("Jugar");
        }
    }

    /// <summary>
    /// Inicia el juego sin crear una nueva partida.
    /// </summary>
    void play()
    {
        disableButtons();
        SceneManager.LoadSceneAsync("Hub");
    }

    /// <summary>
    /// Crea una nueva partida e inicia el juego.
    /// </summary>
    void newGame()
    {
        disableButtons();
        DataPersitence.instance.newGame();
        SceneManager.LoadSceneAsync("Hub");
    }

    /// <summary>
    /// Desactiva los elementos de UI del menu y activa los de la creación de minijuegos/actividades.
    /// </summary>
    void openCreateSceneinterface()
    {
        UIManager.Instance.DisableObject("Nueva");
        UIManager.Instance.DisableObject("Jugar");
        UIManager.Instance.DisableObject("CrearMenu");
        UIManager.Instance.DisableObject("Opciones");

        UIManager.Instance.EnableObject("Continuar");
        UIManager.Instance.EnableObject("Volver");
        UIManager.Instance.EnableObject("InputNombre");
        UIManager.Instance.EnableObject("TextoCrear");

        UIManager.Instance.SetText("TextoMenu", "NOMBRE DE LA ESCENA");
    }

    /// <summary>
    /// Carga la escena de cración de minijuegos/actividades.
    /// </summary>
    void continueCreation()
    {
        if (screenindex == 0) //Insercción del nombre
        {
            string inputText = UIManager.Instance.GetInputValue("InputNombre");

            if (inputText == "") //En caso de que el jugador no introduzca ningún texto muestro un error
            {
                UIManager.Instance.SetText("Error", "Tienes que ponerle un nombre al juego");
                UIManager.Instance.EnableObject("Error");
            }
            else if (sceneExists(inputText) == true) //En caso de que ya exista una escena con el nombre introducido muestro un error
            {
                UIManager.Instance.SetText("Error", "El nombre no es válido");
                UIManager.Instance.EnableObject("Error");
            }
            else if (jsonExists(inputText) == false)
            {
                //Guardo el nombre de la escena y paso a la selección de personaje
                PlayerPrefs.SetString("SceneName", inputText);
                PlayerPrefs.Save();
                startCharacterSelection();
            }
            else
            {
                //Guardo el nombre de la escena para su posterior creación y cargo la escena de creación de minijuegos/actividades
                PlayerPrefs.SetString("SceneName", inputText);
                PlayerPrefs.Save();
                SceneManager.LoadScene("SceneCreation");
            }
        }
        else if (screenindex == 1) //Selección del personaje
        {
            if (characterIndex == 0)
            {
                UIManager.Instance.SetText("Error", "Tienes que seleccionar un personaje");
                UIManager.Instance.EnableObject("Error");
            }
            else
            {
                Debug.Log("Se ha seleccionado la imagen: " + characterIndex);
                PlayerPrefs.SetString("SelectedNPC", characterIndex.ToString());
                PlayerPrefs.Save();
                startLocationSelection();
            }
        }
        else if (screenindex == 2) //Selección de la localización
        {
            if (locationIndex == 0)
            {
                UIManager.Instance.SetText("Error", "Tienes que seleccionar una localización");
                UIManager.Instance.EnableObject("Error");
            }
            else
            {
                Debug.Log("Se ha seleccionado la localización: " + locationIndex);
                PlayerPrefs.SetString("SelectedLocation", locationIndex.ToString());
                PlayerPrefs.Save();
                startPhraseSelection();
            }
        }
        else
        {
            string inputText = UIManager.Instance.GetInputValue("InputNombre");

            if (inputText == "") //En caso de que el jugador no introduzca ningún texto muestro un error
            {
                UIManager.Instance.SetText("Error", "Tienes que introducir una frase");
                UIManager.Instance.EnableObject("Error");
            }
            else
            {
                PlayerPrefs.SetString("CharacterPhrase", inputText);
                PlayerPrefs.Save();
                SceneManager.LoadScene("SceneCreation");
            }
        }
    }

    /// <summary>
    /// Inicializa los elementos de la selección de personaje.
    /// </summary>
    void startCharacterSelection()
    {
        UIManager.Instance.SetText("TextoMenu", "SELECCIONAR PERSONAJE");
        UIManager.Instance.DisableObject("InputNombre");
        UIManager.Instance.DisableObject("Error");
        characterSelection.SetActive(true);

        //Obtengo las imágenes de los personajes
        characters = characterSelection.GetComponentsInChildren<RawImage>();

        foreach (RawImage character in characters)
        {
            character.color = new Color(character.color.r, character.color.g, character.color.b, 0.5f);
            GameObject gameObject = character.gameObject;
            if (gameObject.GetComponent<Button>() == null)
            {
                Button button = gameObject.AddComponent<Button>();
                button.onClick.AddListener(() => characterSelected(gameObject));
            }
        }

        screenindex++;
    }

    /// <summary>
    /// Inicializa los elementos de la selección de localización.
    /// </summary>
    void startLocationSelection()
    {
        UIManager.Instance.SetText("TextoMenu", "SELECCIONAR LOCALIZACIÓN");
        UIManager.Instance.DisableObject("Error");
        characterSelection.SetActive(false);

        terrainImage.SetActive(true);
        locationSelection.SetActive(true);

        Button[] buttons = locationSelection.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => locationSelected(button.gameObject));
        }

        locations = locationSelection.GetComponentsInChildren<Image>();

        screenindex++;
    }

    /// <summary>
    /// Inicializa los elementos de la selección de frase para el personaje.
    /// </summary>
    void startPhraseSelection()
    {
        UIManager.Instance.SetText("TextoMenu", "INTRODUCE LA FRASE");
        UIManager.Instance.DisableObject("Error");

        UIManager.Instance.EnableObject("InputNombre");
        UIManager.Instance.SetInputValue("InputNombre", "");
        terrainImage.SetActive(false);
        locationSelection.SetActive(false);

        screenindex++;
    }

    /// <summary>
    /// Devuelve true si ya existe una escena con el nombre pasado por argumento.
    /// </summary>
    /// <param name="sceneName">Nombre de la escena que se quiere crear.</param>
    /// <returns>True si existe la escena false si no</returns>
    bool sceneExists(string sceneName)
    {
        if (sceneList.Contains(sceneName)) {
            return true;
        }

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
    /// Devuelve true si ya existe un json de la creación de una escena con el nombre pasado por argumento.
    /// </summary>
    /// <param name="sceneName">Nombre de la escena que se quiere crear.</param>
    /// <returns>True si existe el json de la escena false si no</returns>
    bool jsonExists(string sceneName)
    {
        string creationSavePath = Path.Combine(Application.persistentDataPath, "Managers/" + "ScenesOnCreation.json");

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
    /// Activa los elementos de UI del menu y desactiva los de la creación de minijuegos/actividades.
    /// </summary>
    void returnToMenu()
    {
        UIManager.Instance.DisableObject("Crear");
        UIManager.Instance.DisableObject("Continuar");
        UIManager.Instance.DisableObject("Volver");
        UIManager.Instance.DisableObject("InputNombre");
        UIManager.Instance.DisableObject("Error");

        UIManager.Instance.EnableObject("Nueva");
        UIManager.Instance.EnableObject("Jugar");
        UIManager.Instance.EnableObject("CrearMenu");
        UIManager.Instance.EnableObject("Opciones");

        if (fileDataManager.file_exists() == false)
        {
            UIManager.Instance.DisableObject("Jugar");
        }

        characterSelection.SetActive(false);
        if (characterIndex != 0)
        {
            characters[characterIndex - 1].color = new Color(characters[characterIndex - 1].color.r, characters[characterIndex - 1].color.g, characters[characterIndex - 1].color.b, 0.5f);
        }
        characterIndex = 0;
        screenindex = 0;
        locationSelection.SetActive(false);
        terrainImage.SetActive(false);
        UIManager.Instance.SetText("TextoMenu", "MINI PUEBLO");
    }

    /// <summary>
    /// Desabilita la UI principal.
    /// </summary>
    void disableButtons()
    {
        UIManager.Instance.DisableButton("Nueva");
        UIManager.Instance.DisableButton("Jugar");
        UIManager.Instance.DisableButton("Opciones");
        UIManager.Instance.DisableObject("CrearMenu");
    }

    /// <summary>
    /// Elimina la selección anterior de personaje y realiza la nueva.
    /// </summary>
    /// <param name="characterObject">Objeto sobre el que se ha hecho click.</param>
    void characterSelected(GameObject characterObject)
    {
        int imageIndex = characterIndex - 1;
        if (characterIndex != 0) 
            characters[imageIndex].color = new Color(characters[imageIndex].color.r, characters[imageIndex].color.g, characters[imageIndex].color.b, 0.5f);

        imageIndex = int.Parse(characterObject.name) - 1;
        characters[imageIndex].color = new Color(characters[imageIndex].color.r, characters[imageIndex].color.g, characters[imageIndex].color.b, 1);
        characterIndex = imageIndex + 1;
    }

    /// <summary>
    /// Elimina la selección anterior de personaje y realiza la nueva.
    /// </summary>
    /// <param name="locationObject">Objeto sobre el que se ha hecho click.</param>
    void locationSelected(GameObject locationObject)
    {
        int index = locationIndex - 1;
        if (locationIndex != 0)
            locations[index].color = Color.white;

        index = int.Parse(locationObject.name) - 1;
        locations[index].color = Color.blue;
        locationIndex = index + 1;
    }
}
