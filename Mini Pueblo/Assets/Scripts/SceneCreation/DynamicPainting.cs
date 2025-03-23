using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicPainting : MonoBehaviour
{
    [HideInInspector]
    public PaintingSceneData sceneData; //Información de la escena

    [SerializeField]
    TextMeshProUGUI botonCrear;

    void Awake()
    {
        //Obtengo el json con la información de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        //Leo la información de los personajes y los creo en la escena
        string json = File.ReadAllText(filePath);
        sceneData = JsonUtility.FromJson<PaintingSceneData>(json);

        //Mandala como tema por defecto
        if (sceneData == null || sceneData.sceneThemeEnglish == "")
        {
            sceneData = new PaintingSceneData();
            sceneData.sceneThemeEnglish = "mandala";
            sceneData.sceneThemeSpanish = "mandala";
            sceneData.paintingStyle = PaintingStyle.COLORBOOK;
        }

        //Cambio el prompt para la generación de imágenes
        if (sceneData.paintingStyle == PaintingStyle.COLORBOOK)
        {
            string newPrompt = $"Create a simple colorbook page of a {sceneData.sceneThemeEnglish} with black lines that are fully closed";
            GetComponent<ImageGenerator>().prompt = newPrompt;
        }
        else if (sceneData.paintingStyle == PaintingStyle.ABSTRACT)
        {
            string newPrompt = $"Create a simple abstract page of a {sceneData.sceneThemeEnglish} with black lines that are fully closed";
            GetComponent<ImageGenerator>().prompt = newPrompt;
        }
        else
        {
            string newPrompt = $"Create a simple cubist page of a {sceneData.sceneThemeEnglish} with black lines that are fully closed";
            GetComponent<ImageGenerator>().prompt = newPrompt;
        }

        botonCrear.text = $"CREAR {sceneData.sceneThemeSpanish.ToUpper()}";
    }
}
