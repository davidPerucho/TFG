using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DynamicLoading : MonoBehaviour
{
    void Awake()
    {
        //Obtengo el json con la información de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "CarGame");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
