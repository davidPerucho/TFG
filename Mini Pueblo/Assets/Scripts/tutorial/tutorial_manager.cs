using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Clase que se encarga de manejar el tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour, IDataPersistence
{
    public List<PhraseTutorial> frasesTutorial; //Lista con los textos (y posibles reestricciones) que irán apareciendo en el tutorial

    bool startTutorial = true; //True si se quiere realizar el tutorial

    public TextMeshProUGUI text; //Texto de la intervaz
    int index = 0; //Indice actual de la lista
    bool paused = false; //Indica si el juego está pausado

    void Start()
    {
        if (startTutorial == true)
        {
            text.text = frasesTutorial[0].phrase;
        }
    }

    void Update()
    {
        if (startTutorial == true)
        {
            //Si no hay condición de pausa se pasan las frases al hacer click
            if (frasesTutorial[index].pause == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    index++;
                    if (index >= frasesTutorial.Count)
                    {
                        text.text = "";
                        startTutorial = false;
                    }
                    else
                    {
                        text.text = frasesTutorial[index].phrase;
                    }
                }
            }
            else
            {
                //Si el juego no se ha pausado se comprueba la condición de pausa
                if (paused == false)
                {
                    if (frasesTutorial[index].pause.Pause() == true)
                    {
                        //En caso de que se cumpla la condición de pausa se para el juego
                        Time.timeScale = 0;
                        paused = true;
                    }
                }
                else
                {
                    //Si el juego está pausado se comprueba la condición para reanudar
                    if (frasesTutorial[index].pause.UnPause() == true)
                    {
                        //En caso de que se cumpla la condición de reanudar se reanuda el juego y se pasa a la siguiente frase
                        Time.timeScale = 1;
                        paused = false;

                        index++;
                        if (index >= frasesTutorial.Count)
                        {
                            text.text = "";
                            startTutorial = false;
                        }
                        else
                        {
                            text.text = frasesTutorial[index].phrase;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Carga los datos guardados del tutorial.
    /// </summary>
    /// <param name="data">Datos del juego.</param>
    public void loadData(GameData data)
    {
        startTutorial = data.tutorial;
    }

    /// <summary>
    /// Guarda los datos del tutorial.
    /// </summary>
    /// <param name="data">Referencia a los datos del juego.</param>
    public void saveData(ref GameData data)
    {
        data.tutorial = startTutorial;
    }
}
