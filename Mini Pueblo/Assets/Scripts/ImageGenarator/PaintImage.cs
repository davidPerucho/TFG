using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintImage : MonoBehaviour
{
    string filePath;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.AddListenerToButton("ButtonSave", save);
    }

    public void DisplayPaintImage(string imagePath)
    {
        filePath = imagePath;

        Debug.Log($"Empezamos a pintar la imagen {filePath}");
    }

    void save()
    {

    }
}
