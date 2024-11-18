using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz que contiene funciones para pausar y reanudar el jugo.
/// </summary>
public abstract class AbstractPause: ScriptableObject
{
    /// <summary>
    /// Indica el momento en el que se debe pausar el juego.
    /// </summary>
    /// <returns>Devuelve True si se cumple la condición para parar el juego.</returns>
    public abstract bool Pause();

    /// <summary>
    /// Indica el momento en el que se debe reanudar el juego.
    /// </summary>
    /// <returns>Devuelve True si se cumple la condición para reanudar el juego.</returns>
    public abstract bool UnPause();
}