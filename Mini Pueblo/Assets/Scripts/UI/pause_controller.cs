using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    Button pauseButton; //Botón de pausa

    [SerializeField]
    GameObject pauseUI; //Objeto con la interfaz del menu de pausa

    [SerializeField]
    Button resume; //Botón para reanudar el juego

    [SerializeField]
    Button mainMenu; //Botón para volver al menu principal

    // Start is called before the first frame update
    void Start()
    {
        //Añado las funciones a los botones
        pauseButton = GetComponent<Button>();
        pauseButton.onClick.AddListener(pauseGame);
        mainMenu.onClick.AddListener(() => { DataPersitence.instance.saveGame(); Time.timeScale = 1; SceneManager.LoadScene("MainMenu"); });
        resume.onClick.AddListener(resumeGame);
    }

    /// <summary>
    /// Pausa el juego y muestra el menu de pausa.
    /// </summary>
    void pauseGame()
    {
        Time.timeScale = 0;
        pauseUI.SetActive(true);
        if (SoundManager.instance != null)
        {
            SoundManager.instance.volumeMusicDown();
        }
    }

    /// <summary>
    /// Reanuda el juego.
    /// </summary>
    void resumeGame()
    {
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        if (SoundManager.instance != null)
        {
            SoundManager.instance.volumeMusicUp();
        }
    }
}
