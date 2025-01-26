using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject buttonPlay;
    public GameObject buttonNewGame;
    public Button buttonOptions;
    FileDataManager fileDataManager;

    // Start is called before the first frame update
    void Start()
    {
        buttonPlay.GetComponent<Button>().onClick.AddListener(play);
        buttonOptions.onClick.AddListener(options);
        fileDataManager = new FileDataManager(Application.persistentDataPath, "datos.json");
        buttonNewGame.GetComponent<Button>().onClick.AddListener(new_game);
        if (fileDataManager.file_exists() == false) 
        {
            buttonPlay.SetActive(false);
            
        }
    }

    void play()
    {
        disableButtons();
        SceneManager.LoadSceneAsync("Hub");
    }

    void new_game()
    {
        disableButtons();
        DataPersitence.instance.newGame();
        SceneManager.LoadSceneAsync("Hub");
    }    

    void options()
    {
        disableButtons();
        SceneManager.LoadSceneAsync("AnimalPainting");
    }

    void disableButtons()
    {
        buttonNewGame.SetActive(false);
        buttonPlay.SetActive(false);
        buttonOptions.enabled = false;
    }
}
