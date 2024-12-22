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
        if (fileDataManager.file_exists()) 
        {
            buttonNewGame.SetActive(true);
            buttonNewGame.GetComponent<Button>().onClick.AddListener(new_game);
        }
    }

    void play()
    {
        SceneManager.LoadScene("Hub");
    }

    void new_game()
    {
        fileDataManager.delete_save();
        SceneManager.LoadScene("Hub");
    }

    void options()
    {
        SceneManager.LoadScene("AnimalPainting");
    }
}
