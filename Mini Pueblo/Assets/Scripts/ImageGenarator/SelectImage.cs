using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class SelectImage : MonoBehaviour
{
    string filePath;

    [SerializeField]
    GameObject imagePrefab;

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
        GameObject imageObject = Instantiate(imagePrefab);
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

    void DeleteImage()
    {

    }

    void ColorImage()
    {

    }

    void Return()
    {

    }
}
