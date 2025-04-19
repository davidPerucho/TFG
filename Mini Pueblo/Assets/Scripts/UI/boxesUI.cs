using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Clase encargada de hacer que las casillas se puedan ordenar en un tablero.
/// </summary>
public class BoxesUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    RectTransform dropArea; //Area en la que se puede mover el elemento
    Transform originalParent; //Elemento padre de la casilla antes de que se empiece a arrastrar
    Vector2 originalPosition; //Posición en la que estaba la casilla antes de ser arrastrada
    CanvasGroup canvasGroup; //Grupo de elementos UI

    void Start()
    {
        dropArea = GameObject.Find("ContentBoxes").GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Muestra la interfaz de edición al hacer click sobre la casilla.
    /// </summary>
    /// <param name="eventData">Evento que se produce al hacer click.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        string id = GetComponentInChildren<TextMeshProUGUI>().text;
        CreationManager.Instance.startEditingBox(id);
    }

    /// <summary>
    /// Almacena la posición de la casilla juato cuando empieza a arrastrarse.
    /// </summary>
    /// <param name="eventData">Evento que se genera cuando la casilla empieza a arrastrarse.</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(originalParent.root);
    }

    /// <summary>
    /// Actualiza la posición de la casilla a medida que esta se va arrastrando.
    /// </summary>
    /// <param name="eventData">Evento que se genera mientras se arrastra la casilla.</param>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// Si la casilla se ha arrastrado fuera de la zona permitida vuelve a la posición original, si no se deja en el punto donde se ha soltado.
    /// </summary>
    /// <param name="eventData">Evento que se genera al terminar de arrastrar.</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(dropArea, eventData.position))
        {
            transform.SetParent(dropArea);
        }
        else
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }

        canvasGroup.blocksRaycasts = true;
    }
}
