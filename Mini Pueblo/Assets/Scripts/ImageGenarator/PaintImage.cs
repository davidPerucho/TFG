using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PaintImage : MonoBehaviour
{
    string savePath;
    string filePath;
    Texture2D texture;
    Color fillColor;

    [SerializeField]
    RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "GeneratedImages");
        UIManager.Instance.AddListenerToButton("ButtonSave", save);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && image.gameObject.activeSelf)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                image.rectTransform,
                Input.mousePosition,
                null,
                out localCursor);

            if (IsInsideImage(localCursor))
            {
                Vector2Int pixel = GetPixelFromCursor(localCursor);
                Color selectedColor = texture.GetPixel(pixel.x, pixel.y);

                fillColor = Color.red;
                FloodFill(pixel.x, pixel.y);
                texture.Apply();
            }
        }
    }

    public void DisplayPaintImage(string imagePath)
    {
        filePath = imagePath;
        image.gameObject.SetActive(true);

        byte[] imageBytes = File.ReadAllBytes(imagePath);
        texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        image.texture = texture;
    }

    // Comprueba si el clic está dentro de la imagen
    private bool IsInsideImage(Vector2 localCursor)
    {
        Rect rect = image.rectTransform.rect;
        return rect.Contains(localCursor);
    }

    // Convierte la posición del mouse en coordenadas de píxel
    private Vector2Int GetPixelFromCursor(Vector2 localCursor)
    {
        Rect rect = image.rectTransform.rect;
        Vector2 normalized = new Vector2(
            (localCursor.x - rect.xMin) / rect.width,
            (localCursor.y - rect.yMin) / rect.height);
        return new Vector2Int(
            Mathf.RoundToInt(normalized.x * texture.width),
            Mathf.RoundToInt(normalized.y * texture.height));
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
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height) continue;

            // Obtén el color actual del píxel
            Color currentColor = texture.GetPixel(x, y);

            // Detén si el color es negro o muy oscuro, o si ya es del color objetivo
            if (currentColor == Color.black || isDarkColor(currentColor) || currentColor == fillColor) continue;

            // Rellena el píxel con el color deseado
            texture.SetPixel(x, y, fillColor);

            // Agrega los píxeles vecinos a la pila
            pixels.Push(new Vector2Int(x + 1, y));
            pixels.Push(new Vector2Int(x - 1, y));
            pixels.Push(new Vector2Int(x, y + 1));
            pixels.Push(new Vector2Int(x, y - 1));
        }

        // Aplica los cambios a la textura después de terminar
        texture.Apply();
    }

    bool isDarkColor(Color color)
    {
        // Calcula el brillo del color (usando la fórmula de luminosidad perceptiva)
        float brightness = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;

        // Devuelve verdadero si el brillo es menor a un umbral (por ejemplo, 0.2)
        return brightness < 0.2f;
    }

    bool isBrightColor(Color color)
    {
        // Calcula el brillo del color (usando la fórmula de luminosidad perceptiva)
        float brightness = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;

        // Devuelve verdadero si el brillo es menor a un umbral (por ejemplo, 0.2)
        return brightness > 0.2f;
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

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(savePath, $"Coloreada_0.png"), bytes);
        Debug.Log($"Imagen guardada en {savePath}");

        //Vuelvo a la lista de imagenes
        //Activo los elementos UI de la lista
        UIManager.Instance.enableObject("ButtonListRight");
        UIManager.Instance.enableObject("ButtonListLeft");
        UIManager.Instance.enableObject("ButtonGenerateImage");

        //Desactivo los elementos UI del selector de imagen
        UIManager.Instance.disableObject("ButtonSave");

        //Elimino la imagen de la vista
        image.gameObject.SetActive(false);

        //Inicio la funcionalidad del selector de imagen
        FindAnyObjectByType<ShowImages>().reloadImages();
    }
}
