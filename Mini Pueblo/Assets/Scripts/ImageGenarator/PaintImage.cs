using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PaintImage : MonoBehaviour
{
    string savePath;
    string filePath;
    Texture2D imageTexture;
    Color fillColor;
    Color currentColor = Color.white;

    [SerializeField]
    RawImage image;

    [SerializeField]
    RawImage colorWheel;

    [SerializeField]
    RawImage colorOutput;

    [SerializeField]
    GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "GeneratedImages");
        UIManager.Instance.AddListenerToButton("ButtonSave", save);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && image.gameObject.activeSelf && colorWheel.gameObject.activeSelf && colorOutput.gameObject.activeSelf)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, Input.mousePosition, null, out localCursor);

            if (isInsideImage(localCursor))
            {
                Vector2Int pixel = GetPixelFromCursor(localCursor, imageTexture, image.rectTransform.rect);
                Color selectedColor = imageTexture.GetPixel(pixel.x, pixel.y);

                fillColor = currentColor;
                FloodFill(pixel.x, pixel.y);
                imageTexture.Apply();
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(colorWheel.rectTransform, Input.mousePosition, null, out localCursor);

                if (isInsideColorWheel(localCursor))
                {
                    cursor.SetActive(true);
                    cursor.transform.position = colorWheel.rectTransform.TransformPoint(localCursor);

                    Texture2D textureColorWheel = colorWheel.texture as Texture2D;
                    Vector2Int pixel = GetPixelFromCursor(localCursor, textureColorWheel, colorWheel.rectTransform.rect);
                    currentColor = textureColorWheel.GetPixel(pixel.x, pixel.y);

                    // Crear una textura de 1x1 píxeles
                    Texture2D colorTexture = new Texture2D(1, 1);
                    colorTexture.SetPixel(0, 0, currentColor);
                    colorTexture.Apply();

                    // Asignar la textura al RawImage de colorOutput
                    colorOutput.texture = colorTexture;
                }
            }
        }
    }

    public void DisplayPaintImage(string imagePath)
    {
        filePath = imagePath;
        image.gameObject.SetActive(true);
        colorWheel.gameObject.SetActive(true);
        colorOutput.gameObject.SetActive(true);

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        imageTexture = new Texture2D(2, 2);
        imageTexture.LoadImage(imageBytes);
        image.texture = imageTexture;

        createColorWheel();
    }

    // Comprueba si el clic está dentro de la imagen
    private bool isInsideImage(Vector2 localCursor)
    {
        Rect rect = image.rectTransform.rect;
        return rect.Contains(localCursor);
    }

    // Comprueba si el clic está dentro del selector de color
    private bool isInsideColorWheel(Vector2 localCursor)
    {
        Rect rect = colorWheel.rectTransform.rect;
        return rect.Contains(localCursor);
    }

    // Convierte la posición del mouse en coordenadas de píxel
    private Vector2Int GetPixelFromCursor(Vector2 localCursor, Texture2D texture, Rect rect)
    {
        Vector2 normalized = new Vector2((localCursor.x - rect.xMin) / rect.width, (localCursor.y - rect.yMin) / rect.height);

        return new Vector2Int(Mathf.RoundToInt(normalized.x * texture.width), Mathf.RoundToInt(normalized.y * texture.height));
    }

    void FloodFill(int startX, int startY)
    {
        Stack<Vector2Int> pixels = new Stack<Vector2Int>();
        pixels.Push(new Vector2Int(startX, startY));

        while (pixels.Count > 0)
        {
            Vector2Int currentPixel = pixels.Pop();
            int x = currentPixel.x;
            int y = currentPixel.y;

            // Verifica los límites de la textura
            if (x < 0 || x >= imageTexture.width || y < 0 || y >= imageTexture.height) continue;

            // Obtén el color actual del píxel
            Color currentColor = imageTexture.GetPixel(x, y);

            // Detén si el color es negro o muy oscuro, o si ya es del color objetivo
            if (currentColor == Color.black || isDarkColor(currentColor) || currentColor == fillColor) continue;

            // Rellena el píxel con el color deseado
            imageTexture.SetPixel(x, y, fillColor);

            // Agrega los píxeles vecinos a la pila
            pixels.Push(new Vector2Int(x + 1, y));
            pixels.Push(new Vector2Int(x - 1, y));
            pixels.Push(new Vector2Int(x, y + 1));
            pixels.Push(new Vector2Int(x, y - 1));
        }

        // Aplica los cambios a la textura después de terminar
        imageTexture.Apply();
    }

    bool isDarkColor(Color color)
    {
        // Calcula el brillo del color (usando la fórmula de luminosidad perceptiva)
        float brightness = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;

        // Devuelve verdadero si el brillo es menor a un umbral (por ejemplo, 0.2)
        return brightness < 0.4f;
    }

    void createColorWheel()
    {
        int width = 256;  // Ancho de la rueda/paleta
        int height = 256; // Alto de la rueda/paleta
        Texture2D colorWheelTexture = new Texture2D(width, height);
        colorWheelTexture.wrapMode = TextureWrapMode.Clamp;

        // Generar colores usando HSV (Hue, Saturation, Value)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Convertir coordenadas a valores de Hue (tono) y Saturation (saturación)
                float hue = (float)x / width;          // Rango de 0 a 1 para el tono
                float saturation = (float)y / height; // Rango de 0 a 1 para la saturación
                float value = 1.0f;                   // Fijar el valor en 1 (máxima intensidad)

                // Crear color basado en HSV
                Color color = Color.HSVToRGB(hue, saturation, value);

                // Establecer el píxel en la textura
                colorWheelTexture.SetPixel(x, y, color);
            }
        }

        // Aplicar cambios a la textura
        colorWheelTexture.Apply();

        // Asignar la textura a la rueda de colores
        colorWheel.texture = colorWheelTexture;
    }

    void save()
    {
        try
        {
            File.Delete(filePath);
            Debug.Log($"Imagen eliminada correctamente: {filePath}");
        }
        catch (IOException ioEx)
        {
            Debug.LogError($"Error al intentar eliminar la imagen: {ioEx.Message}");
        }

        //Guardo la imagen añadiendo la fecha en el nombre
        byte[] bytes = imageTexture.EncodeToPNG();
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        File.WriteAllBytes(Path.Combine(savePath, $"Coloreada_{timestamp}.png"), bytes);
        Debug.Log($"Imagen guardada en {savePath}");

        //Vuelvo a la lista de imagenes
        UIManager.Instance.enableObject("ButtonListRight");
        UIManager.Instance.enableObject("ButtonListLeft");
        UIManager.Instance.enableObject("ButtonGenerateImage");

        //Desactivo los elementos UI del selector de imagen
        UIManager.Instance.disableObject("ButtonSave");
        UIManager.Instance.disableObject("TextColor");

        //Desactivo los elementos UI
        image.gameObject.SetActive(false);
        colorWheel.gameObject.SetActive(false);
        colorOutput.gameObject.SetActive(false);
        cursor.SetActive(false);

        //Inicio la funcionalidad del selector de imagen
        FindAnyObjectByType<ShowImages>().reloadImages();
    }
}
