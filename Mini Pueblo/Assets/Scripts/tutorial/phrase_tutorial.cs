using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estructura que tendrán de las frases que se mostrarán en el tutorial.
/// </summary>
[System.Serializable]
public struct PhraseTutorial
{
    public string phrase; //Frase
    public AbstractPause pause; //Funciones para pausar y reanudar el juego
}
