using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableGameManager : MonoBehaviour
{
    [SerializeField]
    Transform playerTokenScroll;

    [SerializeField]
    Transform boardScroll;

    [SerializeField]
    GameObject tokenPrefab;

    TableSceneData table;
    int diceNum = 1;
    bool diceThrown = false;
    bool gameEnd = false;
    int currentlyPlaying = 0;
    List<GameObject> playerTokens = new List<GameObject>();

    void Awake()
    {
        //Obtengo el json con la información de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        //Leo la información  creo en la escena
        string json = File.ReadAllText(filePath);
        table = JsonUtility.FromJson<TableSceneData>(json);
    }

    void Start()
    {
        if (table.dice == true)
        {
            UIManager.Instance.EnableObject("DadoBoton");
            UIManager.Instance.EnableObject("Dado");

            UIManager.Instance.AddListenerToButton("DadoBoton", () => {
                if (diceThrown == false)
                {
                    diceNum = Random.Range(1, 7);
                    UIManager.Instance.SetText("Dado", diceNum.ToString());
                    diceThrown = true;
                }
            });
        }

        currentlyPlaying = 0;
        setPlayerInfo();
    }

    void Update()
    {
        if (!gameEnd)
        {
            gameLoop();
        }
    }

    void gameLoop()
    {

    }

    void setPlayerInfo()
    {
        UIManager.Instance.SetText("Turno", $"Turno jugador {table.players[currentlyPlaying].id}");

        //Funcionalidad jugador local
        if (table.players[currentlyPlaying].playerType == TablePlayerType.LOCAL)
        {
            UIManager.Instance.SetText("Instruccion", $"Selecciona una ficha");
        }
        //Funcionalidad IA
        else
        {
            UIManager.Instance.SetText("Instruccion", $"Jugando IA");
        }

        //Muestro las fichas disponibles del jugador
        foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
        {
            GameObject newItem = Instantiate(tokenPrefab, playerTokenScroll);
            newItem.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
            newItem.transform.Find("TextoFicha").GetComponent<TextMeshProUGUI>().text = $"Ficha {table.players[currentlyPlaying].id}";
            playerTokens.Add(newItem);
        }
    }
}
