using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_parable : MonoBehaviour
{
    public GameObject point; //Contiene el objeto punto que formará la parábola
    public int numberOfPoints; //Número de puntos de la parábola
    int lastNumberOfPoints; //Cuantos puntos tenía la parábola la ultima vez que se dibujó
    public float force; //La fuerza que se usa para calcular la parábola
    public float spaceBetwenPoints; //Espacio entre los puntos de la parábola
    public bool drawAlways; //La parábola se dibuja constantemente
    public bool drawWhenTouched = true; //La parábola se dibuja al hacer click
    public bool dynamicDifficulty; //La parábola se vuelve más corta a medida que se avanza
    public bool fixedX = false; //True si solo se mira al eje Y del ratón
    public bool fixedY = false; //True si solo se mira al eje X del ratón
    public float fixedXValue = 0; //Valor de la posición x de referencia para la rotación cuando fixedX es true
    public float fixedYValue = 0; //Valor de la posición y de referencia para la rotación cuando fixedY es true
    public int pointsToIncrease; //Número de puntos necesarios para aumentar la dificultad
    GameObject[] points; //Array con los puntos que conforman la parábola
    Vector3 lastTouchPos; //Última posición en la que se hizo click
    bool pressed; //True si se esta haciendo click
    bool drawn; //True si se ha dibujado la parábola
    int lastIncrease; //La ultima vez que se aumentó la dificultad
    Vector3 mouseScreenPosition; //Posición del ratón en la pantalla

    // Start is called before the first frame update
    void Start()
    {
        force = force / 50; //Ajusta la furza de la parábola
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

        //Comprueba la dificultad dinámica
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
