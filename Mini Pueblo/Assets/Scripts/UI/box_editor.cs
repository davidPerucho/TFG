using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase encargada de gestionar la edición de casillas del tablero
/// </summary>
public class BoxEditor : MonoBehaviour
{
    [SerializeField]
    GameObject freeToken; //Prefab de las fichas disponibles

    [SerializeField]
    Transform freeTokenPosition; //Lugar en el que añadir las fichas disponibles

    [SerializeField]
    GameObject addedToken; //Prefab de los tokens añadidos

    [SerializeField]
    Transform addedTokenPosition; //Lugar en el que añadir las fichas

    bool eat = false; //True si se puede comer en esta casilla
    bool maxTokens = false; //True si existe un máximo número de fichas en la casilla
    bool win = false; //True si es una casilla de victoria
    int tokensToWin = 1; //Número de fichas necesarias para ganar si es una casilla de victoria
    int numMaxTokens = 1; //Número máximo de fichas que puede haber en la casilla
    int numTokens = 0; //Número de tokens en la casilla
    List<GameObject> freeTokensUI; //Lista de las fichas disponibles
    List<GameObject> addedTokensUI; //Lista de las fichas añadidas
    List<TablePlayerData> players; //Datos de los jugadores
    int id; //Id de la casilla que se está editando

    public static BoxEditor Instance { get; private set; } //Instancia de la clase

    void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Obtengo los botones de la interfaz
        Button eatButton = transform.Find("Comer").GetComponent<Button>();
        Button maxTokensButton = transform.Find("MaxFichas").GetComponent<Button>();
        Button cancelButton = transform.Find("Cancelar").GetComponent<Button>();
        Button saveButton = transform.Find("Guardar").GetComponent<Button>();
        Button winButton = transform.Find("Ganadora").GetComponent<Button>();

        //Obtengo el id de la casilla
        id = int.Parse(transform.Find("ImagenCasilla").transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

        //Añado la funcionalidad correspondiente a cada boton
        eatButton.onClick.AddListener(() =>
        {
            eat = !eat;

            if (eat == true)
            {
                transform.Find("Comer").GetComponent<Image>().color = Color.green;
            }
            else
            {
                transform.Find("Comer").GetComponent<Image>().color = Color.white;
            }
        });
        maxTokensButton.onClick.AddListener(() =>
        {
            maxTokens = !maxTokens;

            if (maxTokens == true)
            {
                UIManager.Instance.EnableObject("MaxFichasText");
                UIManager.Instance.EnableObject("AddMaxFichas");
                UIManager.Instance.EnableObject("RemoveMaxFichas");
                transform.Find("MaxFichas").GetComponent<Image>().color = Color.green;
            }
            else
            {
                UIManager.Instance.DisableObject("MaxFichasText");
                UIManager.Instance.DisableObject("AddMaxFichas");
                UIManager.Instance.DisableObject("RemoveMaxFichas");
                transform.Find("MaxFichas").GetComponent<Image>().color = Color.white;
            }
        });
        winButton.onClick.AddListener(() =>
        {
            win = !win;

            if (win == true)
            {
                UIManager.Instance.EnableObject("GanarText");
                UIManager.Instance.EnableObject("FichasGanarText");
                UIManager.Instance.EnableObject("AddFichasGanar");
                UIManager.Instance.EnableObject("RemoveFichasGanar");
                transform.Find("Ganadora").GetComponent<Image>().color = Color.green;
            }
            else
            {
                UIManager.Instance.DisableObject("GanarText");
                UIManager.Instance.DisableObject("FichasGanarText");
                UIManager.Instance.DisableObject("AddFichasGanar");
                UIManager.Instance.DisableObject("RemoveFichasGanar");
                transform.Find("Ganadora").GetComponent<Image>().color = Color.white;
            }
        });
        cancelButton.onClick.AddListener(() =>
        {
            foreach (GameObject free in freeTokensUI)
            {
                Destroy(free);
            }

            foreach (GameObject added in addedTokensUI)
            {
                Destroy(added);
            }

            gameObject.SetActive(false);
        });
        saveButton.onClick.AddListener(() =>
        {
            CreationManager.Instance.loadEditingBoxData(players, eat, maxTokens, numMaxTokens, win, tokensToWin, id);

            foreach (GameObject free in freeTokensUI)
            {
                Destroy(free);
            }

            foreach (GameObject added in addedTokensUI)
            {
                Destroy(added);
            }

            gameObject.SetActive(false);
        });

        UIManager.Instance.AddListenerToButton("AddMaxFichas", () =>
        {
            numMaxTokens++;
            UIManager.Instance.SetText("MaxFichasText", numMaxTokens.ToString());
        });
        UIManager.Instance.AddListenerToButton("RemoveMaxFichas", () =>
        {
            if (numMaxTokens > 1)
            {
                numMaxTokens--;
                UIManager.Instance.SetText("MaxFichasText", numMaxTokens.ToString());
            }
        });
        UIManager.Instance.AddListenerToButton("AddFichasGanar", () =>
        {
            tokensToWin++;
            UIManager.Instance.SetText("FichasGanarText", tokensToWin.ToString());
        });
        UIManager.Instance.AddListenerToButton("RemoveFichasGanar", () =>
        {
            if (tokensToWin > 1)
            {
                tokensToWin--;
                UIManager.Instance.SetText("FichasGanarText", tokensToWin.ToString());
            }
        });
    }

    /// <summary>
    /// Carga las fichas disponibles y añadidas a la casilla en la interfaz.
    /// </summary>
    /// <param name="data">Datos de los jugadores con sus fichas.</param>
    public void loadTokens(List<TablePlayerData> data)
    {
        freeTokensUI = new List<GameObject>();
        addedTokensUI = new List<GameObject>();

        players = data;

        foreach (TablePlayerData p in players)
        {
            foreach (TableTokenData t in p.tokens)
            {
                //Cargo las fichas que ya han sido añadidas a la casilla
                if (t.startingBoxId == id)
                {
                    GameObject newItem = Instantiate(addedToken, addedTokenPosition);
                    newItem.GetComponent<Image>().color = p.tokenColor;
                    addedTokensUI.Add(newItem);
                }

                //Cargo la lista de fichas disponibles
                if (t.startingBoxId == -1)
                {
                    GameObject freeItem = Instantiate(freeToken, freeTokenPosition);
                    freeItem.transform.Find("TextoJugador").GetComponent<TextMeshProUGUI>().SetText($"Jugador {p.id}:");
                    freeItem.transform.Find("TextoFicha").GetComponent<TextMeshProUGUI>().SetText($"Ficha {t.id}:");
                    freeItem.transform.Find("ImagenFicha").GetComponent<Image>().color = p.tokenColor;

                    //Funcionalidad para añadir una de las fichas disponibles a la casilla
                    freeItem.transform.Find("BotonFicha").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (maxTokens == false || (maxTokens == true && numTokens < numMaxTokens))
                        {
                            t.boxId = id;
                            t.startingBoxId = id;
                            GameObject newItem = Instantiate(addedToken, addedTokenPosition);
                            newItem.GetComponent<Image>().color = p.tokenColor;

                            int i = 0;
                            foreach (GameObject free in freeTokensUI)
                            {
                                if (free == freeItem)
                                {
                                    Destroy(free);
                                    freeTokensUI.RemoveAt(i);
                                    break;
                                }
                                i++;
                            }
                            addedTokensUI.Add(newItem);
                            numTokens++;
                        }
                    });
                    freeTokensUI.Add(freeItem);
                }
            }
        }
    }
}
