using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class draw_parable : MonoBehaviour
{
    public GameObject point; //The points that are going to be drawn
    public int numberOfPoints; //The number of points in the parable
    int lastNumberOfPoints; //The last number of points in the parable
    public float force; //The force used to calculate the parable
    public float spaceBetwenPoints; //The space between the points of the drawing
    public bool drawAlways; //The parable will always be drawed
    public bool drawWhenTouched = true; //The parable will only be drawed when the screen is touched
    public bool dynamicDifficulty; //The parable will get sorter as the player scores more points
    public int pointsToIncrease; //The amount of points needed to increase the dificulty
    GameObject[] points; //Array containing all the points
    Vector3 lastTouchPos; //Position of the last time the screen was toched
    bool pressed;
    bool drawn;
    int lastIncrease;

    // Start is called before the first frame update
    void Start()
    {
        force = force / 50; //Apply the proportion of the force to make the parable and the trayectory be the same
        points = new GameObject[numberOfPoints];
        lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastIncrease = FindAnyObjectByType<points_system>().initialPoints;
        lastNumberOfPoints = numberOfPoints;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

        //Check dynamic difficulty
        if (dynamicDifficulty == true)
        {
            updateDifficulty();
        }
    }

    Vector2 getPointPosition(float t)
    {
        Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseScreenPosition.x <= transform.position.x)
        {
            mouseScreenPosition.x = transform.position.x + (transform.position.x - mouseScreenPosition.x);
            mouseScreenPosition.y = transform.position.y + (transform.position.y - mouseScreenPosition.y);
        }
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
