using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase encargada de cargar los datos para las escenas de pintura.
/// </summary>
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
            sceneData.paintingSceneType = PaintingSceneType.NORMAL;
        }

        //Cambio el prompt para la generación de imágenes en función de los datos de la escena
        if (sceneData.paintingSceneType == PaintingSceneType.NORMAL)
        {
            if (sceneData.paintingStyle == PaintingStyle.COLORBOOK)
            {
                string newPrompt = $"Create a simple colorbook page of a {sceneData.sceneThemeEnglish} in black and white with black lines that are fully closed";
                GetComponent<ImageGenerator>().prompt = newPrompt;
            }
            else if (sceneData.paintingStyle == PaintingStyle.ABSTRACT)
            {
                string newPrompt = $"Create a simple abstract page of a {sceneData.sceneThemeEnglish} in black and white with black lines that are fully closed";
                GetComponent<ImageGenerator>().prompt = newPrompt;
            }
            else
            {
                string newPrompt = $"Create a simple cubist page of a {sceneData.sceneThemeEnglish} in black and white with black lines that are fully closed";
                GetComponent<ImageGenerator>().prompt = newPrompt;
            }
        }
        else
        {
            if (sceneData.paintingStyle == PaintingStyle.COLORBOOK)
            {
                string newPrompt = $"Generate a colorbook picture of a {sceneData.sceneThemeEnglish} in black and white with black lines that are fully closed and with little numbers from one to ten inside the center of each white region";
                GetComponent<ImageGenerator>().prompt = newPrompt;
            }
            else if (sceneData.paintingStyle == PaintingStyle.ABSTRACT)
            {
                string newPrompt = $"Generate a abstract picture of a {sceneData.sceneThemeEnglish} in black and white with black lines that are fully closed and with little numbers from one to ten inside the center of each white region";
                GetComponent<ImageGenerator>().prompt = newPrompt;
            }
            else
            {
                string newPrompt = $"Generate a cubist picture of a {sceneData.sceneThemeEnglish} in black and white with black lines that are fully closed and with little numbers from one to ten inside the center of each white region";
                GetComponent<ImageGenerator>().prompt = newPrompt;
            }
        }

        botonCrear.text = $"CREAR {sceneData.sceneThemeSpanish.ToUpper()}";
    }
}
