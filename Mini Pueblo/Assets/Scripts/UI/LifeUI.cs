using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Clase que se encarga de la representaci�n y gesti�n de las vidas.
/// </summary>
public class LifeUI : MonoBehaviour
{
    [SerializeField]
    public int numLifes; //Numero de vidas

    [SerializeField]
    float spaceBetweenLifes = 0.5f;

    [SerializeField]
    Sprite heartImage; //Imagen que representar� los corazones

    [SerializeField]
    Vector2 imageSize; //Tama�o de la imagen

    List<GameObject> hearts; //Corazones

    public static LifeUI instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton LifeUI.");
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        hearts = new List<GameObject>();
        float pixelsPerUnit = heartImage.pixelsPerUnit;
        imageSize.x = imageSize.x/pixelsPerUnit;
        imageSize.y = imageSize.y/pixelsPerUnit;

        //Creo un numero numLifes de corazones
        int i;
        for (i = 0; i < numLifes; i++)
        {
            //Creo el coraz�n
            GameObject heart = new GameObject("Heart_" + i);
            heart.transform.SetParent(this.transform);

            //Creo el componente de la imag�n
            heart.AddComponent<SpriteRenderer>();
            SpriteRenderer imageComponent = heart.GetComponent<SpriteRenderer>();
            imageComponent.sprite = heartImage;

            //Configuro la escala y la posici�n
            heart.transform.localScale = new Vector3(imageSize.x, imageSize.y, 1);
            heart.transform.position = new Vector3(transform.position.x + (i * (imageSize.x*2 + spaceBetweenLifes)), transform.position.y, transform.position.z);

            //A�ado el coraz�n al array
            hearts.Add(heart);
        }
    }

    /// <summary>
    /// Reduce en uno el n�mero de vidas.
    /// </summary>
    public void decreaseLife()
    {
        int i = 1;
        foreach (GameObject heart in hearts)
        {
            if (i == numLifes)
            {
                hearts.RemoveAt(i-1);
                Destroy(heart);
                break;
            }

            i++;
        }

        numLifes--;
    }

    /// <summary>
    /// Aumenta en uno el n�mero de vidas.
    /// </summary>
    public void increaseLife()
    {
        //Creo el coraz�n
        GameObject heart = new GameObject("Heart_" + numLifes);
        heart.transform.SetParent(this.transform);

        //Creo el componente de la imag�n
        heart.AddComponent<SpriteRenderer>();
        SpriteRenderer imageComponent = heart.GetComponent<SpriteRenderer>();
        imageComponent.sprite = heartImage;

        //Configuro la escala y la posici�n
        heart.transform.localScale = new Vector3(imageSize.x, imageSize.y, 1);
        heart.transform.position = new Vector3(transform.position.x + (numLifes * (imageSize.x * 2 + spaceBetweenLifes)), transform.position.y, transform.position.z);

        //A�ado el coraz�n al array
        hearts.Add(heart);

        numLifes++;
    }
}
