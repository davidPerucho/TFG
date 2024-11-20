using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Clase que se encarga de pausar y reanudar el juego en la primera frase del tutorial.
/// </summary>
[CreateAssetMenu(menuName = "Tutorial/Hub_tutorial_phrase 1")]
public class hub_tutorial_phrase1 : AbstractPause
{

    /// <summary>
    /// Indica que hay que pausar el juego al empezar.
    /// </summary>
    /// <returns>Devuelve True para indicar que se debe de empezar pausando el juego.</returns>
    public override bool Pause()
    {
        return true;
    }

    /// <summary>
    /// Reanuda el juego cuando se hace click en el terreno.
    /// </summary>
    /// <returns>Devuelve True para indicar que se debe de reanudar el juego.</returns>
    public override bool UnPause()
    {
        string tag = FindAnyObjectByType<player_movement>().hit.collider?.tag ?? "";

        if (tag == "Terrain")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
