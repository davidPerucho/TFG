using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DynamicLoading : MonoBehaviour
{
    void Awake()
    {
        //Obtengo el json con la informaci�n de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneName", "CarGame");
        string characterIndex = PlayerPrefs.GetString("SelectedNPC", "1");
        string locationIndex = PlayerPrefs.GetString("SelectedLocation", "1");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        Debug.Log("Nombre escena: " + sceneToLoad + ", Index personaje: " + characterIndex + ", Index localizaci�n: " + locationIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
