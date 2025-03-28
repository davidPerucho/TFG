using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaintingNumbers : MonoBehaviour
{
    public static PaintingNumbers instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton PaintingNumbers.");
            return;
        }

        instance = this;
    }

    /// <summary>
    /// Añade números a los huecos blancos de una imagen.
    /// </summary>
    /// <param name="baseImage">Imagen base sobre la que añadir los números.</param>
    /// <param name="font">Fuente que utilizarán los números añadidos a la imagen</param>
    public Texture2D AddNumbers(Texture2D baseImage)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Texture2D numberedImage = new Texture2D(baseImage.width, baseImage.height);
        Graphics.CopyTexture(baseImage, numberedImage);

        List<Vector2> regionCenters = GetEmptyRegions(baseImage);

        for (int i = 0; i < regionCenters.Count; i++)
        {
            DrawNumber(regionCenters[i], (i + 1).ToString(), font);
        }

        numberedImage.Apply();

        return numberedImage;
    }

    /// <summary>
    /// Devuelve las regiones en blanco de la imagen.
    /// </summary>
    /// <param name="image">Imagen de la que se obtendrán las zonas blancas</param>
    List<Vector2> GetEmptyRegions(Texture2D image)
    {
        List<Vector2> centers = new List<Vector2>();
        bool[,] visited = new bool[image.width, image.height];

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                if (!visited[x, y] && IsWhite(image.GetPixel(x, y)))
                {
                    List<Vector2> regionPixels = GetRegionPixels(image, x, y, visited);
                    if (regionPixels.Count > 150)
                    {
                        centers.Add(GetCenter(regionPixels));
                    }
                }
            }
        }

        return centers;
    }

    /// <summary>
    /// Utiliza el algoritmo flood fill para encontrar los píxeles conectados a una región.
    /// </summary>
    /// <param name="image">Imagen en la que se buscará la región.</param>
    /// <param name="startX">Coordenada X sobre la que se empezará a buscar.</param>
    /// <param name="startY">Coordenada Y sobre la que se empezará a buscar.</param>
    /// <param name="visited">Contiene true para los puntos visitados.</param>
    List<Vector2> GetRegionPixels(Texture2D image, int startX, int startY, bool[,] visited)
    {
        List<Vector2> pixels = new List<Vector2>();
        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(new Vector2(startX, startY));

        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();
            int x = (int)current.x;
            int y = (int)current.y;

            if (x < 0 || x >= image.width || y < 0 || y >= image.height || visited[x, y])
            {
                continue;
            }

            if (!IsWhite(image.GetPixel(x, y)))
            {
                continue;
            }

            visited[x, y] = true;
            pixels.Add(current);

            queue.Enqueue(new Vector2(x + 1, y));
            queue.Enqueue(new Vector2(x - 1, y));
            queue.Enqueue(new Vector2(x, y + 1));
            queue.Enqueue(new Vector2(x, y - 1));
        }

        return pixels;
    }

    /// <summary>
    /// Devuelve true si un color es lo suficientemente blanco.
    /// </summary>
    bool IsWhite(Color color)
    {
        if (color.r > 0.8f && color.g > 0.8f && color.b > 0.8f) { 
            return true;
        }

        return false;
    }

    /// <summary>
    /// Obtiene el centro aproximado de una lista de píxeles.
    /// </summary>
    Vector2 GetCenter(List<Vector2> pixels)
    {
        float sumX = 0, sumY = 0;
        foreach (Vector2 pixel in pixels)
        {
            sumX += pixel.x;
            sumY += pixel.y;
        }
        return new Vector2(sumX / pixels.Count, sumY / pixels.Count);
    }

    /// <summary>
    /// Dibuja números sobre una imagen.
    /// </summary>
    /// <param name="texture">Imagen sobre la que se dibujarán los números.</param>
    /// <param name="position">Posición sobre la que se dibujará el número.</param>
    /// <param name="font">Fuente de los números.</param>
    void DrawNumber(Vector2 position, string number, Font font)
    {
        GameObject textObj = new GameObject("NumberText" + Time.time.ToString());
        textObj.transform.position = position;

        
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = number;
        tmp.fontSize = 30;
        tmp.color = Color.white;
        tmp.faceColor = Color.black;
        tmp.alignment = TextAlignmentOptions.Center;
    }
}
