using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersitence : MonoBehaviour
{
    public static DataPersitence instance { get; private set; }

    GameData gameData;
    List<IDataPersistence> persitentObjects;

    [SerializeField] string fileName;
    FileDataManager fileDataManager;

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

    public void newGame()
    {
        gameData = new GameData();
    }

    public void saveGame()
    {
        foreach (IDataPersistence obj in persitentObjects)
        {
            obj.saveData(ref gameData);
        }

        fileDataManager.save(gameData);
    }

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

    List<IDataPersistence> findPersistenceObjects() 
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }
}
