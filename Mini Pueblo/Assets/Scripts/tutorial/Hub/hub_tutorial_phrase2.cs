using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de pausar y reanudar el juego en la segunda frase del tutorial.
/// </summary>
[CreateAssetMenu(menuName = "Tutorial/Hub_tutorial_phrase 2")]
public class hub_tutorial_phrase2 : AbstractPause
{
    float time; //Almacena el tiempo (en segundos) de la primera llamada a función
    bool startWait = false; //True cuando ya se ha llamado a la función por primera vez

    /// <summary>
    /// Indica que hay que pausar el juego cuando hayan pasado dos segundos de el anterior texto del tutorial.
    /// </summary>
    /// <returns>Devuelve True para indicar que se debe de pausar el juego.</returns>
    public override bool Pause()
    {
        //Almacena el tiempo actual en la primera llamada a la función
        if (startWait == false)
        {
            startWait = true;
            time = Time.time;
        }
        else
        {
            if ((Time.time - time) >= 2)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Reanuda el juego cuando se hace click en un NPC.
    /// </summary>
    /// <returns>Devuelve True para indicar que se debe de reanudar el juego.</returns>
    public override bool UnPause()
    {
        string tag = FindAnyObjectByType<player_movement>().hit.collider?.tag ?? "";

        if (tag == "NPC")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
