using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ImageGenerator : MonoBehaviour
{
    // Parámetros de la API
    static int numImage = 0;
    public string baseUrl = "https://image.pollinations.ai/prompt/";
    public string prompt = "A simple colorbook page of a landscape without color";
    public int width = 512;
    public int height = 512;
    public bool nologo = true; // No incluir el logo
    int seed = 0;

    void Start()
    {
        seed = Random.Range(0, int.MaxValue);
        StartCoroutine(GenerateAndSaveImage(prompt));
    }

    IEnumerator GenerateAndSaveImage(string prompt)
    {
        // Construir la URL con los parámetros
        string apiUrl = $"{baseUrl}{UnityWebRequest.EscapeURL(prompt)}?width={width}&height={height}&nologo={(nologo ? 1 : 0)}&seed={seed}";

        // Realizar la solicitud GET
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(apiUrl);

        // Esperar la respuesta
        yield return request.SendWebRequest();

        // Manejar errores
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

            string filePath = "C:\\Users\\david\\TFG\\Mini Pueblo\\Assets\\GeneratedImages\\Image"+seed.ToString()+".png";
            numImage++;

            // Guardar la imagen en el disco
            File.WriteAllBytes(filePath, imageData);

            Debug.Log($"Imagen guardada en: {filePath}");
        }
    }
}