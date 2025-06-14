using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

/// <summary>
/// Clase encargada de cargar los scripts de las escenas externas
/// </summary>
public class ExternalLoading : MonoBehaviour
{
    public static ExternalLoading instance { get; private set; } //Instancia de la clase

    [HideInInspector]
    public List<string> scenesLoaded = new List<string>(); //Almacena aquellas escenas que se hayan cargado

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

        SceneManager.sceneLoaded += OnSceneLoaded; //Llamo a la función OnSceneLoaded cada vez que se entra a una escena
    }

    /// <summary>
    /// Función encargada de la carga de scripts de escenas externas.
    /// </summary>
    /// <param name="scene">Nombre de la escena a la que se ha entrado</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scenesLoaded.Contains(scene.name) == false)
        {
            string dllPath = System.IO.Path.Combine(Application.persistentDataPath, $"{scene.name}/{scene.name}.dll");
            string typesPath = Path.Combine(Application.persistentDataPath, "Scenes/ScenesTypes.json");
            string jsonTypes = File.ReadAllText(typesPath);
            ScenesTypes scenesTypes = JsonUtility.FromJson<ScenesTypes>(jsonTypes);
            SceneType sceneType = SceneType.TABLE;

            //Compruebo que la escena sea una escena externa
            foreach (SceneTuple s in scenesTypes.scenes)
            {
                if (s.name == scene.name)
                {
                    sceneType = s.type; 
                    break; 
                }
            }

            if (sceneType == SceneType.EXTERNAL)
            {
                //En caso de que exista código para cargar lo cargo
                if (File.Exists(dllPath))
                {
                    if (loadSceneCode(dllPath) == false)
                    {
                        Debug.LogError("Error en la carga del dll");
                        SceneManager.LoadScene("Hub");
                        return;
                    }
                    else
                    {
                        scenesLoaded.Add(scene.name);
                    }
                }
                else
                {
                    scenesLoaded.Add(scene.name);
                }
            }
        }
    }

    /// <summary>
    /// Carga el código para una escena almacenado en un dll.
    /// </summary>
    /// <param name="dllPath">Ruta del dll.</param>
    /// <returns>Devuelve true si todo ha ido bien, false en caso contrario.</returns>
    bool loadSceneCode(string dllPath)
    {
        byte[] dllBytes = File.ReadAllBytes(dllPath);
        Assembly assembly = null;

        //Si existe el archivo pdb lo cargo junto con el dll para una mejor comprensión del código
        if (File.Exists(Path.ChangeExtension(dllPath, ".pdb"))) 
        {
            byte[] pdbBytes = File.ReadAllBytes(Path.ChangeExtension(dllPath, ".pdb"));
            assembly = Assembly.Load(dllBytes, pdbBytes);
        }
        else
        {
            assembly = Assembly.Load(dllBytes);
        }

        if (assembly == null || assembly.GetTypes().Length == 0)
        {
            return false;
        }
        else
        {
            foreach (System.Type t in assembly.GetTypes())
            {
                // Obtener todos los atributos aplicados a la clase
                var attrs = t.GetCustomAttributes(false);

                foreach (var attr in attrs)
                {
                    if (attr.GetType().Name == "ExternalAttribute")
                    {
                        PropertyInfo prop = attr.GetType().GetProperty("GameObjectName");
                        string goName = prop?.GetValue(attr) as string;

                        //Debug.Log($"Encontrado atributo en {t.Name} para GameObject: {goName}");

                        GameObject obj = GameObject.Find(goName);
                        if (obj != null && typeof(MonoBehaviour).IsAssignableFrom(t))
                        {
                            obj.AddComponent(t);
                        }
                    }
                }
            }
        }

        return true;
    }
}
