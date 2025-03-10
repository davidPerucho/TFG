using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject yesNoUI;

    Scene createdScene;
    string sceneSavePath;
    (float,float)[] characterCoordinates = { (-4.985f, 9.2435f), (18.265f, 11.573f), (27.105f, 5.383f), (-12.32f, 4.47f), (0.915f, -9.537f), (12.705f, -5.847f), (27.315f, -12.407f) };

    void Awake()
    {
        //Creo la nueva escena con el nombre insertado en el menu de creación
        string sceneName = PlayerPrefs.GetString("SceneName", "Minijuego");
        createdScene = SceneManager.CreateScene(sceneName);
        addCamera(); //Añado una cámara a la escena

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

    void Start()
    {
        //Agrego funciones de UI
        UIManager.Instance.AddListenerToButton("returnButton", () => { yesNoUI.SetActive(true); });
        UIManager.Instance.AddListenerToButton("No", () => { SceneManager.LoadScene("MainMenu"); });
        UIManager.Instance.AddListenerToButton("Si", saveScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void saveScene()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(sceneSavePath));
            string dataToStore = JsonUtility.ToJson(createdScene, true);

            using (FileStream stream = new FileStream(sceneSavePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            SceneManager.LoadScene("MainMenu");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void addCamera()
    {
        //Creo la cámara
        GameObject cameraObject = new GameObject("Camera");
        Camera cameraComponent = cameraObject.AddComponent<Camera>();

        cameraComponent.clearFlags = CameraClearFlags.Skybox; //Configuro la vista
        cameraComponent.backgroundColor = Color.blue; //Configuro el color de fondo
        cameraComponent.orthographic = false;
        cameraObject.tag = "MainCamera"; //Añado un tag para que Unity la reconozca como la cámara principal

        SceneManager.MoveGameObjectToScene(cameraObject, createdScene); //Añado la cámara a la escena
    }
}
