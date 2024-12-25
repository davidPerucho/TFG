using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    const float BUTTONS_TRANSPARENCY = 0.4f;

    [SerializeField]
    Button[] buttonsUI;

    [SerializeField]
    TextMeshProUGUI[] textsUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

    public void enableButton(string buttonName)
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

    public void disableButton(string buttonName)
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

    public void disableObject(string objectName)
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
                    text.gameObject.SetActive(false);
                }
            }
        }
    }

    public void enableObject(string objectName)
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
                    text.gameObject.SetActive(true);
                }
            }
        }
    }

    public void setText(string name, string newText)
    {
        foreach (TextMeshProUGUI text in textsUI)
        {
            if (text.name == name)
            {
                text.text = newText;
            }
        }
    }

    public void setTextColor(string name, Color color)
    {
        foreach (TextMeshProUGUI text in textsUI)
        {
            if (text.name == name)
            {
                text.color = color;
            }
        }
    }
}
