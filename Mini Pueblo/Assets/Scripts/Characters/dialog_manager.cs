using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Clase que se encarga de manejar la interfaz del diálogo.
/// </summary>
public class dialog_manager : MonoBehaviour
{
    TextMeshProUGUI text; //Tecto de la intervaz.
    public GameObject botonSi; //Botón de aceptar de la interfaz.
    public GameObject botonNo; //Botón de rechazar de la interfaz.
    float secondsBeforeTalk = 1; //Retraso antes de mostrar la interfaz (para dar tiempo a las animaciones)

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        botonSi.SetActive(false);
        botonNo.SetActive(false);
        botonNo.GetComponent<Button>().onClick.AddListener(cancel);
    }

    /// <summary>
    /// Calcula la suma de dos números enteros.
    /// </summary>
    /// <param name="frase">Frase que se mostrará por pantalla.</param>
    /// <param name="functionLoad">Función de carga de la escena.</param>
    /// <param name="functionCancel">Función que se ejecuta cuando se rechaza al NPC.</param>
    /// <returns>Espera de secondsBeforTalk segundos.</returns>
    public IEnumerator showUIWithDelay(string frase, UnityAction functionLoad, UnityAction functionCancel)
    {
        yield return new WaitForSeconds(secondsBeforeTalk);
        showUI(frase, functionLoad, functionCancel);
    }

    /// <summary>
    /// Función que se llama tras la espera y que actualiza la interfaz.
    /// </summary>
    /// <param name="frase">Frase que se mostrará por pantalla.</param>
    /// <param name="functionLoad">Función de carga de la escena.</param>
    /// <param name="functionCancel">Función que se ejecuta cuando se rechaza al NPC.</param>
    void showUI(string frase, UnityAction functionLoad, UnityAction functionCancel)
    {
        text.text = frase;
        botonSi.SetActive(true);
        botonNo.SetActive(true);

        botonSi.GetComponent<Button>().onClick.RemoveAllListeners();
        botonSi.GetComponent<Button>().onClick.AddListener(functionLoad);

        botonNo.GetComponent<Button>().onClick.RemoveAllListeners();
        botonNo.GetComponent<Button>().onClick.AddListener(cancel);
        botonNo.GetComponent<Button>().onClick.AddListener(functionCancel);
    }

    /// <summary>
    /// Elimina el texto y las funciones de los botones.
    /// </summary>
    public void deleteText()
    {
        text.text = "";
        botonSi.SetActive(false);
        botonNo.SetActive(false);
    }

    /// <summary>
    /// Cancela la conversación.
    /// </summary>
    void cancel()
    {
        deleteText();
        //Se activa al zoomOut de la cámara
        FindAnyObjectByType<camera_movement>().zoomInNow = false;
        FindAnyObjectByType<camera_movement>().zoomOutNow = true;
    }
}
