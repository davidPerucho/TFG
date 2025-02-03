using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTutorial : MonoBehaviour
{
    [SerializeField]
    string[] frasesTutorial; //Array de frases que se mostrarán en el tutorial

    [SerializeField]
    public TextMeshProUGUI text; //Componente de texto en el que se mostraran las frases

    int textIndex = 0; //Indice actual del texto que se está mostrando
    public static TextTutorial instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton TextTutorial.");
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        text.text = frasesTutorial[textIndex];
    }

    public void nextText()
    {
        textIndex++;

        if (textIndex >= frasesTutorial.Length)
        {
            textIndex = 0;
        }

        text.text = frasesTutorial[textIndex];
    }

    public void goToIndex(int index)
    {
        if (index < frasesTutorial.Length && index >= 0)
        {
            text.text = frasesTutorial[index];
        }
        else
        {
            Debug.LogError("No existe ese índice en las frases.");
        }
    }
}
