using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase encargada de gestionar la edición de links del tablero.
/// </summary>
public class LinkEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Obtengo los botones de la interfaz
        Button returnButton = transform.Find("Volver").GetComponent<Button>();

        //Añado la funcionalidad correspondiente a cada boton
        returnButton.onClick.AddListener(() =>
        {
            UIManager.Instance.EnableObject("Jugadores");
            UIManager.Instance.EnableObject("Tablero");
            UIManager.Instance.EnableObject("Links");
            UIManager.Instance.EnableObject("CrearTablero");

            gameObject.SetActive(false);
        });
    }
}
