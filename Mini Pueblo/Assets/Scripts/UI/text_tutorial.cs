using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class text_tutorial : MonoBehaviour
{
    string[] frasesTutorial;
    TextMeshProUGUI text;
    int textIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        frasesTutorial = new string[]{
            "Mueve el avion para esquivar los obstaculos y aguantar el mayor tiempo posible.",
            "Manten pulsada la pantalla para subir.",
            "Deja de pulsar la pantalla para bajar."
        };

        text = GetComponent<TextMeshProUGUI>();

        text.text = frasesTutorial[textIndex];
    }

    public void nextText()
    {
        textIndex++;

        if (textIndex >= frasesTutorial.Length)
        {
            textIndex = 1;
        }

        text.text = frasesTutorial[textIndex];
    }
}
