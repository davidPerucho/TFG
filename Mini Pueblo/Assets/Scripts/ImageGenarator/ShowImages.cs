using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowImages : MonoBehaviour
{
    const int MAX_IMAGES = 3;

    [SerializeField]
    Transform imageLayout; //Un GameObject con un Horizontal Layout Group para posicionar las imágenes

    [SerializeField]
    GameObject imagePrefab; //Un prefab con un componente Image

    public Button leftButon;
    public Button rightButton;

    string imagesDirectory;
    int leftIndex = 0;
    int rightIndex = 0;
    int numImages = MAX_IMAGES;
    string[] imageFiles;

    // Start is called before the first frame update
    void Start()
    {
        //Añado a cada boton su función correspondiente
        UIManager.Instance.AddListenerToButton("ButtonListLeft", moveLeft);
        UIManager.Instance.AddListenerToButton("ButtonListRight", moveRight);

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
            UIManager.Instance.setText("TextTutorial", "Pulsa sobre un mandala para seleccionarlo");
        }
        else
        {
            UIManager.Instance.setText("TextTutorial", "Pulsa el boton de la parte superior para crear un nuevo mandala");
        }

        if (FindAnyObjectByType<ImageGenerator>().loading == true)
        {
            UIManager.Instance.disableButton("ButtonListLeft");
            UIManager.Instance.disableButton("ButtonListRight");
        }
        else
        {
            if (leftIndex == 0 || numImages <= 3)
            {
                UIManager.Instance.disableButton("ButtonListLeft");
            }
            else
            {
                UIManager.Instance.enableButton("ButtonListLeft");
            }

            if (rightIndex == (numImages - 1) || numImages <= 3)
            {
                UIManager.Instance.disableButton("ButtonListRight");
            }
            else
            {
                UIManager.Instance.enableButton("ButtonListRight");
            }
        }
    }

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
                    uiButton.onClick.AddListener(() => DisplaySingleImage(filePath));
                }
                else
                {
                    Debug.LogError("No se ha encontrado el componente Button en el prefab de la imagen.");
                }
            }
        }
    }

    void moveRight()
    {
        leftIndex++;
        rightIndex++;

        DisplayImages();
    }

    void moveLeft()
    {
        leftIndex--;
        rightIndex--;

        DisplayImages();
    }

    public void reloadImages()
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

    void DisplaySingleImage(string filePath)
    {
        //Si se esta creando un nuevo mandala no se puede seleccionar un mandala
        if (FindAnyObjectByType<ImageGenerator>().loading == false) 
        { 
            //Desactivo los elementos UI de la lista
            UIManager.Instance.disableObject("ButtonListRight");
            UIManager.Instance.disableObject("ButtonListLeft");
            UIManager.Instance.disableObject("ButtonGenerateImage");
            UIManager.Instance.disableObject("TextImageGeneration");
            UIManager.Instance.disableObject("TextTutorial");

            //Activo los elementos UI del selector de imagen
            UIManager.Instance.enableObject("ButtonReturn");
            UIManager.Instance.enableObject("ButtonColor");
            UIManager.Instance.enableObject("ButtonDelete");

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
