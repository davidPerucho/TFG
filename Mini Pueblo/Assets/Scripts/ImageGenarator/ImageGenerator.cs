using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Clase que se encarga de la funcionalidad a la hora de generar imágenes.
/// </summary>
public class ImageGenerator : MonoBehaviour
{
    // Parámetros de la API
    [SerializeField]
    string baseUrl = "https://image.pollinations.ai/prompt/"; //Ruta de la api

    [SerializeField]
    string prompt; //Prompt utilizado para generar la imagen

    public int width; //Ancho deseado para la imagen generada
    public int height; //Alto deseado para la imagen generada

    [HideInInspector]
    public bool loading = false; //True cuando se está creando una nueva imagen

    [SerializeField]
    bool noLogo = true; //True cuando no se quiere que el logo de la IA aparezca en la imagen

    [SerializeField]
    Button generateImageButton; //Botón encargado de activar la generación de imágenes

    string imagesDirectory; //Ruta al directorio en el que se encuentran las imágenes generadas
    int seed = 0; //Semilla utilizada para generar la imagen

    void Start()
    {
        UIManager.Instance.AddListenerToButton("ButtonGenerateImage", GenerateAndLoad);
        seed = Random.Range(0, int.MaxValue);
        imagesDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");

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
        seed = Random.Range(0, int.MaxValue);

        //Desactivo los botones
        UIManager.Instance.DisableButton("ButtonGenerateImage");
        UIManager.Instance.DisableButton("ButtonListLeft");
        UIManager.Instance.DisableButton("ButtonListRight");

        //Construyo la URL con sus parámetros
        string apiUrl = $"{baseUrl}{UnityWebRequest.EscapeURL(prompt)}?width={width}&height={height}&nologo={(noLogo ? 1 : 0)}&seed={seed}";

        //Realizo la solicitud GET
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl);

        yield return request.SendWebRequest(); //Espero la respuesta

        if (request.result != UnityWebRequest.Result.Success)
        {
            //Si hay un error se muestra con un texto rojo por pantalla
            loading = false;
            UIManager.Instance.SetText("TextImageGeneration", "Error al generar el mandala, comprueba la conexión a internet.");
            UIManager.Instance.SetTextColor("TextImageGeneration", Color.red);
            Debug.LogError($"Error al generar la imagen: {request.error}");
        }
        else
        {
            //Obtengo la textura de la respuesta
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            //Convierto la textura en bytes PNG
            byte[] imageData = texture.EncodeToPNG();

            string filePath = Path.Combine(imagesDirectory, $"Image_{seed}.png");

            //Guardo la imagen en el disco
            File.WriteAllBytes(filePath, imageData);

            //Vuelvo a activar los botones
            loading = false;
            UIManager.Instance.SetText("TextImageGeneration", "Mandala generado correctamente.");
            UIManager.Instance.SetTextColor("TextImageGeneration", Color.green);

            UIManager.Instance.EnableButton("ButtonGenerateImage");
            UIManager.Instance.EnableButton("ButtonListLeft");
            UIManager.Instance.EnableButton("ButtonListRight");

            //Vuelvo a cargar la lista
            FindAnyObjectByType<ShowImages>().ReloadImages();

            Debug.Log($"Imagen guardada en: {filePath}");
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
        UIManager.Instance.SetText("TextImageGeneration", "Generando mandala...");

        //Genero la imagen
        StartCoroutine(GenerateAndSaveImage());
    }
}