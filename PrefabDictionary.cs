using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase encargada de almacenar los prefabs que se cargarán en las escenas externas.
/// </summary>
public class PrefabDictionary : MonoBehaviour
{
    public static PrefabDictionary instance { get; private set; } //Instancia de la clase

    public List<GameObject> prefabs = new List<GameObject>();

    //Almacena los prefabs cargados de las escenas externas
    public Dictionary<string, GameObject> loadedPrefabs = new Dictionary<string, GameObject>();

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

        foreach (GameObject p in prefabs)
        {
            loadedPrefabs.Add(p.name, p);
        }
    }
}
