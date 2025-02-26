using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    FileDataManager fileDataManager;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.AddListenerToButton("Jugar", play);
        UIManager.Instance.AddListenerToButton("Volver", returnToMenu);
        UIManager.Instance.AddListenerToButton("CrearMenu", openCreateSceneinterface);
        fileDataManager = new FileDataManager(Application.persistentDataPath, "datos.json");
        UIManager.Instance.AddListenerToButton("Nueva", newGame);
        if (fileDataManager.file_exists() == false) 
        {
            UIManager.Instance.DisableObject("Jugar");
        }
    }

    void play()
    {
        disableButtons();
        SceneManager.LoadSceneAsync("Hub");
    }

    void newGame()
    {
        disableButtons();
        DataPersitence.instance.newGame();
        SceneManager.LoadSceneAsync("Hub");
    }

    void openCreateSceneinterface()
    {
        UIManager.Instance.DisableObject("Nueva");
        UIManager.Instance.DisableObject("Jugar");
        UIManager.Instance.DisableObject("TextoMenu");
        UIManager.Instance.DisableObject("CrearMenu");
        UIManager.Instance.DisableObject("Opciones");

        UIManager.Instance.EnableObject("Crear");
        UIManager.Instance.EnableObject("Volver");
        UIManager.Instance.EnableObject("InputNombre");
        UIManager.Instance.EnableObject("TextoCrear");
    }

    void returnToMenu()
    {
        UIManager.Instance.DisableObject("Crear");
        UIManager.Instance.DisableObject("Volver");
        UIManager.Instance.DisableObject("InputNombre");
        UIManager.Instance.DisableObject("TextoCrear");

        UIManager.Instance.EnableObject("Nueva");
        UIManager.Instance.EnableObject("Jugar");
        UIManager.Instance.EnableObject("TextoMenu");
        UIManager.Instance.EnableObject("CrearMenu");
        UIManager.Instance.EnableObject("Opciones");

        if (fileDataManager.file_exists() == false)
        {
            UIManager.Instance.DisableObject("Jugar");
        }
    }

    void disableButtons()
    {
        UIManager.Instance.DisableObject("Nueva");
        UIManager.Instance.DisableObject("Jugar");
        UIManager.Instance.DisableObject("CrearMenu");
    }
}
