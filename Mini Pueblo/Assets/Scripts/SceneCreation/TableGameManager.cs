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

    [SerializeField]
    GameObject boxPrefab;

    [SerializeField]
    GameObject tokenItemUI;

    [SerializeField]
    ScrollRect boardBar;

    [SerializeField]
    ScrollRect playerBar;

    TableSceneData table;
    int diceNum = 1;
    bool diceThrown = false;
    bool gameEnd = false;
    int currentlyPlaying = 0;
    List<GameObject> playerTokens = new List<GameObject>();
    List<GameObject> boardBoxes = new List<GameObject>();

    void Awake()
    {
        //Obtengo el json con la información de la escena que se quiere cargar
        string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "");
        string filePath = Path.Combine(Application.persistentDataPath, "Scenes/" + sceneToLoad + ".json");

        //Leo la información  creo en la escena
        string json = File.ReadAllText(filePath);
        table = JsonUtility.FromJson<TableSceneData>(json);

        boardScroll.GetComponent<RectTransform>().anchoredPosition = new Vector2(-700f, 768.5f);
        playerBar.verticalNormalizedPosition = 1f;
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

        //Añado las casillas del tablero
        foreach (TableBoxData b in table.boxes)
        {
            GameObject newBox = Instantiate(boxPrefab, boardScroll);
            newBox.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text = b.id.ToString();

            newBox.transform.position = b.position;
            newBox.transform.SetParent(boardScroll);

            //Añado las posibles fichas a la casilla
            foreach (TablePlayerData p in table.players)
            {
                foreach (TableTokenData t in p.tokens)
                {
                    if (t.startingBoxId == b.id)
                    {
                        Transform tokenPosition = newBox.GetComponentInChildren<ScrollRect>().content;
                        GameObject newItem = Instantiate(tokenItemUI, tokenPosition);
                        newItem.GetComponent<Image>().color = p.tokenColor;
                    }
                }
            }

            boardBoxes.Add(newBox);        }
        boardBar.verticalNormalizedPosition = 1f;

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
