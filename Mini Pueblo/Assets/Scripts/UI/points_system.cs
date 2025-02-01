using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Clase que se encarga de manejar los puntos y el marcador de los minijuegos.
/// </summary>
public class PointsSystem : MonoBehaviour
{
    public int initialPoints = 0; //Puntos a partir de los cuales empieza a contar el marcador
    public bool pointEachSecond; //True si se quiere sumar puntos a medida que pasa el tiempo
    public int points { get; private set; } //Puntos actuales
    TextMeshProUGUI pointsText; //Texto que muestra por pantalla los puntos actuales
    public static PointsSystem instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton PointsSystem.");
            return;
        }

        instance = this;
    }

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

    /// <summary>
    /// Añade un número de puntos al marcador.
    /// </summary>
    /// <param name="points">Número de puntos a añadir.</param>
    public void addPoints(int points)
    {
        this.points += points;
        pointsText.text = this.points.ToString();
    }

    /// <summary>
    /// Deduce un número de puntos del marcador.
    /// </summary>
    /// <param name="points">Número de puntos a deducir.</param>
    public void deductPoints(int points)
    {
        this.points -= points;
        pointsText.text = this.points.ToString();
    }

    /// <summary>
    /// Aumenta el contador a medida que pasan los segundos.
    /// </summary>
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
