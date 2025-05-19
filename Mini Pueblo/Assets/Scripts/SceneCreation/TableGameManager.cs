using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    GameObject victoryUI;

    [SerializeField]
    GameObject errorUI;

    [SerializeField]
    GameObject infoUI;

    [SerializeField]
    AudioClip tokenMoveSFX;

    [SerializeField]
    AudioClip diceToastSFX;

    [SerializeField]
    AudioClip victorySFX;

    TableSceneData table;
    int diceNum = 1;
    bool diceThrown = true;
    bool gameEnd = false;
    int currentlyPlaying = 0;
    int selectedToken = -1;
    int selectedBox = -1;
    bool selectingBox = false;
    List<GameObject> playerTokens = new List<GameObject>();
    List<GameObject> boardBoxes = new List<GameObject>();
    AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();

        victoryUI.transform.Find("Salir").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Hub");
        });

        errorUI.transform.Find("Salir").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Hub");
        });

        if (table.dice == true)
        {
            UIManager.Instance.EnableObject("DadoBoton");

            UIManager.Instance.AddListenerToButton("DadoBoton", () => { StartCoroutine(diceToast()); });
        }

        //Añado las casillas del tablero
        foreach (TableBoxData b in table.boxes)
        {
            GameObject newBox = Instantiate(boxPrefab, boardScroll);
            newBox.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text = b.id.ToString();

            newBox.transform.position = b.position;
            newBox.transform.SetParent(boardScroll);

            //Añado las imagenes de fondo de las casillas
            if (b.imagePath != null && b.imagePath != "")
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(b.imagePath, 1024);
                if (texture == null)
                {
                    Debug.LogWarning("No se pudo cargar la imagen");
                    return;
                }

                Sprite imageSprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                newBox.GetComponent<Image>().sprite = imageSprite;
            }

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

    /// <summary>
    /// Se encarga de realizar la tirada del dado.
    /// </summary>
    IEnumerator diceToast()
    {
        if (diceThrown == false && selectedToken != -1)
        {
            //Sonido del dado
            audioSource.PlayOneShot(diceToastSFX);

            //Asignación del número
            UIManager.Instance.EnableObject("Dado");
            diceNum = Random.Range(1, 7);
            UIManager.Instance.SetText("Dado", diceNum.ToString());
            diceThrown = true;

            yield return new WaitForSeconds(1);
            StartCoroutine(playerThrow());
        }
    }

    /// <summary>
    /// Muestra la información del jugador y realiza los ajustes pertinentes para la tirada.
    /// </summary>
    void setPlayerInfo()
    {
        foreach (GameObject t in playerTokens)
        {
            Destroy(t);
        }
        playerTokens.Clear();

        diceThrown = true;
        UIManager.Instance.SetText("Turno", $"Turno jugador {table.players[currentlyPlaying].id}");

        //Jugador local
        if (table.players[currentlyPlaying].playerType == TablePlayerType.LOCAL)
        {
            UIManager.Instance.SetText("Instruccion", $"Selecciona una ficha");
        }
        //Jugador IA
        else
        {
            UIManager.Instance.SetText("Instruccion", $"Jugando IA");
        }

        //Muestro las fichas disponibles del jugador
        foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
        {
            TableTokenData currentToken = t;

            GameObject newItem = Instantiate(tokenPrefab, playerTokenScroll);
            newItem.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
            newItem.transform.Find("TextoFicha").GetComponent<TextMeshProUGUI>().text = $"Ficha {currentToken.id}";

            //Funcionalidad botones
            if (table.players[currentlyPlaying].playerType == TablePlayerType.IA)
            {
                newItem.transform.Find("Select").GetComponent<Button>().gameObject.SetActive(false);
            }
            else
            {
                newItem.transform.Find("Select").GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectToken(currentToken);
                });
            }
            
            if (currentToken.startingBoxId == -1 || table.players[currentlyPlaying].playerType == TablePlayerType.IA)
            {
                newItem.transform.Find("ViewToken").GetComponent<Button>().gameObject.SetActive(false);
            }
            else
            {
                newItem.transform.Find("ViewToken").GetComponent<Button>().onClick.AddListener(() =>
                {
                    viewToken(currentToken);
                });
            }
            playerTokens.Add(newItem);
        }

        //Inicio el turno de la IA
        if (table.players[currentlyPlaying].playerType == TablePlayerType.IA)
        {
            int tokenindex = Random.Range(0, table.players[currentlyPlaying].tokens.Count);
            selectToken(table.players[currentlyPlaying].tokens[tokenindex]);
        }
    }

    /// <summary>
    /// Mueve la camara para centrar una ficha y resalta la ficha visualmente.
    /// </summary>
    /// <param name="token">Ficha que se quiere visualizar.</param>
    void viewToken(TableTokenData token)
    {
        foreach (GameObject b in boardBoxes)
        {
            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

            if (token.boxId == boxId)
            {
                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);

                    if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                    {
                        child.Find("LuzFicha").gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);

                    if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                    {
                        child.Find("LuzFicha").gameObject.SetActive(false);
                    }
                }
            }
        }
        viewBox(token.boxId);
    }

    /// <summary>
    /// Mueve la camara para centrar una ficha, resalta la ficha visualmente y selecciona la ficha iniciando el movimiento.
    /// </summary>
    /// <param name="token">Ficha que se quiere selccionar.</param>
    void selectToken(TableTokenData token)
    {
        foreach (GameObject p in playerTokens)
        {
            p.transform.Find("Select").GetComponent<Button>().gameObject.SetActive(false);
        }
        foreach (GameObject b in boardBoxes)
        {
            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

            if (token.boxId == boxId)
            {
                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);

                    if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                    {
                        child.Find("LuzFicha").gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);

                    if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                    {
                        child.Find("LuzFicha").gameObject.SetActive(false);
                    }
                }
            }
        }

        selectedToken = token.id;
        if (token.startingBoxId == -1)
        {
            StartCoroutine(playerDropToken());
        }
        else if (table.dice == true)
        {
            diceThrown = false;
            if (table.players[currentlyPlaying].playerType == TablePlayerType.IA)
            {
                diceToast();
            }
            else
            {
                UIManager.Instance.SetText("Instruccion", $"Tira el dado");
            }
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
        foreach (GameObject b in boardBoxes)
        {
            int id = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
            if (id == boxId)
            {
                Canvas.ForceUpdateCanvases();
                Vector2 initialValue = (Vector2)boardBar.transform.InverseTransformPoint(boardBar.content.position) - (Vector2)boardBar.transform.InverseTransformPoint(b.transform.position);
                boardBar.content.anchoredPosition = new Vector2(initialValue.x + boardBar.viewport.rect.width / 2, initialValue.y - boardBar.viewport.rect.height / 2);
            }
        }
    }

    /// <summary>
    /// Función encargada de colocar las fichas que no están todavía en el tablero.
    /// </summary>
    IEnumerator playerDropToken()
    {
        bool canDrop = false; //Indica si se puede depositar la ficha en la casilla seleccionada
        bool canEat = false; //Indica si la casilla a la que voy a mover la ficha permite comerfichas
        bool maxTokens = false; //Indica si la casilla a la que voy a mover la ficha tiene limitaciones de cantidad
        int numTokens = -1; //Cuantos tokens permite la casilla a la que voy a mover la ficha

        //Obtengo el token seleccionado
        TableTokenData token = new TableTokenData();

        foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
        {
            if (t.id == selectedToken)
            {
                token = t;
            }
        }

        while (canDrop == false)
        {
            foreach (GameObject tokenUI in playerTokens)
            {
                tokenUI.transform.Find("Select").GetComponent<Button>().enabled = false;
                tokenUI.transform.Find("ViewToken").GetComponent<Button>().enabled = false;
            }

            if (table.players[currentlyPlaying].playerType == TablePlayerType.LOCAL)
            {
                UIManager.Instance.SetText("Instruccion", $"Selecciona una casilla");
                selectedBox = -1;
                selectingBox = true;
                yield return new WaitUntil(() => selectedBox != -1);
            }
            else
            {
                selectingBox = true;
                selectedBox = Random.Range(0, table.numBoxes);
                selectingBox = false;
            }

            //Compruebo máximo número de fichas y la posibilidad de que se puedan comer fichas
            foreach (TableBoxData b in table.boxes)
            {
                if (selectedBox == b.id)
                {
                    if (b.maxTokens != -1)
                    {
                        maxTokens = true;
                        numTokens = b.maxTokens;
                    }
                    if (b.eat == true)
                    {
                        canEat = true;
                    }
                    break;
                }
            }

            //Miro cuantos tokens tiene la casilla y si se puede realizar el movimiento
            if (maxTokens == true)
            {
                int boxTokens = 0; //Número de ficahas que tiene la casilla
                List<int> playersWithTokens = new List<int>(); //Lista de jugadores con tokens en la casilla a la que se quiere mover
                foreach (TablePlayerData p in table.players)
                {
                    foreach (TableTokenData t in p.tokens)
                    {
                        if (t.boxId == selectedBox)
                        {
                            boxTokens++;
                            if (playersWithTokens.Contains(p.id) == false)
                            {
                                playersWithTokens.Add(p.id);
                            }
                        }
                    }
                }

                if (canEat == false && numTokens <= boxTokens)
                {
                    canDrop = false;
                }
                else if (canEat == true && (playersWithTokens.Count > 0 || playersWithTokens.Contains(table.players[currentlyPlaying].id) == false))
                {
                    canDrop = true;
                }
                else
                {
                    canDrop = true;
                }
            }
            else
            {
                canDrop = true;
            }
        }

        int nextBoxId = -1;
        foreach (GameObject b in boardBoxes)
        {
            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
            Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
            if (selectedBox == boxId)
            {
                //Compruebo si hay que comer alguna ficha
                if (canEat == true)
                {
                    nextBoxId = boxId;
                    for (int j = 0; j < content.childCount; j++)
                    {
                        Transform child = content.GetChild(j);

                        if (child.gameObject.GetComponent<Image>().color != table.players[currentlyPlaying].tokenColor)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }

                token.boxId = boxId;
                if (token.startingBoxId == -1)
                {
                    token.startingBoxId = boxId;
                }

                GameObject item = Instantiate(tokenItemUI, content);
                item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
            }
        }

        //En caso de que se haya comido alguna ficha la devuelvo a su origen
        if (nextBoxId != -1)
        {
            List<(int, Color)> updatedTokens = new List<(int, Color)>();
            foreach (TablePlayerData p in table.players)
            {
                foreach (TableTokenData t in p.tokens)
                {
                    if (t.boxId == nextBoxId && p.id != table.players[currentlyPlaying].id)
                    {
                        t.boxId = t.startingBoxId;
                        updatedTokens.Add((t.boxId, p.tokenColor));
                    }
                }
            }

            foreach ((int, Color) t in updatedTokens)
            {
                foreach (GameObject b in boardBoxes)
                {
                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                    Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                    if (t.Item1 == boxId)
                    {
                        GameObject item = Instantiate(tokenItemUI, content);
                        item.GetComponent<Image>().color = t.Item2;
                    }
                }
            }
        }

        //Reproduzco el sonido de la ficha y espero un segundo
        audioSource.PlayOneShot(tokenMoveSFX);
        yield return new WaitForSeconds(1);

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
                    if (tokensPositions.Contains(boxId) == false)
                    {
                        linkCompleted = false;
                    }
                }

                if (linkCompleted == true)
                {
                    //VICTORIA
                    audioSource.PlayOneShot(victorySFX);
                    victoryUI.SetActive(true);
                    victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                    gameEnd = true;
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
                        audioSource.PlayOneShot(victorySFX);
                        victoryUI.SetActive(true);
                        victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                        gameEnd = true;
                    }
                    else
                    {
                        if (b.tokensToWin <= numTokensOnBox(b.id))
                        {
                            //VICTORIA
                            audioSource.PlayOneShot(victorySFX);
                            victoryUI.SetActive(true);
                            victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                            gameEnd = true;
                        }
                    }
                }
            }
        }

        //Paso al siguiente turno
        if (gameEnd == false)
        {
            //Quito el resalte de la ficha seleccionada
            foreach (GameObject b in boardBoxes)
            {
                int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);
                    child.Find("LuzFicha").gameObject.SetActive(false);
                }
            }

            currentlyPlaying++;

            if (currentlyPlaying >= table.players.Count)
            {
                currentlyPlaying = 0;
            }
            setPlayerInfo();
        }
    }

    /// <summary>
    /// Mueve las fichas del jugador cuando no hay dado.
    /// </summary>
    IEnumerator playerMove()
    {
        bool canEat = false; //Indica si la casilla a la que voy a mover la ficha permite comerfichas
        bool maxTokens = false; //Indica si la casilla a la que voy a mover la ficha tiene limitaciones de cantidad
        int numTokens = -1; //Cuantos tokens permite la casilla a la que voy a mover la ficha
        bool canMove = true; //True si se puede hacer el movimiento a la próxima casilla
        bool error = false; //True si se ha producido un error durante el juego
        bool continueSelection = true; //True si se tiene que continuar con la selección de casilla

        //Obtengo el token seleccionado
        TableTokenData token = new TableTokenData();

        foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
        {
            if (t.id == selectedToken)
            {
                token = t;
            }
        }

        foreach (GameObject tokenUI in playerTokens)
        {
            tokenUI.transform.Find("Select").GetComponent<Button>().enabled = false;
            tokenUI.transform.Find("ViewToken").GetComponent<Button>().enabled = false;
        }

        List<TableLinkData> posibleLinks = new List<TableLinkData>();

        //Miro cuales son los links posibles
        foreach (TableLinkData l in table.links)
        {
            if (l.winner == false && l.auto == false)
            {
                if (l.fromId == token.boxId && (l.playerId == -1 || l.playerId == table.players[currentlyPlaying].id))
                {
                    posibleLinks.Add(l);
                }
            }
        }

        //Si no hay posibles caminos muestro un error
        if (posibleLinks.Count == 0)
        {
            //ERROR
            error = true;
        }
        else
        {
            while (continueSelection == true)
            {
                TableLinkData selectedLink = null;

                if (table.players[currentlyPlaying].playerType == TablePlayerType.IA)
                {
                    selectedLink = posibleLinks[Random.Range(0, posibleLinks.Count)];
                    selectedBox = selectedLink.toId;
                }
                else
                {
                    UIManager.Instance.SetText("Instruccion", $"Selecciona una casilla");

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
                }

                //Compruebo máximo número de fichas y la posibilidad de que se puedan comer fichas
                foreach (TableBoxData b in table.boxes)
                {
                    if (selectedLink.toId == b.id)
                    {
                        if (b.maxTokens != -1)
                        {
                            maxTokens = true;
                            numTokens = b.maxTokens;
                        }
                        if (b.eat == true)
                        {
                            canEat = true;
                        }
                        break;
                    }
                }

                //Miro cuantos tokens tiene la casilla y si se puede realizar el movimiento
                if (maxTokens == true)
                {
                    int boxTokens = 0; //Número de ficahas que tiene la casilla
                    List<int> playersWithTokens = new List<int>(); //Lista de jugadores con tokens en la casilla a la que se quiere mover
                    foreach (TablePlayerData p in table.players)
                    {
                        foreach (TableTokenData t in p.tokens)
                        {
                            if (t.boxId == selectedLink.toId)
                            {
                                boxTokens++;
                                if (playersWithTokens.Contains(p.id) == false)
                                {
                                    playersWithTokens.Add(p.id);
                                }
                            }
                        }
                    }

                    if (canEat == false && numTokens <= boxTokens)
                    {
                        canMove = false;
                    }
                    else if (canEat == true && (playersWithTokens.Count > 0 || playersWithTokens.Contains(table.players[currentlyPlaying].id) == false))
                    {
                        canMove = true;
                    }
                    else
                    {
                        canMove = true;
                    }
                }

                //Realizo el movimiento en caso de que sea posible
                if (canMove == true)
                {
                    continueSelection = false;
                    int nextBoxId = -1;
                    foreach (GameObject b in boardBoxes)
                    {
                        int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                        Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                        if (selectedLink.fromId == boxId)
                        {
                            for (int j = 0; j < content.childCount; j++)
                            {
                                Transform child = content.GetChild(j);

                                if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }
                        }
                        else if (selectedLink.toId == boxId)
                        {
                            //Compruebo si hay que comer alguna ficha
                            if (canEat == true)
                            {
                                nextBoxId = boxId;
                                for (int j = 0; j < content.childCount; j++)
                                {
                                    Transform child = content.GetChild(j);

                                    if (child.gameObject.GetComponent<Image>().color != table.players[currentlyPlaying].tokenColor)
                                    {
                                        Destroy(child.gameObject);
                                    }
                                }
                            }

                            token.boxId = boxId;
                            if (token.startingBoxId == -1)
                            {
                                token.startingBoxId = boxId;
                            }

                            GameObject item = Instantiate(tokenItemUI, content);
                            item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                        }
                    }

                    //En caso de que se haya comido alguna ficha la devuelvo a su origen
                    if (nextBoxId != -1)
                    {
                        List<(int, Color)> updatedTokens = new List<(int, Color)>();
                        foreach (TablePlayerData p in table.players)
                        {
                            foreach (TableTokenData t in p.tokens)
                            {
                                if (t.boxId == nextBoxId && p.id != table.players[currentlyPlaying].id)
                                {
                                    t.boxId = t.startingBoxId;
                                    updatedTokens.Add((t.boxId, p.tokenColor));
                                }
                            }
                        }

                        foreach ((int, Color) t in updatedTokens)
                        {
                            foreach (GameObject b in boardBoxes)
                            {
                                int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                                if (t.Item1 == boxId)
                                {
                                    GameObject item = Instantiate(tokenItemUI, content);
                                    item.GetComponent<Image>().color = t.Item2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (posibleLinks.Count == 1)
                    {
                        //BLOQUEO
                        infoUI.SetActive(true);
                        infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = "La ficha que quieres mover está bloqueada";
                        continueSelection = false;
                    }
                }
            }
        }

        //Reproduzco el sonido de la ficha y espero un segundo
        audioSource.PlayOneShot(tokenMoveSFX);
        yield return new WaitForSeconds(1);

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
            foreach (TableBoxData b in table.boxes)
            {
                if (link.toId == b.id)
                {
                    if (b.maxTokens != -1)
                    {
                        maxTokens = true;
                        numTokens = b.maxTokens;
                    }
                    if (b.eat == true)
                    {
                        canEat = true;
                    }
                    break;
                }
            }

            //Miro cuantos tokens tiene la casilla y si se puede realizar el movimiento
            if (maxTokens == true)
            {
                int boxTokens = 0; //Número de ficahas que tiene la casilla
                List<int> playersWithTokens = new List<int>(); //Lista de jugadores con tokens en la casilla a la que se quiere mover
                foreach (TablePlayerData p in table.players)
                {
                    foreach (TableTokenData t in p.tokens)
                    {
                        if (t.boxId == link.toId)
                        {
                            boxTokens++;
                            if (playersWithTokens.Contains(p.id) == false)
                            {
                                playersWithTokens.Add(p.id);
                            }
                        }
                    }
                }

                if (canEat == false && numTokens <= boxTokens)
                {
                    canMove = false;
                }
                else if (canEat == true && (playersWithTokens.Count > 0 || playersWithTokens.Contains(table.players[currentlyPlaying].id) == false))
                {
                    canMove = true;
                }
                else
                {
                    canMove = true;
                }
            }

            //Realizo el movimiento en caso de que sea posible
            if (canMove == true)
            {
                int nextBoxId = -1;
                foreach (GameObject b in boardBoxes)
                {
                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                    Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                    if (link.fromId == boxId)
                    {
                        for (int j = 0; j < content.childCount; j++)
                        {
                            Transform child = content.GetChild(j);

                            if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                            {
                                Destroy(child.gameObject);
                                break;
                            }
                        }
                    }
                    else if (link.toId == boxId)
                    {
                        //Compruebo si hay que comer alguna ficha
                        if (canEat == true)
                        {
                            nextBoxId = boxId;
                            for (int j = 0; j < content.childCount; j++)
                            {
                                Transform child = content.GetChild(j);

                                if (child.gameObject.GetComponent<Image>().color != table.players[currentlyPlaying].tokenColor)
                                {
                                    Destroy(child.gameObject);
                                }
                            }
                        }

                        token.boxId = boxId;
                        if (token.startingBoxId == -1)
                        {
                            token.startingBoxId = boxId;
                        }

                        GameObject item = Instantiate(tokenItemUI, content);
                        item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                    }
                }

                //En caso de que se haya comido alguna ficha la devuelvo a su origen
                if (nextBoxId != -1)
                {
                    List<(int, Color)> updatedTokens = new List<(int, Color)>();
                    foreach (TablePlayerData p in table.players)
                    {
                        foreach (TableTokenData t in p.tokens)
                        {
                            if (t.boxId == nextBoxId && p.id != table.players[currentlyPlaying].id)
                            {
                                t.boxId = t.startingBoxId;
                                updatedTokens.Add((t.boxId, p.tokenColor));
                            }
                        }
                    }

                    foreach ((int, Color) t in updatedTokens)
                    {
                        foreach (GameObject b in boardBoxes)
                        {
                            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                            Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                            if (t.Item1 == boxId)
                            {
                                GameObject item = Instantiate(tokenItemUI, content);
                                item.GetComponent<Image>().color = t.Item2;
                            }
                        }
                    }
                }

                //Reproduzco el sonido de la ficha y espero un segundo
                audioSource.PlayOneShot(tokenMoveSFX);
                yield return new WaitForSeconds(1);
            }
            else
            {
                //BLOQUEO
                infoUI.SetActive(true);
                infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = "La ficha que quieres mover está bloqueada";
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
                    if (tokensPositions.Contains(boxId) == false)
                    {
                        linkCompleted = false;
                    }
                }

                if (linkCompleted == true)
                {
                    //VICTORIA
                    audioSource.PlayOneShot(victorySFX);
                    victoryUI.SetActive(true);
                    victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                    gameEnd = true;
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
                        audioSource.PlayOneShot(victorySFX);
                        victoryUI.SetActive(true);
                        victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                        gameEnd = true;
                    }
                    else
                    {
                        if (b.tokensToWin <= numTokensOnBox(b.id))
                        {
                            //VICTORIA
                            audioSource.PlayOneShot(victorySFX);
                            victoryUI.SetActive(true);
                            victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                            gameEnd = true;
                        }
                    }
                }
            }
        }

        infoUI.SetActive(false);

        if (error == true && gameEnd == false)
        {
            gameEnd = true;
            errorUI.SetActive(true);
        }

        //Paso al siguiente turno
        if (gameEnd == false)
        {
            //Quito el resalte de la ficha seleccionada
            foreach (GameObject b in boardBoxes)
            {
                int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);
                    child.Find("LuzFicha").gameObject.SetActive(false);
                }
            }

            currentlyPlaying++;

            if (currentlyPlaying >= table.players.Count)
            {
                currentlyPlaying = 0;
            }
            setPlayerInfo();
        }
    }

    /// <summary>
    /// Se encarga de la tirada del jugador.
    /// </summary>
    IEnumerator playerThrow()
    {
        bool canEat = false; //Indica si la casilla a la que voy a mover la ficha permite comerfichas
        bool maxTokens = false; //Indica si la casilla a la que voy a mover la ficha tiene limitaciones de cantidad
        int numTokens = -1; //Cuantos tokens permite la casilla a la que voy a mover la ficha
        bool canMove = true; //True si se puede hacer el movimiento a la próxima casilla
        bool error = false; //True si se ha producido un error durante el juego

        //Obtengo el token seleccionado
        TableTokenData token = new TableTokenData();

        foreach (TableTokenData t in table.players[currentlyPlaying].tokens)
        {
            if (t.id == selectedToken)
            {
                token = t;
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
                if (l.winner == false && l.auto == false)
                {
                    if (l.fromId == token.boxId && (l.playerId == -1 || l.playerId == table.players[currentlyPlaying].id))
                    {
                        posibleLinks.Add(l);
                    }
                }
            }
            
            //Si no hay posibles caminos muestro un error
            if (posibleLinks.Count == 0)
            {
                //ERROR
                error = true;
            }
            //Si solo hay un camino posible muevo la ficha en esa dirección
            else if (posibleLinks.Count == 1)
            {
                //Compruebo que el número del dado sea suficiente
                if (posibleLinks[0].minNum <= diceNum - i || posibleLinks[0].minNum == -1)
                {
                    //Compruebo máximo número de fichas y la posibilidad de que se puedan comer fichas
                    foreach (TableBoxData b in table.boxes)
                    {
                        if (posibleLinks[0].toId == b.id)
                        {
                            if (b.maxTokens != -1)
                            {
                                maxTokens = true;
                                numTokens = b.maxTokens;
                            }
                            if (b.eat == true)
                            {
                                canEat = true;
                            }
                            break;
                        }
                    }

                    //Miro cuantos tokens tiene la casilla y si se puede realizar el movimiento
                    if (maxTokens == true)
                    {
                        int boxTokens = 0; //Número de ficahas que tiene la casilla
                        List<int> playersWithTokens = new List<int>(); //Lista de jugadores con tokens en la casilla a la que se quiere mover
                        foreach (TablePlayerData p in table.players)
                        {
                            foreach (TableTokenData t in p.tokens)
                            {
                                if (t.boxId == posibleLinks[0].toId)
                                {
                                    boxTokens++;
                                    if (playersWithTokens.Contains(p.id) == false)
                                    {
                                        playersWithTokens.Add(p.id);
                                    }
                                }
                            }
                        }

                        if (canEat == false && numTokens <= boxTokens)
                        {
                            canMove = false;
                        }
                        else if (canEat == true && (playersWithTokens.Count > 0 || playersWithTokens.Contains(table.players[currentlyPlaying].id) == false))
                        {
                            canMove = true;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }

                    //Realizo el movimiento en caso de que sea posible
                    if (canMove == true)
                    {
                        int nextBoxId = -1;
                        foreach (GameObject b in boardBoxes)
                        {
                            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                            Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                            if (posibleLinks[0].fromId == boxId)
                            {
                                for (int j = 0; j < content.childCount; j++)
                                {
                                    Transform child = content.GetChild(j);

                                    if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                                    {
                                        Destroy(child.gameObject);
                                        break;
                                    }
                                }
                            }
                            else if (posibleLinks[0].toId == boxId)
                            {
                                //Compruebo si hay que comer alguna ficha
                                if (canEat == true)
                                {
                                    nextBoxId = boxId;
                                    for (int j = 0; j < content.childCount; j++)
                                    {
                                        Transform child = content.GetChild(j);

                                        if (child.gameObject.GetComponent<Image>().color != table.players[currentlyPlaying].tokenColor)
                                        {
                                            Destroy(child.gameObject);
                                        }
                                    }
                                }

                                token.boxId = boxId;
                                if (token.startingBoxId == -1)
                                {
                                    token.startingBoxId = boxId;
                                }

                                GameObject item = Instantiate(tokenItemUI, content);
                                item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                            }
                        }

                        //En caso de que se haya comido alguna ficha la devuelvo a su origen
                        if (nextBoxId != -1)
                        {
                            List<(int, Color)> updatedTokens = new List<(int, Color)>();
                            foreach (TablePlayerData p in table.players)
                            {
                                foreach (TableTokenData t in p.tokens)
                                {
                                    if (t.boxId == nextBoxId && p.id != table.players[currentlyPlaying].id)
                                    {
                                        t.boxId = t.startingBoxId;
                                        updatedTokens.Add((t.boxId, p.tokenColor));
                                    }
                                }
                            }

                            foreach ((int, Color) t in updatedTokens)
                            {
                                foreach (GameObject b in boardBoxes)
                                {
                                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                                    Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                                    if (t.Item1 == boxId)
                                    {
                                        GameObject item = Instantiate(tokenItemUI, content);
                                        item.GetComponent<Image>().color = t.Item2;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //BLOQUEO
                        infoUI.SetActive(true);
                        infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = "La ficha que quieres mover está bloqueada";
                    }
                }
                else
                {
                    //INSUFICIENTE
                    infoUI.SetActive(true);
                    infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = $"Necesitas sacar un {posibleLinks[0].minNum} o más para poder salir de esta casilla";
                }
            }

            //En caso de que haya más de un camino posible espero a que el jugador seleccione la casilla que quiere y luego muevo la ficha
            else
            {
                TableLinkData selectedLink = null;

                if (table.players[currentlyPlaying].playerType == TablePlayerType.IA)
                {
                    selectedLink = posibleLinks[Random.Range(0, posibleLinks.Count)];
                    selectedBox = selectedLink.toId;
                }
                else
                {
                    UIManager.Instance.SetText("Instruccion", $"Selecciona una casilla");

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
                }

                //Compruebo que el número del dado sea suficiente
                if (selectedLink.minNum <= diceNum - i || selectedLink.minNum == -1)
                {
                    //Compruebo máximo número de fichas y la posibilidad de que se puedan comer fichas
                    foreach (TableBoxData b in table.boxes)
                    {
                        if (selectedLink.toId == b.id)
                        {
                            if (b.maxTokens != -1)
                            {
                                maxTokens = true;
                                numTokens = b.maxTokens;
                            }
                            if (b.eat == true)
                            {
                                canEat = true;
                            }
                            break;
                        }
                    }

                    //Miro cuantos tokens tiene la casilla y si se puede realizar el movimiento
                    if (maxTokens == true)
                    {
                        int boxTokens = 0; //Número de ficahas que tiene la casilla
                        List<int> playersWithTokens = new List<int>(); //Lista de jugadores con tokens en la casilla a la que se quiere mover
                        foreach (TablePlayerData p in table.players)
                        {
                            foreach (TableTokenData t in p.tokens)
                            {
                                if (t.boxId == selectedLink.toId)
                                {
                                    boxTokens++;
                                    if (playersWithTokens.Contains(p.id) == false)
                                    {
                                        playersWithTokens.Add(p.id);
                                    }
                                }
                            }
                        }

                        if (canEat == false && numTokens <= boxTokens)
                        {
                            canMove = false;
                        }
                        else if (canEat == true && (playersWithTokens.Count > 0 || playersWithTokens.Contains(table.players[currentlyPlaying].id) == false))
                        {
                            canMove = true;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }

                    //Realizo el movimiento en caso de que sea posible
                    if (canMove == true)
                    {
                        int nextBoxId = -1;
                        foreach (GameObject b in boardBoxes)
                        {
                            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                            Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                            if (selectedLink.fromId == boxId)
                            {
                                for (int j = 0; j < content.childCount; j++)
                                {
                                    Transform child = content.GetChild(j);

                                    if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                                    {
                                        Destroy(child.gameObject);
                                        break;
                                    }
                                }
                            }
                            else if (selectedLink.toId == boxId)
                            {
                                //Compruebo si hay que comer alguna ficha
                                if (canEat == true)
                                {
                                    nextBoxId = boxId;
                                    for (int j = 0; j < content.childCount; j++)
                                    {
                                        Transform child = content.GetChild(j);

                                        if (child.gameObject.GetComponent<Image>().color != table.players[currentlyPlaying].tokenColor)
                                        {
                                            Destroy(child.gameObject);
                                        }
                                    }
                                }

                                token.boxId = boxId;
                                if (token.startingBoxId == -1)
                                {
                                    token.startingBoxId = boxId;
                                }

                                GameObject item = Instantiate(tokenItemUI, content);
                                item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                            }
                        }

                        //En caso de que se haya comido alguna ficha la devuelvo a su origen
                        if (nextBoxId != -1)
                        {
                            List<(int, Color)> updatedTokens = new List<(int, Color)>();
                            foreach (TablePlayerData p in table.players)
                            {
                                foreach (TableTokenData t in p.tokens)
                                {
                                    if (t.boxId == nextBoxId && p.id != table.players[currentlyPlaying].id)
                                    {
                                        t.boxId = t.startingBoxId;
                                        updatedTokens.Add((t.boxId, p.tokenColor));
                                    }
                                }
                            }

                            foreach ((int, Color) t in updatedTokens)
                            {
                                foreach (GameObject b in boardBoxes)
                                {
                                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                                    Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                                    if (t.Item1 == boxId)
                                    {
                                        GameObject item = Instantiate(tokenItemUI, content);
                                        item.GetComponent<Image>().color = t.Item2;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //BLOQUEO
                        infoUI.SetActive(true);
                        infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = "La ficha que quieres mover está bloqueada";
                    }
                }
                else
                {
                    //INSUFICIENTE
                    infoUI.SetActive(true);
                    infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = $"Necesitas sacar un {posibleLinks[0].minNum} o más para poder salir de esta casilla";
                }
            }

            //Reproduzco el sonido de la ficha y espero un segundo
            audioSource.PlayOneShot(tokenMoveSFX);
            yield return new WaitForSeconds(1);
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
            foreach (TableBoxData b in table.boxes)
            {
                if (link.toId == b.id)
                {
                    if (b.maxTokens != -1)
                    {
                        maxTokens = true;
                        numTokens = b.maxTokens;
                    }
                    if (b.eat == true)
                    {
                        canEat = true;
                    }
                    break;
                }
            }

            //Miro cuantos tokens tiene la casilla y si se puede realizar el movimiento
            if (maxTokens == true)
            {
                int boxTokens = 0; //Número de ficahas que tiene la casilla
                List<int> playersWithTokens = new List<int>(); //Lista de jugadores con tokens en la casilla a la que se quiere mover
                foreach (TablePlayerData p in table.players)
                {
                    foreach (TableTokenData t in p.tokens)
                    {
                        if (t.boxId == link.toId)
                        {
                            boxTokens++;
                            if (playersWithTokens.Contains(p.id) == false)
                            {
                                playersWithTokens.Add(p.id);
                            }
                        }
                    }
                }

                if (canEat == false && numTokens <= boxTokens)
                {
                    canMove = false;
                }
                else if (canEat == true && (playersWithTokens.Count > 0 || playersWithTokens.Contains(table.players[currentlyPlaying].id) == false))
                {
                    canMove = true;
                }
                else
                {
                    canMove = true;
                }
            }

            //Realizo el movimiento en caso de que sea posible
            if (canMove == true)
            {
                int nextBoxId = -1;
                foreach (GameObject b in boardBoxes)
                {
                    int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                    Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                    if (link.fromId == boxId)
                    {
                        for (int j = 0; j < content.childCount; j++)
                        {
                            Transform child = content.GetChild(j);

                            if (child.gameObject.GetComponent<Image>().color == table.players[currentlyPlaying].tokenColor)
                            {
                                Destroy(child.gameObject);
                                break;
                            }
                        }
                    }
                    else if (link.toId == boxId)
                    {
                        //Compruebo si hay que comer alguna ficha
                        if (canEat == true)
                        {
                            nextBoxId = boxId;
                            for (int j = 0; j < content.childCount; j++)
                            {
                                Transform child = content.GetChild(j);

                                if (child.gameObject.GetComponent<Image>().color != table.players[currentlyPlaying].tokenColor)
                                {
                                    Destroy(child.gameObject);
                                }
                            }
                        }

                        token.boxId = boxId;
                        if (token.startingBoxId == -1)
                        {
                            token.startingBoxId = boxId;
                        }

                        GameObject item = Instantiate(tokenItemUI, content);
                        item.GetComponent<Image>().color = table.players[currentlyPlaying].tokenColor;
                    }
                }

                //En caso de que se haya comido alguna ficha la devuelvo a su origen
                if (nextBoxId != -1)
                {
                    List<(int, Color)> updatedTokens = new List<(int, Color)>();
                    foreach (TablePlayerData p in table.players)
                    {
                        foreach (TableTokenData t in p.tokens)
                        {
                            if (t.boxId == nextBoxId && p.id != table.players[currentlyPlaying].id)
                            {
                                t.boxId = t.startingBoxId;
                                updatedTokens.Add((t.boxId, p.tokenColor));
                            }
                        }
                    }

                    foreach ((int, Color) t in updatedTokens)
                    {
                        foreach (GameObject b in boardBoxes)
                        {
                            int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
                            Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                            if (t.Item1 == boxId)
                            {
                                GameObject item = Instantiate(tokenItemUI, content);
                                item.GetComponent<Image>().color = t.Item2;
                            }
                        }
                    }
                }

                //Reproduzco el sonido de la ficha y espero un segundo
                audioSource.PlayOneShot(tokenMoveSFX);
                yield return new WaitForSeconds(1);
            }
            else
            {
                //BLOQUEO
                infoUI.SetActive(true);
                infoUI.transform.Find("TextoInfo").GetComponent<TextMeshProUGUI>().text = "La ficha que quieres mover está bloqueada";
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
                    if (tokensPositions.Contains(boxId) == false)
                    {
                        linkCompleted = false;
                    }
                }

                if (linkCompleted == true)
                {
                    //VICTORIA
                    audioSource.PlayOneShot(victorySFX);
                    victoryUI.SetActive(true);
                    victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                    gameEnd = true;
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
                        audioSource.PlayOneShot(victorySFX);
                        victoryUI.SetActive(true);
                        victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                        gameEnd = true;
                    }
                    else
                    {
                        if (b.tokensToWin <= numTokensOnBox(b.id))
                        {
                            //VICTORIA
                            audioSource.PlayOneShot(victorySFX);
                            victoryUI.SetActive(true);
                            victoryUI.transform.Find("TextoVictoria").GetComponent<TextMeshProUGUI>().text = $"¡¡VICTORIA JUGADOR {table.players[currentlyPlaying].id}!!";
                            gameEnd = true;
                        }
                    }
                }
            }
        }

        infoUI.SetActive(false);
        
        if (error == true && gameEnd == false)
        {
            gameEnd = true;
            errorUI.SetActive(true);
        }

        //Paso al siguiente turno
        if (gameEnd == false)
        {
            //Quito el resalte de la ficha seleccionada
            foreach (GameObject b in boardBoxes)
            {
                int boxId = int.Parse(b.transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

                Transform content = b.transform.Find("Scroll View/Viewport/ContentFichas");
                for (int j = 0; j < content.childCount; j++)
                {
                    Transform child = content.GetChild(j);
                    child.Find("LuzFicha").gameObject.SetActive(false);
                }
            }

            currentlyPlaying++;

            if (currentlyPlaying >= table.players.Count)
            {
                currentlyPlaying = 0;
            }
            setPlayerInfo();
        }
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
