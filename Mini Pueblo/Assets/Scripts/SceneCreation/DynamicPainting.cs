using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicPainting : MonoBehaviour
{
    [HideInInspector]
    public PaintingSceneData sceneData; //Informaci�n de la escena

    [SerializeField]
    TextMeshProUGUI botonCrear;

    void Awake()
    {
        //Obtengo el json con la informaci�n de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        //Leo la informaci�n de los personajes y los creo en la escena
        string json = File.ReadAllText(filePath);
        sceneData = JsonUtility.FromJson<PaintingSceneData>(json);

        //Mandala como tema por defecto
        if (sceneData == null || sceneData.sceneThemeEnglish == "")
        {
            sceneData = new PaintingSceneData();
            sceneData.sceneThemeEnglish = "mandala";
            sceneData.sceneThemeSpanish = "mandala";
        }

        //Cambio el prompt para la generaci�n de im�genes
        string newPrompt = $"Create a simple colorbook page of a {sceneData.sceneThemeEnglish} with black lines that are fully closed";
        GetComponent<ImageGenerator>().prompt = newPrompt;

        botonCrear.text = $"CREAR {sceneData.sceneThemeSpanish.ToUpper()}";
    }
}
