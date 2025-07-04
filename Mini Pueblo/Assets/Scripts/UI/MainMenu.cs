using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


/// <summary>
/// Clase encargada del manejo de la UI y las funcionalidades del menu principal y de la creaci�n de escenas.
/// </summary>
public class MainMenu : MonoBehaviour, IDataPersistence
{
    FileDataManager fileDataManager; //Manejador de datos guardados del juego
    RawImage[] characters; //Almacena las im�genes de los personajes
    Image[] locations; //Almacena el color del boton de los personajes
    int screenindex = 0; //Index que indica la parte del menu que se est� mostrando
    int characterIndex = 0; //Index del personaje seleccionado
    int locationIndex = 0; //Index de la localizaci�n
    static readonly List<string> sceneList = new List<string> { "DynamicScene", "Hub", "MainMenu", "MandalaPainting", "PaperPlane", "SceneCreation", "ShootingGame", "TutorialPaperPlane" }; //Lista de nombres de escenas ya existentes

    [SerializeField]
    GameObject characterSelection; //Selector de personajes

    [SerializeField]
    GameObject locationSelection; //Selector de personajes

    [SerializeField]
    GameObject terrainImage; //Imagen del terreno

    [SerializeField]
    GameObject titeImage; //Imagen del t�tulos

    [SerializeField]
    GameObject musicVolumeSelector; //Selector del volumen de la m�sica

    [SerializeField]
    GameObject sfxVolumeSelector; //Selector del volumen de los efectos

    [SerializeField]
    GameObject ttsCheck; //Selector de las voces de los personajes


    // Start is called before the first frame update
    void Start()
    {
        //A�ado listeners para cuando se cambia el volumen de la musica
        musicVolumeSelector.GetComponent<Slider>().onValueChanged.AddListener(changeMusicVolume);

        //A�ado las funciones a los botones
        UIManager.Instance.AddListenerToButton("Jugar", play);
        UIManager.Instance.AddListenerToButton("Continuar", continueCreation);
        UIManager.Instance.AddListenerToButton("Crear", continueCreation);
        UIManager.Instance.AddListenerToButton("Volver", returnToMenu);
        UIManager.Instance.AddListenerToButton("CrearMenu", openCreateSceneInterface);
        UIManager.Instance.AddListenerToButton("VolverOpciones", hideOptions);
        UIManager.Instance.AddListenerToButton("Resetear", resetOptions);
        UIManager.Instance.AddListenerToButton("Opciones", showOptionsMenu);
        fileDataManager = new FileDataManager(Application.persistentDataPath, "datos.json");
        UIManager.Instance.AddListenerToButton("Nueva", newGame);
        if (fileDataManager.file_exists() == false) 
        {
            UIManager.Instance.DisableButton("Jugar");
        }
        UIManager.Instance.AddListenerToButton("Salir", exitGame);

        //Cargo los valores del menu de opciones
        DataPersitence.instance.reloadObjects();
        DataPersitence.instance.loadGame();
    }

    /// <summary>
    /// Funci�n que se encarga de salir del juego.
    /// </summary>
    void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Cambia el volumen de la m�sica.
    /// </summary>
    /// <param name="volume">Nuevo volumen de la m�sica.</param>
    void changeMusicVolume(float volume)
    {
        GetComponent<AudioSource>().volume = volume;
    }

    /// <summary>
    /// Muestra el menu de opciones.
    /// </summary>
    void showOptionsMenu()
    {
        UIManager.Instance.DisableObject("CrearMenu");
        UIManager.Instance.DisableObject("Nueva");
        UIManager.Instance.DisableObject("Jugar");
        UIManager.Instance.DisableObject("Opciones");
        UIManager.Instance.DisableObject("Salir");

        UIManager.Instance.EnableObject("VolverOpciones");
        UIManager.Instance.EnableObject("Resetear");
        UIManager.Instance.EnableObject("TextoMusica");
        UIManager.Instance.EnableObject("TextoEfectos");
        musicVolumeSelector.SetActive(true);
        sfxVolumeSelector.SetActive(true);
        ttsCheck.SetActive(true);
    }

    /// <summary>
    /// Oculta el menu de opciones y vuelve al menu principal.
    /// </summary>
    void hideOptions()
    {
        UIManager.Instance.DisableObject("VolverOpciones");
        UIManager.Instance.DisableObject("Resetear");
        UIManager.Instance.DisableObject("TextoMusica");
        UIManager.Instance.DisableObject("TextoEfectos");
        musicVolumeSelector.SetActive(false);
        sfxVolumeSelector.SetActive(false);
        ttsCheck.SetActive(false);

        UIManager.Instance.EnableObject("CrearMenu");
        UIManager.Instance.EnableObject("Nueva");
        UIManager.Instance.EnableObject("Jugar");
        UIManager.Instance.EnableObject("Opciones");
        UIManager.Instance.EnableObject("Salir");
    }

    /// <summary>
    /// Resetea los valores de las opciones devolviendolos a su valor por defecto.
    /// </summary>
    void resetOptions()
    {
        musicVolumeSelector.GetComponent<Slider>().value = 1;
        sfxVolumeSelector.GetComponent<Slider>().value = 1;
        ttsCheck.GetComponent<Toggle>().isOn = true;
    }

    /// <summary>
    /// Inicia el juego sin crear una nueva partida.
    /// </summary>
    void play()
    {
        disableButtons();
        DataPersitence.instance.reloadObjects();
        DataPersitence.instance.saveGame();
        SceneManager.LoadSceneAsync("Hub");
    }

    /// <summary>
    /// Crea una nueva partida e inicia el juego.
    /// </summary>
    void newGame()
    {
        disableButtons();
        DataPersitence.instance.newGame();
        DataPersitence.instance.reloadObjects();
        DataPersitence.instance.saveGame();
        SceneManager.LoadSceneAsync("Hub");
    }

    /// <summary>
    /// Desactiva los elementos de UI del menu y activa los de la creaci�n de minijuegos/actividades.
    /// </summary>
    void openCreateSceneInterface()
    {
        titeImage.SetActive(false);

        UIManager.Instance.DisableObject("Nueva");
        UIManager.Instance.DisableObject("Jugar");
        UIManager.Instance.DisableObject("CrearMenu");
        UIManager.Instance.DisableObject("Opciones");
        UIManager.Instance.DisableObject("Salir");

        UIManager.Instance.EnableObject("Continuar");
        UIManager.Instance.EnableObject("Volver");
        UIManager.Instance.EnableObject("InputNombre");
        UIManager.Instance.EnableObject("TextoCrear");

        UIManager.Instance.SetText("TextoMenu", "NOMBRE DE LA ESCENA");
    }

    /// <summary>
    /// Carga la escena de craci�n de minijuegos/actividades.
    /// </summary>
    void continueCreation()
    {
        if (screenindex == 0) //Insercci�n del nombre
        {
            string inputText = UIManager.Instance.GetInputValue("InputNombre");

            if (inputText == "") //En caso de que el jugador no introduzca ning�n texto muestro un error
            {
                UIManager.Instance.SetText("Error", "Tienes que ponerle un nombre al juego");
                UIManager.Instance.EnableObject("Error");
            }
            else if (sceneExists(inputText) == true) //En caso de que ya exista una escena con el nombre introducido muestro un error
            {
                UIManager.Instance.SetText("Error", "El nombre no es v�lido");
                UIManager.Instance.EnableObject("Error");
            }
            else if (jsonExists(inputText) == false)
            {
                //Guardo el nombre de la escena y paso a la selecci�n de personaje
                PlayerPrefs.SetString("SceneName", inputText);
                PlayerPrefs.Save();
                startCharacterSelection();
            }
            else
            {
                //Guardo el nombre de la escena para su posterior creaci�n y cargo la escena de creaci�n de minijuegos/actividades
                PlayerPrefs.SetString("SceneName", inputText);
                PlayerPrefs.Save();
                SceneManager.LoadScene("SceneCreation");
            }
        }
        else if (screenindex == 1) //Selecci�n del personaje
        {
            if (characterIndex == 0)
            {
                UIManager.Instance.SetText("Error", "Tienes que seleccionar un personaje");
                UIManager.Instance.EnableObject("Error");
            }
            else
            {
                PlayerPrefs.SetString("SelectedNPC", characterIndex.ToString());
                PlayerPrefs.Save();
                startLocationSelection();
            }
        }
        else if (screenindex == 2) //Selecci�n de la localizaci�n
        {
            if (locationIndex == 0)
            {
                UIManager.Instance.SetText("Error", "Tienes que seleccionar una localizaci�n");
                UIManager.Instance.EnableObject("Error");
            }
            else
            {
                PlayerPrefs.SetString("SelectedLocation", locationIndex.ToString());
                PlayerPrefs.Save();
                startPhraseSelection();
            }
        }
        else
        {
            string inputText = UIManager.Instance.GetInputValue("InputNombre");

            if (inputText == "") //En caso de que el jugador no introduzca ning�n texto muestro un error
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
    /// Inicializa los elementos de la selecci�n de personaje.
    /// </summary>
    void startCharacterSelection()
    {
        UIManager.Instance.SetText("TextoMenu", "SELECCIONAR PERSONAJE");
        UIManager.Instance.DisableObject("InputNombre");
        UIManager.Instance.DisableObject("Error");
        characterSelection.SetActive(true);

        //Obtengo las im�genes de los personajes
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
    /// Inicializa los elementos de la selecci�n de localizaci�n.
    /// </summary>
    void startLocationSelection()
    {
        UIManager.Instance.SetText("TextoMenu", "SELECCIONAR LOCALIZACI�N");
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
    /// Inicializa los elementos de la selecci�n de frase para el personaje.
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
            Debug.Log($"El directorio no exist�a, pero se ha creado: {scenesPath}");

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
    /// Devuelve true si ya existe un json de la creaci�n de una escena con el nombre pasado por argumento.
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
    /// Activa los elementos de UI del menu y desactiva los de la creaci�n de minijuegos/actividades.
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
        UIManager.Instance.EnableObject("Salir");

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

        UIManager.Instance.SetText("TextoMenu", "");
        titeImage.SetActive(true);
    }

    /// <summary>
    /// Desabilita la UI principal.
    /// </summary>
    void disableButtons()
    {
        UIManager.Instance.DisableButton("Nueva");
        UIManager.Instance.DisableButton("Jugar");
        UIManager.Instance.DisableButton("Opciones");
        UIManager.Instance.DisableButton("Salir");
        UIManager.Instance.DisableButton("CrearMenu");
    }

    /// <summary>
    /// Elimina la selecci�n anterior de personaje y realiza la nueva.
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
    /// Elimina la selecci�n anterior de personaje y realiza la nueva.
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

    /// <summary>
    /// Carga los datos de las opciones.
    /// </summary>
    /// <param name="data">Datos del juego.</param>
    public void loadData(GameData data)
    {
        musicVolumeSelector.GetComponent<Slider>().value = data.musicVolume;
        sfxVolumeSelector.GetComponent<Slider>().value = data.sfxVolume;
        ttsCheck.GetComponent<Toggle>().isOn = data.characterVoices;
    }

    /// <summary>
    /// Guarda los datos del menu de opciones.
    /// </summary>
    /// <param name="data">Datos del juego.</param>
    public void saveData(ref GameData data)
    {
        data.musicVolume = musicVolumeSelector.GetComponent<Slider>().value;
        data.sfxVolume = sfxVolumeSelector.GetComponent<Slider>().value;
        data.characterVoices = ttsCheck.GetComponent<Toggle>().isOn;
    }
}
