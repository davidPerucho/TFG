using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class BoxEditor : MonoBehaviour
{
    [SerializeField]
    GameObject freeToken;

    [SerializeField]
    Transform freeTokenPosition;

    [SerializeField]
    GameObject addedToken;

    [SerializeField]
    Transform addedTokenPosition;

    bool eat = false;
    bool maxTokens = false;
    bool win = false;
    int tokensToWin = 1;
    int numMaxTokens = 1;
    int numTokens = 0;
    List<GameObject> freeTokensUI;
    List<GameObject> addedTokensUI;
    List<TablePlayerData> players;
    int id;

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
        Button eatButton = transform.Find("Comer").GetComponent<Button>();
        Button maxTokensButton = transform.Find("MaxFichas").GetComponent<Button>();
        Button cancelButton = transform.Find("Cancelar").GetComponent<Button>();
        Button saveButton = transform.Find("Guardar").GetComponent<Button>();
        Button winButton = transform.Find("Ganadora").GetComponent<Button>();

        id = int.Parse(transform.Find("ImagenCasilla").transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

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

    public void loadTokens(List<TablePlayerData> data)
    {
        freeTokensUI = new List<GameObject>();
        addedTokensUI = new List<GameObject>();

        players = data;

        foreach (TablePlayerData p in players)
        {
            foreach (TableTokenData t in p.tokens)
            {
                if (t.startingBoxId == id)
                {
                    GameObject newItem = Instantiate(addedToken, addedTokenPosition);
                    newItem.GetComponent<Image>().color = p.tokenColor;
                    addedTokensUI.Add(newItem);
                }

                if (t.startingBoxId == -1)
                {
                    GameObject freeItem = Instantiate(freeToken, freeTokenPosition);
                    freeItem.transform.Find("TextoJugador").GetComponent<TextMeshProUGUI>().SetText($"Jugador {p.id}:");
                    freeItem.transform.Find("TextoFicha").GetComponent<TextMeshProUGUI>().SetText($"Ficha {t.id}:");
                    freeItem.transform.Find("ImagenFicha").GetComponent<Image>().color = p.tokenColor;
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
