using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Clase singleton que se encarga de manejar los elementos UI del juego.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } //Instancia de la clase
    const float BUTTONS_TRANSPARENCY = 0.4f; //Transparencia que tienen los botones al ser desactivados

    [SerializeField]
    Button[] buttonsUI; //Array con los botones de la interfaz de usuario

    [SerializeField]
    TextMeshProUGUI[] textsUI; //Array con los textos de la interfaz de usuario

    [SerializeField]
    TMP_InputField[] inputsUI; //Array con los inputs de texto de la interfaz de usuario

    //Singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Añade a un boton su función.
    /// </summary>
    /// <param name="buttonName">Nombre del boton.</param>
    /// <param name="function">Función que se llamará al pulsar el botón.</param>
    public void AddListenerToButton(string buttonName, UnityAction function)
    {
        foreach (Button button in buttonsUI)
        {
            if (button.name == buttonName)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(function);
            }
        }
    }

    /// <summary>
    /// Activa un botón.
    /// </summary>
    /// <param name="buttonName">Nombre del boton.</param>
    public void EnableButton(string buttonName)
    {
        foreach (Button button in buttonsUI)
        {
            if (button.name == buttonName)
            {
                button.enabled = true;

                //Aumenta la transparencia del motor
                Image imageButton = button.GetComponent<Image>();
                Color colorButton = imageButton.color;
                colorButton.a = Mathf.Clamp01(1);
                imageButton.color = colorButton;
            }
        }
    }

    /// <summary>
    /// Desactiva un botón.
    /// </summary>
    /// <param name="buttonName">Nombre del boton.</param>
    public void DisableButton(string buttonName)
    {
        foreach (Button button in buttonsUI)
        {
            if (button.name == buttonName)
            {
                button.enabled = false;

                //Quita la transparencia del boton
                Image imageButton = button.GetComponent<Image>();
                Color colorButton = imageButton.color;
                colorButton.a = Mathf.Clamp01(BUTTONS_TRANSPARENCY);
                imageButton.color = colorButton;
            }
        }
    }

    /// <summary>
    /// Desactiva un objeto.
    /// </summary>
    /// <param name="objectName">Nombre del objeto.</param>
    public void DisableObject(string objectName)
    {
        bool found = false;

        foreach (Button button in buttonsUI)
        {
            if (button.name == objectName)
            {
                found = true;
                button.gameObject.SetActive(false);
            }
        }

        if (found == false)
        {
            foreach (TextMeshProUGUI text in textsUI)
            {
                if (text.name == objectName)
                {
                    found = true;
                    text.gameObject.SetActive(false);
                }
            }
        }

        if (found == false)
        {
            foreach (TMP_InputField input in inputsUI)
            {
                if (input.name == objectName)
                {
                    input.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Activa un objeto.
    /// </summary>
    /// <param name="objectName">Nombre del objeto.</param>
    public void EnableObject(string objectName)
    {
        bool found = false;

        foreach (Button button in buttonsUI)
        {
            if (button.name == objectName)
            {
                found = true;
                button.gameObject.SetActive(true);
            }
        }

        if (found == false)
        {
            foreach (TextMeshProUGUI text in textsUI)
            {
                if (text.name == objectName)
                {
                    found = true;
                    text.gameObject.SetActive(true);
                }
            }
        }

        if (found == false)
        {
            foreach (TMP_InputField input in inputsUI)
            {
                if (input.name == objectName)
                {
                    input.gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Cambia la frase de un elemnto de texto.
    /// </summary>
    /// <param name="name">Nombre del elemento de texto.</param>
    /// <param name="newText">Nueva frase.</param>
    public void SetText(string name, string newText)
    {
        foreach (TextMeshProUGUI text in textsUI)
        {
            if (text.name == name)
            {
                text.text = newText;
            }
        }
    }

    /// <summary>
    /// Cambia el color de un elemento de texto.
    /// </summary>
    /// <param name="name">Nombre del elemento de texto.</param>
    /// <param name="color">Color que se quiere asignar al elemento.</param>
    public void SetTextColor(string name, Color color)
    {
        foreach (TextMeshProUGUI text in textsUI)
        {
            if (text.name == name)
            {
                text.color = color;
            }
        }
    }

    /// <summary>
    /// Devuelve el texto que se ha escrito en el input.
    /// </summary>
    /// <param name="name">Nombre del elemento input.</param>
    public string GetInputValue(string name)
    {
        foreach (TMP_InputField input in inputsUI)
        {
            if (input.name == name)
            {
                return input.text;
            }
        }

        return "";
    }
}
