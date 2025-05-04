using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    int selectedToken = -1;
    int selectedBox = -1;
    bool selectingBox = false;
    List<GameObject> playerTokens = new List<GameObject>();
    List<GameObject> boardBoxes = new List<GameObject>();

    public static TableGameManager Instance { get; private set; } //Instancia de la clase

    void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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
                if (diceThrown == false && selectedToken != -1)
                {
                    diceNum = Random.Range(1, 7);
                    UIManager.Instance.SetText("Dado", diceNum.ToString());
                    diceThrown = true;
                    StartCoroutine(playerThrow());
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

            boardBoxes.Add(newBox);        
        }
        viewBox(0);

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
            newItem.transform.Find("TextoFicha").GetComponent<TextMeshProUGUI>().text = $"Ficha {t.id}";
            newItem.transform.Find("Select").GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(selectToken(t));
            });
            if (t.startingBoxId == -1)
            {
                newItem.transform.Find("ViewToken").GetComponent<Button>().enabled = false;
            }
            else
            {
                newItem.transform.Find("ViewToken").GetComponent<Button>().onClick.AddListener(() =>
                {
                    viewBox(t.boxId);
                });
            }
            playerTokens.Add(newItem);
        }
    }

    IEnumerator selectToken(TableTokenData token)
    {
        if (token.startingBoxId == -1)
        {
            UIManager.Instance.SetText("Instruccion", $"Selecciona una casilla");
            selectedBox = -1;
            selectingBox = true;
            yield return new WaitUntil(() => selectedBox != -1);

            token.startingBoxId = selectedBox;
        }

        selectedToken = token.id;
        if (table.dice == true)
        {
            UIManager.Instance.SetText("Instruccion", $"Tira el dado");
        }
        else
        {
            StartCoroutine(playerMove());
        }
        viewBox(token.boxId);
    }

    /// <summary>
    /// Mueve el tablero para que enfoque a una casilla determinada.
    /// </summary>
    /// <param name="boxId">Id de la casilla a la que se quiere enfocar.</param>
    void viewBox(int boxId)
    {
        foreach (TableBoxData b in table.boxes)
        {
            if (b.id == boxId)
            {
                Canvas.ForceUpdateCanvases();
                Vector2 initialValue = (Vector2)boardBar.transform.InverseTransformPoint(boardBar.content.position) - (Vector2)boardBar.transform.InverseTransformPoint(b.position);
                boardBar.content.anchoredPosition = new Vector2(initialValue.x + boardBar.viewport.rect.width / 2, initialValue.y - (boardBar.viewport.rect.height + boardBar.viewport.rect.height / 2));
            }
        }
    }

    /// <summary>
    /// Mueve las fichas del jugador cuando no hay dado.
    /// </summary>
    IEnumerator playerMove()
    {
        UIManager.Instance.SetText("Instruccion", $"Selecciona una casilla");
        selectedBox = -1;
        selectingBox = true;
        yield return new WaitUntil(() => selectedBox != -1);
    }

    /// <summary>
    /// Se encarga de la tirada del jugador.
    /// </summary>
    IEnumerator playerThrow()
    {
        TableTokenData token = new TableTokenData();

        foreach (TablePlayerData p in table.players)
        {
            foreach (TableTokenData t in p.tokens)
            {
                if (t.id == selectedToken)
                {
                    token = t;
                }
            }
        }

        foreach (GameObject tokenUI in playerTokens)
        {
            tokenUI.transform.Find("Select").GetComponent<Button>().enabled = false;
            tokenUI.transform.Find("ViewToken").GetComponent<Button>().enabled = false;
        }

        //Muevo la ficha el número de casillas que marca el dado
        for (int i = 0; i < diceNum; i++)
        {
            List<TableLinkData> posibleLinks = new List<TableLinkData>();

            //Miro cuales son los links posibles
            foreach (TableLinkData l in table.links)
            {
                if (l.winner == false)
                {
                    if (l.fromId == token.boxId)
                    {
                        posibleLinks.Add(l);
                    }
                }
            }
            
            if (posibleLinks.Count == 1)
            {
                foreach (GameObject b in boardBoxes)
                {
                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);
                    if (posibleLinks[0].fromId == boxId)
                    {
                        Transform content = b.transform.Find("ContentFichas");
                        for (int j = 0; j < content.childCount; j++)
                        {
                            Transform child = content.GetChild(i);

                            if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                            {
                                Destroy(child.gameObject);
                            }
                        }
                    }
                    else if (posibleLinks[0].toId == boxId)
                    {
                        Transform content = b.transform.Find("ContentFichas");
                        token.boxId = boxId;
                        if (token.startingBoxId == -1)
                        {
                            token.startingBoxId = boxId;
                        }

                        GameObject item = Instantiate(tokenItemUI, content);
                        item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                    }
                }
            }
            else
            {
                UIManager.Instance.SetText("Instruccion", $"Selecciona una casilla");
                TableLinkData selectedLink = null;

                //Espero a que se seleccione una de las posibles casillas
                while (selectedLink == null)
                {
                    selectedBox = -1;
                    selectingBox = true;
                    yield return new WaitUntil(() => selectedBox != -1);

                    foreach (TableLinkData l in posibleLinks)
                    {
                        if (l.toId == selectedBox)
                        {
                            selectedLink = l;
                        }
                    }
                }

                foreach (GameObject b in boardBoxes)
                {
                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);
                    if (selectedLink.fromId == boxId)
                    {
                        Transform content = b.transform.Find("ContentFichas");
                        for (int j = 0; j < content.childCount; j++)
                        {
                            Transform child = content.GetChild(j);

                            if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                            {
                                Destroy(child.gameObject);
                            }
                        }
                    }
                    else if (selectedLink.toId == boxId)
                    {
                        Transform content = b.transform.Find("ContentFichas");
                        token.boxId = boxId;
                        if (token.startingBoxId == -1)
                        {
                            token.startingBoxId = boxId;
                        }

                        GameObject item = Instantiate(tokenItemUI, content);
                        item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                    }
                }
            }
        }

        TableLinkData link = null;
        //Miro primero los links automáticos
        foreach (TableLinkData l in table.links)
        {
            if (l.auto == true && l.fromId == token.boxId)
            {
                link = l;
                break;
            }
        }
        if (link != null)
        {
            foreach (GameObject b in boardBoxes)
            {
                int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);
                if (link.fromId == boxId)
                {
                    Transform content = b.transform.Find("ContentFichas");
                    for (int j = 0; j < content.childCount; j++)
                    {
                        Transform child = content.GetChild(j);

                        if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }
                else if (link.toId == boxId)
                {
                    Transform content = b.transform.Find("ContentFichas");
                    token.boxId = boxId;

                    GameObject item = Instantiate(tokenItemUI, content);
                    item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                }
            }
        }

        //Compruebo las condiciones de victoria
        List<TableLinkData> winnigLinks = new List<TableLinkData>();
        foreach (TableLinkData l in table.links)
        {
            if (l.winner == true)
            {
                winnigLinks.Add(l);
            }
        }
        
        //Si existen conexiones de victoria compruebo las conexiones, en caso contrario solo compruebo las casillas de victoria
        if (winnigLinks.Count > 0)
        {
            //Obtengo las posiciones de los tokens de los jugadores
            List<int> tokensPositions = new List<int>();
            foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
            {
                tokensPositions.Add(t.boxId);
            }

            //Compruebo si se ha completado alguno de los links
            foreach (TableLinkData l in winnigLinks)
            {
                bool linkCompleted = true;
                foreach (int boxId in l.winnerBoxes)
                {
                    if (tokensPositions.Contains(boxId))
                    {
                        linkCompleted = false;
                    }
                }

                if (linkCompleted == true)
                {
                    //VICTORIA
                }
            }
        }
        else
        {
            foreach (TableBoxData b in table.boxes)
            {
                if (token.boxId == b.id && b.winner == true)
                {
                    if (b.tokensToWin == 1)
                    {
                        //VICTORIA
                    }
                    else
                    {
                        if (b.tokensToWin <= numTokensOnBox(b.id))
                        {
                            //VICTORIA
                        }
                    }
                }
            }
        }

        currentlyPlaying++;
    }

    /// <summary>
    /// Cuenta cuantas fichas tiene el jugador actual en cierta casilla
    /// </summary>
    /// <param name="boxId">Id de la casilla que se quiere comprobar.</param>
    /// <returns>Número de fichas del jugador en la casilla.</returns>
    int numTokensOnBox(int boxId)
    {
        int i = 0;

        foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
        {
            if (t.boxId == boxId)
            {
                i++;
            }
        }

        return i;
    }

    /// <summary>
    /// Función que se llama al seleccionar una casilla y que guarda la información de la casilla seleccionada.
    /// </summary>
    /// <param name="boxId">Id de la casilla seleccionada.</param>
    public void selectBox(int boxId)
    {
        if (selectingBox == true)
        {
            selectedBox = boxId;
            selectingBox = false;
        }
        else
        {
            selectedBox = -1;
        }
    }
}
