using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de pausar y reanudar el juego en la primera frase del tutorial.
/// </summary>
[CreateAssetMenu(menuName = "Tutorial/Phrase Pause Script 1")]
public class hub_tutorial_phrase1 : AbstractPause
{

    /// <summary>
    /// Indica que hay que pausar el juago al empezar.
    /// </summary>
    /// <returns>Devuelve True para indicar que se debe de empezar pausando el juego.</returns>
    public override bool Pause()
    {
        return true;
    }

    public override bool UnPause()
    {
        return false;
    }
}
