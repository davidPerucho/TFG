using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            Debug.LogError("Error, ya existe una instancia de la clase DataPersitance.");
        }

        instance = this;
    }

    private void Start()
    {
        fileDataManager = new FileDataManager(Application.persistentDataPath, fileName);
        persitentObjects = findPersistenceObjects();
        loadGame();
    }

    /// <summary>
    /// Crea una nueva partida.
    /// </summary>
    public void newGame()
    {
        gameData = new GameData();
    }

    /// <summary>
    /// Guarda los datos de cada objeto.
    /// </summary>
    public void saveGame()
    {
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
            Debug.Log("Iniciando una nueva partida.");
            newGame();
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
