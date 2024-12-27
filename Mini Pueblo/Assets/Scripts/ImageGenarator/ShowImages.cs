using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que se encarga de manejar los elementos de la vista de imágenes.
/// </summary>
public class ShowImages : MonoBehaviour
{
    const int MAX_IMAGES = 3; //El número máximo de imágenes que se pueden mostrar por pantalla a la vez

    [SerializeField]
    Transform imageLayout; //Objeto que contiene el layout utilizado para posicionar las imágenes

    [SerializeField]
    GameObject imagePrefab; //Un prefab utilizado para crear la vista de las imágenes

    public Button leftButon; //Botón utilizado para desplazarse a la izquierda en la lista de imágenes
    public Button rightButton; //Botón utilizado para desplazarse a la derecha en la lista de imágenes

    string imagesDirectory; //Ruta al directorio donde se almacenan las imágenes
    int leftIndex = 0; //Posición en el indice de imágenes de la imágen a la izquierda en la pantalla
    int rightIndex = 0; //Posición en el indice de imágenes de la imágen a la derecha en la pantalla
    int numImages = MAX_IMAGES; //Número de imágenes en el sistema
    string[] imageFiles; //Array con las rutas de las imágenes

    // Start is called before the first frame update
    void Start()
    {
        //Añado a cada boton su función correspondiente
        UIManager.Instance.AddListenerToButton("ButtonListLeft", MoveLeft);
        UIManager.Instance.AddListenerToButton("ButtonListRight", MoveRight);

        //Ruta al directorio donde se guardan las imágenes
        imagesDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");

        //Obtener todos los archivos PNG en el directorio
        imageFiles = Directory.GetFiles(imagesDirectory, "*.png");

        numImages = imageFiles.Length;

        //Se muestran como mucho tres imágenes a la vez
        if (numImages > 3 )
        {
            rightIndex = 2;
        }
        else
        {
            rightIndex = numImages - 1;
        }

        //Comprobar que el directorio existe
        if (Directory.Exists(imagesDirectory))
        {
            DisplayImages();
        }
        else
        {
            Debug.LogError($"No existe el directorio: {imagesDirectory}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Cambio el texto del tutorial segun corresponda
        if (numImages > 0)
        {
            UIManager.Instance.SetText("TextTutorial", "Pulsa sobre un mandala para seleccionarlo");
        }
        else
        {
            UIManager.Instance.SetText("TextTutorial", "Pulsa el boton de la parte superior para crear un nuevo mandala");
        }

        if (FindAnyObjectByType<ImageGenerator>().loading == true)
        {
            UIManager.Instance.DisableButton("ButtonListLeft");
            UIManager.Instance.DisableButton("ButtonListRight");
        }
        else
        {
            if (leftIndex == 0 || numImages <= 3)
            {
                UIManager.Instance.DisableButton("ButtonListLeft");
            }
            else
            {
                UIManager.Instance.EnableButton("ButtonListLeft");
            }

            if (rightIndex == (numImages - 1) || numImages <= 3)
            {
                UIManager.Instance.DisableButton("ButtonListRight");
            }
            else
            {
                UIManager.Instance.EnableButton("ButtonListRight");
            }
        }
    }

    /// <summary>
    /// Mustra por pantalla la lista de imágenes creadas.
    /// </summary>
    void DisplayImages()
    {
        //Eliminar todas las imagenes que hay en la lista actualmente
        foreach (Transform child in imageLayout)
        {
            Destroy(child.gameObject);
        }

        //Cargar las tres primeras imágenes y mostrarlas
        if (numImages > 0)
        {
            for (int i = leftIndex; i <= rightIndex; i++)
            {
                string filePath = imageFiles[i];

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
                GameObject imageObject = Instantiate(imagePrefab, imageLayout);
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

                Button uiButton = imageObject.GetComponent<Button>();
                if (uiButton != null)
                {
                    uiButton.onClick.RemoveAllListeners();
                    uiButton.onClick.AddListener(() => SelectSingleImage(filePath));
                }
                else
                {
                    Debug.LogError("No se ha encontrado el componente Button en el prefab de la imagen.");
                }
            }
        }
    }

    /// <summary>
    /// Desplaza la lista de imágenes hacia la derecha.
    /// </summary>
    void MoveRight()
    {
        leftIndex++;
        rightIndex++;

        DisplayImages();
    }

    /// <summary>
    /// Desplaza la lista de imágenes hacia la izquierda.
    /// </summary>
    void MoveLeft()
    {
        leftIndex--;
        rightIndex--;

        DisplayImages();
    }

    /// <summary>
    /// Recarga la lista de imágenes.
    /// </summary>
    public void ReloadImages()
    {
        //Obtener todos los archivos PNG en el directorio
        imageFiles = Directory.GetFiles(imagesDirectory, "*.png");

        numImages = imageFiles.Length;

        //Se muestran como mucho tres imágenes a la vez
        if (numImages > 3)
        {
            rightIndex = 2;
        }
        else
        {
            rightIndex = numImages - 1;
        }

        leftIndex = 0;

        DisplayImages();
    }

    /// <summary>
    /// Mustra por pantalla la imagen seleccionada y varias opciones.
    /// </summary>
    /// <param name="filePath">Ruta de la imagen seleccionada.</param>
    void SelectSingleImage(string filePath)
    {
        //Si se esta creando una nueva imagen no se puede seleccionar la imagen
        if (FindAnyObjectByType<ImageGenerator>().loading == false) 
        { 
            //Desactivo los elementos UI de la lista
            UIManager.Instance.DisableObject("ButtonListRight");
            UIManager.Instance.DisableObject("ButtonListLeft");
            UIManager.Instance.DisableObject("ButtonGenerateImage");
            UIManager.Instance.DisableObject("TextImageGeneration");
            UIManager.Instance.DisableObject("TextTutorial");

            //Activo los elementos UI del selector de imagen
            UIManager.Instance.EnableObject("ButtonReturn");
            UIManager.Instance.EnableObject("ButtonColor");
            UIManager.Instance.EnableObject("ButtonDelete");

            //Elimino las imágenes de la lista
            foreach (Transform child in imageLayout)
            {
                Destroy(child.gameObject);
            }

            //Inicio la funcionalidad del selector de imagen
            FindAnyObjectByType<SelectImage>().DisplaySingleImage(filePath);
        }
    }
}
