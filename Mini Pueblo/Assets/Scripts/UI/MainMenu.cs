using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Clase encargada del manejo de la UI y las funcionalidades del menu principal y de la creación de escenas.
/// </summary>
public class MainMenu : MonoBehaviour
{
    FileDataManager fileDataManager; //Manejador de datos guardados del juego

    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.AddListenerToButton("Jugar", play);
        UIManager.Instance.AddListenerToButton("Crear", create);
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
        UIManager.Instance.DisableObject("TextoMenu");
        UIManager.Instance.DisableObject("CrearMenu");
        UIManager.Instance.DisableObject("Opciones");

        UIManager.Instance.EnableObject("Crear");
        UIManager.Instance.EnableObject("Volver");
        UIManager.Instance.EnableObject("InputNombre");
        UIManager.Instance.EnableObject("TextoCrear");
    }

    /// <summary>
    /// Carga la escena de cración de minijuegos/actividades.
    /// </summary>
    void create()
    {
        string inputText = UIManager.Instance.GetInputValue("InputNombre");

        if (inputText == "") //En caso de que el jugador no introduzca ningún texto muestro un error
        {
            UIManager.Instance.SetText("Error", "Tienes que ponerle un nombre al juego");
            UIManager.Instance.EnableObject("Error");
        }
        else if (sceneExists(inputText) == true) //En caso de que ya exista una escena con el nombre introducido muestro un error
        {
            UIManager.Instance.SetText("Error", "Ya existe un minijuego con este nombre");
            UIManager.Instance.EnableObject("Error");
        }
        else
        {
            //Guardo el nombre de la escena para su posterior creación y cargo la escena de creación de minijuegos/actividades
            PlayerPrefs.SetString("SceneName", inputText);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SceneCreation");
        }
    }

    /// <summary>
    /// Devuelve true si ya existe una escena con el nombre pasado por argumento.
    /// </summary>
    /// <param name="sceneName">Nombre de la escena que se quiere crear.</param>
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
    /// Activa los elementos de UI del menu y desactiva los de la creación de minijuegos/actividades.
    /// </summary>
    void returnToMenu()
    {
        UIManager.Instance.DisableObject("Crear");
        UIManager.Instance.DisableObject("Volver");
        UIManager.Instance.DisableObject("InputNombre");
        UIManager.Instance.DisableObject("TextoCrear");
        UIManager.Instance.DisableObject("Error");

        UIManager.Instance.EnableObject("Nueva");
        UIManager.Instance.EnableObject("Jugar");
        UIManager.Instance.EnableObject("TextoMenu");
        UIManager.Instance.EnableObject("CrearMenu");
        UIManager.Instance.EnableObject("Opciones");

        if (fileDataManager.file_exists() == false)
        {
            UIManager.Instance.DisableObject("Jugar");
        }
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
}
