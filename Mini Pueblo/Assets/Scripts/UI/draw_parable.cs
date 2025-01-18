using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_parable : MonoBehaviour
{
    public GameObject point; //Contiene el objeto punto que formar� la par�bola
    public int numberOfPoints; //N�mero de puntos de la par�bola
    int lastNumberOfPoints; //Cuantos puntos ten�a la par�bola la ultima vez que se dibuj�
    public float force; //La fuerza que se usa para calcular la par�bola
    public float spaceBetwenPoints; //Espacio entre los puntos de la par�bola
    public bool drawAlways; //La par�bola se dibuja constantemente
    public bool drawWhenTouched = true; //La par�bola se dibuja al hacer click
    public bool dynamicDifficulty; //La par�bola se vuelve m�s corta a medida que se avanza
    public bool fixedX = false; //True si solo se mira al eje Y del rat�n
    public bool fixedY = false; //True si solo se mira al eje X del rat�n
    public float fixedXValue = 0; //Valor de la posici�n x de referencia para la rotaci�n cuando fixedX es true
    public float fixedYValue = 0; //Valor de la posici�n y de referencia para la rotaci�n cuando fixedY es true
    public int pointsToIncrease; //N�mero de puntos necesarios para aumentar la dificultad
    GameObject[] points; //Array con los puntos que conforman la par�bola
    Vector3 lastTouchPos; //�ltima posici�n en la que se hizo click
    bool pressed; //True si se esta haciendo click
    bool drawn; //True si se ha dibujado la par�bola
    int lastIncrease; //La ultima vez que se aument� la dificultad
    Vector3 mouseScreenPosition; //Posici�n del rat�n en la pantalla

    // Start is called before the first frame update
    void Start()
    {
        force = force / 50; //Ajusta la furza de la par�bola
        points = new GameObject[numberOfPoints];
        lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastIncrease = FindAnyObjectByType<points_system>().initialPoints;
        lastNumberOfPoints = numberOfPoints;
    }

    // Update is called once per frame
    void Update()
    {
        mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (fixedX == true)
        {
            mouseScreenPosition.x = 0;
        }
        else if (fixedY == true)
        {
            mouseScreenPosition.y = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            pressed = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
        }

        if ((drawWhenTouched == true && pressed == true) || drawAlways == true)
        {
            if (drawn == false)
            {
                for (int i = 0; i < numberOfPoints; i++)
                {
                    points[i] = Instantiate(point, getPointPosition(i * spaceBetwenPoints), Quaternion.identity);
                }
                drawn = true;
            }
            else
            {
                if (mouseScreenPosition != lastTouchPos)
                {
                    for (int i = 0; i < lastNumberOfPoints; i++)
                    {
                        Destroy(points[i]);
                    }
                    for (int i = 0; i < numberOfPoints; i++)
                    {
                        points[i] = Instantiate(point, getPointPosition(i * spaceBetwenPoints), Quaternion.identity);
                    }

                    lastNumberOfPoints = numberOfPoints;
                }
            }
        }

        else
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                Destroy(points[i]);
            }
        }

        //Comprueba la dificultad din�mica
        if (dynamicDifficulty == true)
        {
            updateDifficulty();
        }
    }

    Vector2 getPointPosition(float t)
    {
        Vector2 direction = new Vector2(mouseScreenPosition.x - transform.position.x, mouseScreenPosition.y - transform.position.y);
        Vector2 speed = direction.normalized * force;
        Vector2 pos = (Vector2)transform.position + (speed * t) + 0.5f * Physics2D.gravity * (t * t);

        return pos;
    }

    void updateDifficulty()
    {
        if (pointsToIncrease <= (FindAnyObjectByType<points_system>().points - lastIncrease))
        {
            numberOfPoints -= 2;
            lastIncrease = FindAnyObjectByType<points_system>().points;
        }
    }
}
