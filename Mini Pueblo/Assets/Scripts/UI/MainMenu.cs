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
    int screenindex = 0; //Index que indica la parte del menu que se está mostrando
    int characterIndex = 0;

    [SerializeField]
    GameObject characterSelection; //Selector de personajes

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

        UIManager.Instance.SetText("TextoMenu", "CREAR MINIJUEGO");
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
            }
        }
        else //Selección de la localización
        {

        }
    }

    /// <summary>
    /// Inicializa los elementos de la selección de personaje.
    /// </summary>
    void startCharacterSelection()
    {
        UIManager.Instance.SetText("TextoMenu", "SELECCIONAR PERSONAJE");
        UIManager.Instance.DisableObject("InputNombre");
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
        characterSelection.SetActive(false);

        screenindex++;
    }

    /// <summary>
    /// Devuelve true si ya existe una escena con el nombre pasado por argumento.
    /// </summary>
    /// <param name="sceneName">Nombre de la escena que se quiere crear.</param>
    /// <returns>True si existe la escena false si no</returns>
    bool sceneExists(string sceneName)
    {
        string scenesPath = Path.Combine(Application.dataPath, "Scenes/");
        string[] sceneFiles = Directory.GetFiles(scenesPath, "*.unity", SearchOption.AllDirectories);

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

        characterSelection.gameObject.SetActive(false);
        if (characterIndex != 0)
        {
            characters[characterIndex - 1].color = new Color(characters[characterIndex - 1].color.r, characters[characterIndex - 1].color.g, characters[characterIndex - 1].color.b, 0.5f);
        }
        characterIndex = 0;
        screenindex = 0;
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
    /// <param name="gameObject">Objeto sobre el que se ha hecho click.</param>
    void characterSelected(GameObject gameObject)
    {
        int imageIndex = characterIndex - 1;
        if (characterIndex != 0) 
            characters[imageIndex].color = new Color(characters[imageIndex].color.r, characters[imageIndex].color.g, characters[imageIndex].color.b, 0.5f);

        imageIndex = int.Parse(gameObject.name) - 1;
        characters[imageIndex].color = new Color(characters[imageIndex].color.r, characters[imageIndex].color.g, characters[imageIndex].color.b, 1);
        characterIndex = imageIndex + 1;
    }
}
