using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreationManager : MonoBehaviour
{
    Scene createdScene;
    string sceneSavePath;

    void Awake()
    {
        //Creo la nueva escena con el nombre insertado en el menu de creación
        string sceneName = PlayerPrefs.GetString("SceneName", "Minijuego");
        createdScene = SceneManager.CreateScene(sceneName);

        //Creo la ruta de guardado para la escena
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Scenes/"))) //Comprobar que el directorio existe
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Scenes/"));
            Debug.Log($"El directorio no existía, pero se ha creado");
        }
        sceneSavePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneName + ".json");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
