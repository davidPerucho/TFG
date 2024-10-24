using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class points_system : MonoBehaviour
{
    public int initialPoints = 0;
    public bool pointEachSecond;
    public int points { get; private set; }
    TextMeshProUGUI pointsText;

    // Start is called before the first frame update
    void Start()
    {
        points = initialPoints;
        pointsText = GetComponent<TextMeshProUGUI>();
        pointsText.text = points.ToString();

        if (pointEachSecond == true)
        {
            StartCoroutine(pointsPerSecond());
        }
    }

    public void addPoints(int points)
    {
        this.points += points;
        pointsText.text = this.points.ToString();
    }

    public void deductPoints(int points)
    {
        this.points -= points;
        pointsText.text = this.points.ToString();
    }

    IEnumerator pointsPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            points++;
            pointsText.text = points.ToString();
        }
    }
}
