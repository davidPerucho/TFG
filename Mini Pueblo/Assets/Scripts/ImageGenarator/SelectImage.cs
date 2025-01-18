using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Clase que se encarga de manejar los elementos del selector de imágenes.
/// </summary>
public class SelectImage : MonoBehaviour
{
    string filePath;
    GameObject imageObject; //Objeto que contiene la imagen actual

    [SerializeField]
    GameObject imagePrefab; //Prefab utilizado para crear objetos con la imagen actual

    // Start is called before the first frame update
    void Start()
    {
        //Añado las acciones a sus respectivos botones
        UIManager.Instance.AddListenerToButton("ButtonReturn", Return);
        UIManager.Instance.AddListenerToButton("ButtonColor", PaintImage);
        UIManager.Instance.AddListenerToButton("ButtonDelete", DeleteImage);
    }

    /// <summary>
    /// Muestra la imagen por pantalla.
    /// </summary>
    /// <param name="imagePath">Ruta de la imagen.</param>
    public void DisplaySingleImage(string imagePath)
    {
        filePath = imagePath;

        //Obtengo el Canvas donde se agrupan los objetos de la UI
        Transform canvasTransform = null;
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvasTransform = canvas.transform;
        }
        else
        {
            Debug.LogError("No se encontró un Canvas en la escena.");
        }

        //Cargar la imagen desde el archivo
        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        //Crear un nuevo Sprite a partir de la textura
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        //Instanciar el prefab y configurarlo
        imageObject = Instantiate(imagePrefab);
        imageObject.transform.SetParent(canvasTransform, false);
        Image uiImage = imageObject.GetComponent<Image>();
        if (uiImage != null)
        {
            uiImage.sprite = sprite;
            uiImage.preserveAspect = true;
        }
        else
        {
            Debug.LogError("No se ha encontrado el componente Image en el prefab de la imagen.");
        }
    }

    /// <summary>
    /// Elimina la imagen seleccionada.
    /// </summary>
    void DeleteImage()
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

        Return();
    }

    /// <summary>
    /// Mustra la vista para pintar la imagen.
    /// </summary>
    void PaintImage()
    {
        //Activo los elementos UI para pintar
        UIManager.Instance.EnableObject("ButtonSave");
        UIManager.Instance.EnableObject("TextColor");

        //Desactivo los elementos UI del selector de imagen
        UIManager.Instance.DisableObject("ButtonReturn");
        UIManager.Instance.DisableObject("ButtonColor");
        UIManager.Instance.DisableObject("ButtonDelete");

        //Elimino la imagen de la vista
        Destroy(imageObject);

        //Iniciao la funcionalidad de pintar
        FindAnyObjectByType<PaintImage>().DisplayPaintImage(filePath);
    }

    /// <summary>
    /// Vuelve a la lista de imágenes.
    /// </summary>
    void Return()
    {
        //Activo los elementos UI de la lista
        UIManager.Instance.EnableObject("ButtonListRight");
        UIManager.Instance.EnableObject("ButtonListLeft");
        UIManager.Instance.EnableObject("ButtonGenerateImage");
        UIManager.Instance.EnableObject("ButtonExit");
        UIManager.Instance.EnableObject("TextImageGenerator");
        UIManager.Instance.EnableObject("TextTutorial");

        //Desactivo los elementos UI del selector de imagen
        UIManager.Instance.DisableObject("ButtonReturn");
        UIManager.Instance.DisableObject("ButtonColor");
        UIManager.Instance.DisableObject("ButtonDelete");

        //Elimino la imagen de la vista
        Destroy(imageObject);

        //Inicio la funcionalidad del selector de imagen
        FindAnyObjectByType<ShowImages>().ReloadImages();
    }
}
