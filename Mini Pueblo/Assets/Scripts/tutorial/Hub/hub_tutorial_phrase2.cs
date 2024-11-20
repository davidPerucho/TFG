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
    bool pause = false;
    bool startWait = false;

    /// <summary>
    /// Indica que hay que pausar el juego cuando el jugador haya llegado a su destino.
    /// </summary>
    /// <returns>Devuelve True para indicar que se debe de pausar el juego.</returns>
    public override bool Pause()
    {
        if (pause == true) {
            return true;
        }
        else
        {
            if (startWait == false)
            {
                startWait = true;
                _ = waitPause();
            }

            return false;
        }
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

    async Task waitPause()
    {
        Debug.Log("Empieza");
        await Task.Delay(2000);
        Debug.Log("Termina");
        pause = true;
    }
}
