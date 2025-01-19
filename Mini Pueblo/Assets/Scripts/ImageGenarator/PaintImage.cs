using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que se encarga de manejar los elementos destinados a pintar las imágenes.
/// </summary>
public class PaintImage : MonoBehaviour
{
    string savePath; //Ruta en la que se guardarán las imágenes coloreadas
    string filePath; //Ruta de la imágen que se va a pintar
    Texture2D imageTexture; //Textura de la imagen que se va a pintar
    Color fillColor = Color.white; //Color seleccionado para colorear
    bool pressed = false; //Es true cuando el bo´tón izquierdo del ratón está presionado o cuando se está tocando la pantalla

    [SerializeField]
    RawImage image; //Elemento utilizado para mostrar la imagen que se está pintando

    [SerializeField]
    RawImage colorWheel; //Elemento utilizado para mostrar el selector de color

    [SerializeField]
    RawImage colorOutput; //Elemento utilizado para mostrar el color seleccionado

    [SerializeField]
    GameObject cursor; //Cursor para la selección de color

    [SerializeField]
    int colorSelectorWidth; //Ancho del selector de color

    [SerializeField]
    int colorSelectorHeight; //Alto del selector de color

    // Start is called before the first frame update
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "GeneratedImages");
        UIManager.Instance.AddListenerToButton("ButtonSave", Save);
    }

    void Update()
    {
        //Comprubo si el botón izquierdo del ratón está siendo presionado
        if (Input.GetMouseButtonDown(0))
        {
            pressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
        }

        //Si se esta presionando el boton del ratón y la imagen esta cargada pintamos en la imagen o seleccionamos un color
        if (pressed && image.gameObject.activeSelf && colorWheel.gameObject.activeSelf && colorOutput.gameObject.activeSelf)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, Input.mousePosition, null, out localCursor);

            if (IsInsideImage(localCursor))
            {
                //Si se hace click sobre la imagen pintamos la zona correspondiente
                Vector2Int pixel = GetPixelFromCursor(localCursor, imageTexture, image.rectTransform.rect);

                FloodFill(pixel.x, pixel.y);
                imageTexture.Apply();
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(colorWheel.rectTransform, Input.mousePosition, null, out localCursor);

                if (IsInsideColorWheel(localCursor))
                {
                    //Si se pulsa sobre el selector de color movemos el cursor y cambiamos el color seleccionado
                    cursor.SetActive(true);
                    cursor.transform.position = colorWheel.rectTransform.TransformPoint(localCursor);

                    Texture2D textureColorWheel = colorWheel.texture as Texture2D;
                    Vector2Int pixel = GetPixelFromCursor(localCursor, textureColorWheel, colorWheel.rectTransform.rect);
                    fillColor = textureColorWheel.GetPixel(pixel.x, pixel.y);

                    //Muestra por pantalla el color seleccionado
                    Texture2D colorTexture = new Texture2D(1, 1);
                    colorTexture.SetPixel(0, 0, fillColor);
                    colorTexture.Apply();
                    colorOutput.texture = colorTexture;
                }
            }
        }
    }

    /// <summary>
    /// Muestra la imagen que esta siendo coloreada por pantalla así como el selector de color.
    /// </summary>
    /// <param name="imagePath">Ruta de la imagen.</param>
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

        CreateColorWheel();
    }

    /// <summary>
    /// Comprueba si el cursor del raton se encuentra dentro de la imagen.
    /// </summary>
    /// <param name="localCursor">Posicion del cursor.</param>
    /// <returns>True si el cursor se encuentra dentro de la imagen.</returns>
    private bool IsInsideImage(Vector2 localCursor)
    {
        Rect rect = image.rectTransform.rect;
        return rect.Contains(localCursor);
    }

    /// <summary>
    /// Comprueba si el cursor del raton se encuentra dentro del selector de color.
    /// </summary>
    /// <param name="localCursor">Posicion del cursor.</param>
    /// <returns>True si el cursor se encuentra dentro del selector de color.</returns>
    private bool IsInsideColorWheel(Vector2 localCursor)
    {
        Rect rect = colorWheel.rectTransform.rect;
        return rect.Contains(localCursor);
    }

    /// <summary>
    /// Devuelve el pixel dentro de la textura en el que se encuetra el cursor del mouse.
    /// </summary>
    /// <param name="localCursor">Cursor del mouse.</param>
    /// <param name="texture">Textura de la imagen.</param>
    /// <param name="rect">Tamaño de la imagen.</param>
    /// <returns>El pixel de la textura el que se encuentra el cursor.</returns>
    private Vector2Int GetPixelFromCursor(Vector2 localCursor, Texture2D texture, Rect rect)
    {
        Vector2 normalized = new Vector2((localCursor.x - rect.xMin) / rect.width, (localCursor.y - rect.yMin) / rect.height);

        return new Vector2Int(Mathf.RoundToInt(normalized.x * texture.width), Mathf.RoundToInt(normalized.y * texture.height));
    }

    /// <summary>
    /// Rellena los pixeles colindantes a una posición determinada con el color seleccionado.
    /// </summary>
    /// <param name="startX">Coordenada X del pixel de inicio.</param>
    /// <param name="startY">Coordenada Y del pixel de inicio.</param>
    void FloodFill(int startX, int startY)
    {
        //Utilizo un stack para almacenar los pixeles que tienen que cambiar de color
        Stack<Vector2Int> pixels = new Stack<Vector2Int>();
        pixels.Push(new Vector2Int(startX, startY));

        while (pixels.Count > 0)
        {
            Vector2Int currentPixel = pixels.Pop();
            int x = currentPixel.x;
            int y = currentPixel.y;

            //Compruebo los límites de la textura
            if (x < 0 || x >= imageTexture.width || y < 0 || y >= imageTexture.height) continue;

            //Obtengo el color actual del píxel
            Color currentColor = imageTexture.GetPixel(x, y);

            //Si el color es negro o muy oscuro, o si ya es del color objetivo detengo el proceso
            if (currentColor == Color.black || IsDarkColor(currentColor) || currentColor == fillColor) continue;

            //Relleno el píxel con el color deseado
            imageTexture.SetPixel(x, y, fillColor);

            //Agrego los píxeles vecinos a la pila
            pixels.Push(new Vector2Int(x + 1, y));
            pixels.Push(new Vector2Int(x - 1, y));
            pixels.Push(new Vector2Int(x, y + 1));
            pixels.Push(new Vector2Int(x, y - 1));
        }

        //Aplica las modificaciones de color a la textura de la imagen
        imageTexture.Apply();
    }

    /// <summary>
    /// Colcula si el brillo de un color dado es superior a 0.25.
    /// </summary>
    /// <param name="color">Color que se quiere comprobar.</param>
    /// <returns>True si el brillo del color es superior a 0.25</returns>
    bool IsDarkColor(Color color)
    {
        float brightness = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        
        return brightness < 0.25f;
    }

    /// <summary>
    /// Muestra el selector de color por pantalla.
    /// </summary>
    void CreateColorWheel()
    {
        Texture2D colorWheelTexture = new Texture2D(colorSelectorWidth, colorSelectorHeight);
        colorWheelTexture.wrapMode = TextureWrapMode.Clamp;

        //Genero los colores del selector usando HSV
        for (int y = 0; y < colorSelectorHeight; y++)
        {
            for (int x = 0; x < colorSelectorWidth; x++)
            {
                //Convierto las coordenadas de los pixeles en valores HUE
                float hue = (float)x / colorSelectorWidth;
                float saturation = (float)y / colorSelectorHeight; 
                float value = 1.0f; 

                //Creo el color basado en los valores HUE
                Color color = Color.HSVToRGB(hue, saturation, value);

                colorWheelTexture.SetPixel(x, y, color);
            }
        }

        //Aplica la textura al selector de color
        colorWheelTexture.Apply();
        colorWheel.texture = colorWheelTexture;
    }

    /// <summary>
    /// Elimina la imagen anterior, guarda la imagen con los cambios y vuelve a la lista de imágenes.
    /// </summary>
    void Save()
    {
        //Elimino la imagen
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
        UIManager.Instance.EnableObject("ButtonListRight");
        UIManager.Instance.EnableObject("ButtonListLeft");
        UIManager.Instance.EnableObject("ButtonGenerateImage");
        UIManager.Instance.EnableObject("ButtonExit");
        UIManager.Instance.EnableObject("TextImageGenerator");
        UIManager.Instance.EnableObject("TextTutorial");

        //Desactivo los elementos UI del selector de imagen
        UIManager.Instance.DisableObject("ButtonSave");
        UIManager.Instance.DisableObject("TextColor");

        //Desactivo los elementos UI
        image.gameObject.SetActive(false);
        colorWheel.gameObject.SetActive(false);
        colorOutput.gameObject.SetActive(false);
        cursor.SetActive(false);

        //Inicio la funcionalidad del selector de imagen
        FindAnyObjectByType<ShowImages>().ReloadImages();
    }
}
