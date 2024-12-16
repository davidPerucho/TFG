using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShowImages : MonoBehaviour
{
    [SerializeField]
    Transform imageLayout; //Un GameObject con un Horizontal Layout Group para posicionar las imágenes

    [SerializeField]
    GameObject imagePrefab; //Un prefab con un componente Image

    string imagesDirectory;

    // Start is called before the first frame update
    void Start()
    {
        imagesDirectory = Path.Combine(Application.persistentDataPath, "GeneratedImages");

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
        
    }

    void DisplayImages()
    {
        int numImages = 3;

        // Obtener todos los archivos PNG en el directorio
        string[] imageFiles = Directory.GetFiles(imagesDirectory, "*.png");

        //Como maximo se muestran tres imágenes en pantalla
        if (imageFiles.Length < 3)
        {
            numImages = imageFiles.Length;
        }

        // Cargar las tres primeras imágenes y mostrarlas
        for (int i = 0; i < numImages; i++)
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
