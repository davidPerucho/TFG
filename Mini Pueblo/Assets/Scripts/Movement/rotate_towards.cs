using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_towards : MonoBehaviour
{
    public bool toObject; //True si la rotación se realiza en dirección a un objeto
    public GameObject rotationObject; //Referencia al objeto hacia el que se rota cuando toObject es true
    public bool toMouse; //True si la rotación se hará en dirección al ratón
    public bool fixedX = false; //True si solo se mira al eje Y del ratón
    public bool fixedY = false; //True si solo se mira al eje X del ratón
    public float fixedXValue = 0; //Valor de la posición x de referencia para la rotación cuando fixedX es true
    public float fixedYValue = 0; //Valor de la posición y de referencia para la rotación cuando fixedY es true

    // Update is called once per frame
    void Update()
    {
        if (toObject == true)
        {
            float anguloRad = Mathf.Atan2(rotationObject.transform.position.y - transform.position.y, rotationObject.transform.position.x - transform.position.x);
            float anguloDeg = (180 / Mathf.PI) * anguloRad;
            transform.rotation = Quaternion.Euler(0, 0, anguloDeg);
        }
        else
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
