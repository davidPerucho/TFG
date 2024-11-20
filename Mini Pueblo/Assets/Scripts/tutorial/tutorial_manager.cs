using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class tutorial_manager : MonoBehaviour
{
    public List<PhraseTutorial> frasesTutorial; //Lista con los textos (y posibles reestricciones) que ir�n apareciendo en el tutorial

    [HideInInspector]
    public bool startTutorial = true; //True si se quiere realizar el tutorial

    public TextMeshProUGUI text; //Texto de la intervaz
    int index = 0; //Indice actual de la lista
    bool paused = false; //Indica si el juego est� pausado

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
            //Si no hay condici�n de pausa se pasan las frases al hacer click
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
                //Si el juego no se ha pausado se comprueba la condici�n de pausa
                if (paused == false)
                {
                    if (frasesTutorial[index].pause.Pause() == true)
                    {
                        //En caso de que se cumpla la condici�n de pausa se para el juego
                        Time.timeScale = 0;
                        paused = true;
                    }
                }
                else
                {
                    //Si el juego est� pausado se comprueba la condici�n para reanudar
                    if (frasesTutorial[index].pause.UnPause() == true)
                    {
                        //En caso de que se cumpla la condici�n de reanudar se reanuda el juego y se pasa a la siguiente frase
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
}
