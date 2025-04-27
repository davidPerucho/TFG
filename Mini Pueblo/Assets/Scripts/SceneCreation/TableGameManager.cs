using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableGameManager : MonoBehaviour
{
    TableSceneData table;
    int diceNum = 1;
    bool diceThrown = false;
    bool gameEnd = false;
    int currentlyPlaying = 0;

    void Awake()
    {
        //Obtengo el json con la información de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        //Leo la información  creo en la escena
        string json = File.ReadAllText(filePath);
        table = JsonUtility.FromJson<TableSceneData>(json);

        Debug.Log("Njugadores: " + table.numPlayers + "Ncasillas: " + table.numBoxes + "Nfichas: " + table.numTokens);
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
    }
}
