using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxesUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    RectTransform dropArea;

    Transform originalParent;
    Vector2 originalPosition;
    CanvasGroup canvasGroup;

    void Start()
    {
        dropArea = GameObject.Find("ContentBoxes").GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Item pulsado: " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(originalParent.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(dropArea, eventData.position))
        {
            transform.SetParent(dropArea);
        }
        else
        {
            Debug.Log(originalParent);
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }

        canvasGroup.blocksRaycasts = true;
    }
}
