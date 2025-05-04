using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxManager : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        int boxId = int.Parse(transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);
        TableGameManager.Instance.selectBox(boxId);
    }
}
