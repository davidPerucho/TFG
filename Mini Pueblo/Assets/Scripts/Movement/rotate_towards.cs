using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de la rotaci�n de un objeto en una direcci�n.
/// </summary>
public class rotate_towards : MonoBehaviour
{
    public bool toObject; //True si la rotaci�n se realiza en direcci�n a un objeto
    public GameObject rotationObject; //Referencia al objeto hacia el que se rota cuando toObject es true
    public bool toMouse; //True si la rotaci�n se har� en direcci�n al rat�n
    public bool fixedX = false; //True si solo se mira al eje Y del rat�n
    public bool fixedY = false; //True si solo se mira al eje X del rat�n
    public float fixedXValue = 0; //Valor de la posici�n x de referencia para la rotaci�n cuando fixedX es true
    public float fixedYValue = 0; //Valor de la posici�n y de referencia para la rotaci�n cuando fixedY es true

    // Update is called once per frame
    void Update()
    {
        //Rotar en direcci�n a un objeto
        if (toObject == true && Time.timeScale != 0)
        {
            float anguloRad = Mathf.Atan2(rotationObject.transform.position.y - transform.position.y, rotationObject.transform.position.x - transform.position.x);
            float anguloDeg = (180 / Mathf.PI) * anguloRad;
            transform.rotation = Quaternion.Euler(0, 0, anguloDeg);
        }
        //Rotar en direcci�n al rat�n
        else
        {
            if (Time.timeScale != 0)
            {
                Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (fixedX == true)
                {
                    mouseScreenPosition.x = fixedXValue;
                }
                else if (fixedY == true)
                {
                    mouseScreenPosition.y = fixedYValue;
                }

                float anguloRad = Mathf.Atan2(mouseScreenPosition.y - transform.position.y, mouseScreenPosition.x - transform.position.x);
                float anguloDeg = (180 / Mathf.PI) * anguloRad;
                transform.rotation = Quaternion.Euler(0, 0, anguloDeg);
            }
        }
    }
}
