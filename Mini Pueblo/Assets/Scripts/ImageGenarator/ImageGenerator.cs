using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageGenerator : MonoBehaviour
{
    // Parámetros de la API
    public string baseUrl = "https://image.pollinations.ai/prompt/";
    public string prompt = "A simple colorbook page of a landscape without color";
    public int width = 512;
    public int height = 512;
    public bool nologo = true; // No incluir el logo
    string imagesDirectory;
    int seed = 0;

    void Start()
    {
        seed = Random.Range(0, int.MaxValue);
        imagesDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");

        //Comprobar que el directorio existe
        if (Directory.Exists(imagesDirectory))
        {
            StartCoroutine(GenerateAndSaveImage(prompt));
        }
        else
        {
            Debug.LogError("No existe el directorio en el que se pretende guardar la imagen");
        }
    }

    IEnumerator GenerateAndSaveImage(string prompt)
    {
        // Construir la URL con los parámetros
        string apiUrl = $"{baseUrl}{UnityWebRequest.EscapeURL(prompt)}?width={width}&height={height}&nologo={(nologo ? 1 : 0)}&seed={seed}";

        // Realizar la solicitud GET
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
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

            Debug.Log($"Imagen guardada en: {filePath}");
        }
    }
}