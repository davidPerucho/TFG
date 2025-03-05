using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject yesNoUI;

    Scene createdScene;
    string sceneSavePath;

    void Awake()
    {
        //Agrego funciones de UI
        UIManager.Instance.AddListenerToButton("returnButton", () => { yesNoUI.SetActive(true); });
        UIManager.Instance.AddListenerToButton("No", () => { SceneManager.LoadScene("MainMenu"); });

        //Creo la nueva escena con el nombre insertado en el menu de creación
        string sceneName = PlayerPrefs.GetString("SceneName", "Minijuego");
        createdScene = SceneManager.CreateScene(sceneName);

        //Creo la ruta de guardado para la escena
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Scenes/"))) //Comprobar que el directorio existe
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Scenes/"));
            Debug.Log($"El directorio no existía, pero se ha creado");
        }
        sceneSavePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneName + ".json");

        string characterIndex = PlayerPrefs.GetString("SelectedNPC", "1");
        string locationIndex = PlayerPrefs.GetString("SelectedLocation", "1");

        Debug.Log("Nombre escena: " + sceneName + ", Index personaje: " + characterIndex + ", Index localización: " + locationIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
