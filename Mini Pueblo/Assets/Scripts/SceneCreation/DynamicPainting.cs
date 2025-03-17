using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DynamicPainting : MonoBehaviour
{
    void Awake()
    {
        //Obtengo el json con la información de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        //Leo la información de los personajes y los creo en la escena
        string json = File.ReadAllText(filePath);
        PaintingSceneData characterList = JsonUtility.FromJson<PaintingSceneData>(json);

        Debug.Log(characterList.sceneThemeEnglish);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
