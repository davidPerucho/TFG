using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Text;
using System;

/// <summary>
/// Clase que se encarga de la funcionalidad a la hora de generar imágenes.
/// </summary>
public class ImageGenerator : MonoBehaviour
{
    string baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/imagen-3.0-generate-002:predict?key="; //Ruta de la api para generar imágenes
    string apiKey = "AIzaSyCt94fTBRR6J-kO3XHo8WkC8aAGKIyqedI"; //Clave API
    public string prompt; //Prompt utilizado para generar la imagen

    public int width; //Ancho deseado para la imagen generada
    public int height; //Alto deseado para la imagen generada

    [HideInInspector]
    public bool loading = false; //True cuando se está creando una nueva imagen

    [SerializeField]
    Button generateImageButton; //Botón encargado de activar la generación de imágenes

    string imagesDirectory; //Ruta al directorio en el que se encuentran las imágenes generadas
    int seed = 0; //Semilla utilizada para generar la imagen

    void Start()
    {
        UIManager.Instance.AddListenerToButton("ButtonGenerateImage", GenerateAndLoad);
        
        seed = UnityEngine.Random.Range(0, int.MaxValue);

        if (GetComponent<DynamicPainting>() == null || GetComponent<DynamicPainting>().sceneData.sceneThemeEnglish == "mandala")
        {
            imagesDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");
        }
        else
        {
            imagesDirectory = Path.Combine(Application.persistentDataPath, $"{GetComponent<DynamicPainting>().sceneData.sceneName}Images");
        }

        //Comprobar que el directorio existe
        if (!Directory.Exists(imagesDirectory))
        {
            Directory.CreateDirectory(imagesDirectory);
            Debug.Log($"El directorio no existía, pero se ha creado: {imagesDirectory}");
        }
    }

    /// <summary>
    /// Rutina encargada de llamar a la api y gestionar la generación de la imagen.
    /// </summary>
    IEnumerator GenerateAndSaveImage()
    {
        //Genero una semilla aleatoria
        seed = UnityEngine.Random.Range(0, int.MaxValue);

        //Desactivo los botones
        UIManager.Instance.DisableButton("ButtonGenerateImage");
        UIManager.Instance.DisableButton("ButtonListLeft");
        UIManager.Instance.DisableButton("ButtonListRight");
        UIManager.Instance.DisableButton("ButtonExit");

        //Construyo la URL con sus parámetros
        string url = baseUrl + apiKey;

        //Creo el json con el prompt
        string jsonData = "{\"instances\":[{\"prompt\":\"" + prompt + "\"}],\"parameters\":{\"sampleCount\":1}}";
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); //Espero la respuesta

            if (request.result != UnityWebRequest.Result.Success)
            {
                //Si hay un error se muestra con un texto rojo por pantalla
                loading = false;
                UIManager.Instance.SetText("TextImageGeneration", $"Error al generar {GetComponent<DynamicPainting>().sceneData.sceneThemeSpanish.ToLower()}, comprueba la conexión a internet.");
                UIManager.Instance.SetTextColor("TextImageGeneration", Color.red);
                Debug.LogError($"Error al generar la imagen: {request.error}");
            }
            else
            {
                //Obtengo la imagen de la respuesta en base64
                string responseText = request.downloadHandler.text;
                string image64 = "";
                string filePath = Path.Combine(imagesDirectory, $"Image_{seed}.png");

                try
                {
                    var json = JsonUtility.FromJson<ImageResponseWrapper>(responseText);
                    image64 = json.predictions[0].bytesBase64Encoded;
                }
                catch (Exception e)
                {
                    Debug.LogError("Error extrallendo la imagen de la respuesta: " + e.Message);
                }

                //Guardo la imagen como png
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(image64);
                    File.WriteAllBytes(filePath, imageBytes);
                    Debug.Log($"Imagen guardada en: {filePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error al guardar la imagen: " + e.Message);
                }

                //Vuelvo a activar los botones
                loading = false;
                UIManager.Instance.SetText("TextImageGeneration", $"{GetComponent<DynamicPainting>().sceneData.sceneThemeSpanish} generado correctamente.");
                UIManager.Instance.SetTextColor("TextImageGeneration", Color.green);

                UIManager.Instance.EnableButton("ButtonGenerateImage");
                UIManager.Instance.EnableButton("ButtonListLeft");
                UIManager.Instance.EnableButton("ButtonListRight");
                UIManager.Instance.EnableButton("ButtonExit");

                //Vuelvo a cargar la lista
                FindAnyObjectByType<ShowImages>().ReloadImages();
            }
        }
    }

    /// <summary>
    /// Fuencion encargada de llamar a la rutina para generar la imagen.
    /// </summary>
    void GenerateAndLoad()
    {
        //Desactivo otras funcionalidades mientras se genera la imagen
        loading = true;
        UIManager.Instance.EnableObject("TextImageGeneration");

        if (GetComponent<DynamicPainting>() != null)
        {
            UIManager.Instance.SetText("TextImageGeneration", $"Generando {GetComponent<DynamicPainting>().sceneData.sceneThemeSpanish}...");
        }

        //Genero la imagen
        StartCoroutine(GenerateAndSaveImage());
    }

    //Clases para extraer la imagen del json
    [Serializable]
    private class ImageResponseWrapper
    {
        public ImageResponse[] predictions;
    }

    [Serializable]
    private class ImageResponse
    {
        public string bytesBase64Encoded;
    }
}