using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class dialog_manager : MonoBehaviour
{
    TextMeshProUGUI text;
    public GameObject botonSi;
    public GameObject botonNo;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        botonSi.SetActive(false);
        botonNo.SetActive(false);
        botonNo.GetComponent<Button>().onClick.AddListener(cancel);
    }

    public void showText(string frase, UnityAction function)
    {
        text.text = frase;
        botonSi.SetActive(true);
        botonNo.SetActive(true);

        botonSi.GetComponent<Button>().onClick.RemoveAllListeners();
        botonSi.GetComponent<Button>().onClick.AddListener(function);
    }

    public void deleteText()
    {
        text.text = "";
        botonSi.SetActive(false);
        botonNo.SetActive(false);
    }

    void cancel()
    {
        deleteText();
        FindAnyObjectByType<camera_movement>().zoomInNow = false;
        FindAnyObjectByType<camera_movement>().zoomOutNow = true;
    }
}
