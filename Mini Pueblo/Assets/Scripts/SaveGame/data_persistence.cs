using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que se encarga de la persistencia de datos de la escena.
/// </summary>
public class DataPersitence : MonoBehaviour
{
    public static DataPersitence instance { get; private set; } //Instancia de la clase

    GameData gameData; //Datos del juego
    List<IDataPersistence> persitentObjects; //Lista de objetos que almacenarán sus datos

    [SerializeField] 
    string fileName; //Nombre del fichero

    FileDataManager fileDataManager; //Se encarga de manejar el fichero.

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Se ha destruido una instancia duplicada de guardado.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); //Evita que el objeto se destruya al pasar de escena

        fileDataManager = new FileDataManager(Application.persistentDataPath, fileName);
    }

    /// <summary>
    /// Función que se llama cuando se entra en la escena.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneEnter;
    }

    /// <summary>
    /// Función que se llama cuando se sale de la escena.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneEnter;
    }

    /// <summary>
    /// Función que se llama cuando se sale del juego.
    /// </summary>
    private void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= SceneEnter;

        if (SceneManager.GetActiveScene().name == "Hub")
        {
            saveGame();
        }
    }

    /// <summary>
    /// Carga los datos de los objetos en escena.
    /// </summary>
    private void SceneEnter(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            persitentObjects = findPersistenceObjects();
            loadGame();
        }
    }

    /// <summary>
    /// Crea una nueva partida.
    /// </summary>
    public void newGame()
    {
        if (fileDataManager.file_exists())
        {
            fileDataManager.delete_save();
        }
        
        gameData = new GameData();
        fileDataManager.save(gameData);
    }

    /// <summary>
    /// Guarda los datos de cada objeto.
    /// </summary>
    public void saveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("No existe partida para guardar.");
            return;
        }

        foreach (IDataPersistence obj in persitentObjects)
        {
            obj.saveData(ref gameData);
        }

        fileDataManager.save(gameData);
    }

    /// <summary>
    /// Carga los datos de los objetos, iniciando la partida.
    /// </summary>
    public void loadGame()
    {
        gameData = fileDataManager.load();

        if (gameData == null)
        {
            Debug.LogWarning("No existe partida para cargar.");
            return;
        }

        foreach (IDataPersistence obj in persitentObjects)
        {
            obj.loadData(gameData);
        }
    }

    /// <summary>
    /// Guarda en una lista todos los objetos que guardan datos.
    /// </summary>
    /// <returns>Lista con los objetos que guardan datos.</returns>
    List<IDataPersistence> findPersistenceObjects() 
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }
}
