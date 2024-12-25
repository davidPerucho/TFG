using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ImageGenerator : MonoBehaviour
{
    // Parámetros de la API
    [SerializeField]
    string baseUrl = "https://image.pollinations.ai/prompt/";

    [SerializeField]
    string prompt = "A simple colorbook page of a landscape without color";

    public int width = 512;
    public int height = 512;

    [HideInInspector]
    public bool loading = false;

    [SerializeField]
    bool noLogo = true;

    [SerializeField]
    Button generateImageButton;

    string imagesDirectory;
    int seed = 0;

    void Start()
    {
        UIManager.Instance.AddListenerToButton("ButtonGenerateImage", generateAndLoad);
        seed = Random.Range(0, int.MaxValue);
        imagesDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");

        //Comprobar que el directorio existe
        if (!Directory.Exists(imagesDirectory))
        {
            Directory.CreateDirectory(imagesDirectory);
            Debug.Log($"El directorio no existía, pero se ha creado: {imagesDirectory}");
        }
    }

    IEnumerator GenerateAndSaveImage()
    {
        //Genero una semilla aleatoria
        seed = Random.Range(0, int.MaxValue);

        //Desactivo los botones
        UIManager.Instance.disableButton("ButtonGenerateImage");
        UIManager.Instance.disableButton("ButtonListLeft");
        UIManager.Instance.disableButton("ButtonListRight");

        // Construir la URL con los parámetros
        string apiUrl = $"{baseUrl}{UnityWebRequest.EscapeURL(prompt)}?width={width}&height={height}&nologo={(noLogo ? 1 : 0)}&seed={seed}";

        // Realizar la solicitud GET
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            loading = false;
            UIManager.Instance.setText("TextImageGeneration", "Error al generar el mandala, comprueba la conexión a internet.");
            UIManager.Instance.setTextColor("TextImageGeneration", Color.red);
            Debug.LogError($"Error al generar la imagen: {request.error}");
        }
        else
        {
            // Obtener la textura de la respuesta
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Convertir la textura en bytes PNG
            byte[] imageData = texture.EncodeToPNG();

            string filePath = Path.Combine(imagesDirectory, $"Image_{seed}.png");

            // Guardar la imagen en el disco
            File.WriteAllBytes(filePath, imageData);

            //Vuelvo a activar los botones
            loading = false;
            UIManager.Instance.setText("TextImageGeneration", "Mandala generado correctamente.");
            UIManager.Instance.setTextColor("TextImageGeneration", Color.green);

            UIManager.Instance.enableButton("ButtonGenerateImage");
            UIManager.Instance.enableButton("ButtonListLeft");
            UIManager.Instance.enableButton("ButtonListRight");

            //Vuelvo a cargar la lista
            FindAnyObjectByType<ShowImages>().reloadImages();

            Debug.Log($"Imagen guardada en: {filePath}");
        }
    }

    void generateAndLoad()
    {
        loading = true;
        UIManager.Instance.enableObject("TextImageGeneration");
        UIManager.Instance.setText("TextImageGeneration", "");

        //Genero la imagen
        StartCoroutine(GenerateAndSaveImage());
    }
}