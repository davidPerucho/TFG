using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Clase que se encarga de manejar la interfaz del di�logo.
/// </summary>
public class dialog_manager : MonoBehaviour
{
    TextMeshProUGUI text; //Tecto de la intervaz.
    public GameObject botonSi; //Bot�n de aceptar de la interfaz.
    public GameObject botonNo; //Bot�n de rechazar de la interfaz.
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
    /// Calcula la suma de dos n�meros enteros.
    /// </summary>
    /// <param name="frase">Frase que se mostrar� por pantalla.</param>
    /// <param name="functionLoad">Funci�n de carga de la escena.</param>
    /// <param name="functionCancel">Funci�n que se ejecuta cuando se rechaza al NPC.</param>
    /// <returns>Espera de secondsBeforTalk segundos.</returns>
    public IEnumerator showUIWithDelay(string frase, UnityAction functionLoad, UnityAction functionCancel)
    {
        yield return new WaitForSeconds(secondsBeforeTalk);
        showUI(frase, functionLoad, functionCancel);
    }

    /// <summary>
    /// Funci�n que se llama tras la espera y que actualiza la interfaz.
    /// </summary>
    /// <param name="frase">Frase que se mostrar� por pantalla.</param>
    /// <param name="functionLoad">Funci�n de carga de la escena.</param>
    /// <param name="functionCancel">Funci�n que se ejecuta cuando se rechaza al NPC.</param>
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
    /// Cancela la conversaci�n.
    /// </summary>
    void cancel()
    {
        deleteText();
        //Se activa al zoomOut de la c�mara
        FindAnyObjectByType<camera_movement>().zoomInNow = false;
        FindAnyObjectByType<camera_movement>().zoomOutNow = true;
    }
}
