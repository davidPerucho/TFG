using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowImages : MonoBehaviour
{
    const int MAX_IMAGES = 3;
    const float BUTTONS_TRANSPARENCY = 0.4f;

    [SerializeField]
    Transform imageLayout; //Un GameObject con un Horizontal Layout Group para posicionar las imágenes

    [SerializeField]
    GameObject imagePrefab; //Un prefab con un componente Image

    [SerializeField]
    Button leftButon;

    [SerializeField]
    Button rightButton;

    string imagesDirectory;
    int leftIndex = 0;
    int rightIndex = 0;
    int numImages = MAX_IMAGES;
    string[] imageFiles;

    // Start is called before the first frame update
    void Start()
    {
        //Añado a cada boton su función correspondiente
        leftButon.onClick.RemoveAllListeners();
        leftButon.onClick.AddListener(moveLeft);

        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(moveRight);

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
        Image imageButtonLeft = leftButon.GetComponent<Image>();
        Image imageButtonRight = rightButton.GetComponent<Image>();
        Color colorButtonLeft = imageButtonLeft.color;
        Color colorButtonRight = imageButtonRight.color;

        if (leftIndex == 0 || numImages <= 3)
        {
            leftButon.enabled = false;
            colorButtonLeft.a = Mathf.Clamp01(BUTTONS_TRANSPARENCY);
            imageButtonLeft.color = colorButtonLeft;
        }
        else
        {
            leftButon.enabled = true;
            colorButtonLeft.a = Mathf.Clamp01(1);
            imageButtonLeft.color = colorButtonLeft;
        }

        if (rightIndex == (numImages-1) || numImages <= 3)
        {
            rightButton.enabled = false;
            colorButtonRight.a = Mathf.Clamp01(BUTTONS_TRANSPARENCY);
            imageButtonRight.color = colorButtonRight;
        }
        else
        {
            rightButton.enabled = true;
            colorButtonRight.a = Mathf.Clamp01(1);
            imageButtonRight.color = colorButtonRight;
        }
    }

    void DisplayImages()
    {
        //Eliminar todas las imagenes que hay en la lista actualmente
        foreach (Transform child in imageLayout)
        {
            Destroy(child.gameObject);
        }

        // Cargar las tres primeras imágenes y mostrarlas
        if (numImages > 0)
        {
            for (int i = leftIndex; i <= rightIndex; i++)
            {
                string filePath = imageFiles[i];

                // Cargar la imagen desde el archivo
                byte[] imageBytes = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);

                // Crear un nuevo Sprite a partir de la textura
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Instanciar el prefab y configurarlo
                GameObject imageObject = Instantiate(imagePrefab, imageLayout);
                Image uiImage = imageObject.GetComponent<Image>();
                if (uiImage != null)
                {
                    uiImage.sprite = sprite;
                    uiImage.preserveAspect = true;
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
}
