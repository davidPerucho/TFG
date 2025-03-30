using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    Button pauseButton; //Bot�n de pausa

    [SerializeField]
    GameObject pauseUI; //Objeto con la interfaz del menu de pausa

    [SerializeField]
    Button resume; //Bot�n para reanudar el juego

    [SerializeField]
    Button mainMenu; //Bot�n para volver al menu principal

    // Start is called before the first frame update
    void Start()
    {
        //A�ado las funciones a los botones
        pauseButton.onClick.AddListener(pauseGame);
        mainMenu.onClick.AddListener(() => { DataPersitence.instance.saveGame(); SceneManager.LoadScene("MainMenu"); });
        resume.onClick.AddListener(resumeGame);
    }

    /// <summary>
    /// Pausa el juego y muestra el menu de pausa.
    /// </summary>
    void pauseGame()
    {
        Time.timeScale = 0;
        pauseUI.SetActive(true);
    }

    /// <summary>
    /// Reanuda el juego.
    /// </summary>
    void resumeGame()
    {
        Time.timeScale = 1;
        pauseUI.SetActive(false);
    }
}
