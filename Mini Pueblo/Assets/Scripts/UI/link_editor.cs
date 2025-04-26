using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase encargada de gestionar la edición de links del tablero.
/// </summary>
public class LinkEditor : MonoBehaviour
{
    [SerializeField]
    Button returnButton;

    [SerializeField]
    Button addLinkButton;

    [SerializeField]
    Button removeLinkButton;

    [SerializeField]
    Transform linksCreationList;

    [SerializeField]
    Transform linksList;

    [SerializeField]
    GameObject AddLinkUI;

    [SerializeField]
    GameObject PrefabLink;

    int minDice = 1;
    bool playerCondition = false;
    bool diceCondition = false;
    bool winnerLink = false;
    bool noWinner = false;

    public List<TableLinkData> createdLinks;
    List<TablePlayerData> players;
    List<TableBoxData> boxes;
    List<GameObject> linkObjects = new List<GameObject>();
    List<GameObject> linkCreationObjects = new List<GameObject>();
    List<int> usedBoxes = new List<int>();

    public static LinkEditor Instance { get; private set; } //Instancia de la clase

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
        //Obtengo las variables
        GameObject playerConditionButton = AddLinkUI.transform.Find("CondicionJugador").gameObject;
        GameObject playerDropdown = AddLinkUI.transform.Find("DropJugadores").gameObject;

        GameObject diceConditionButton = AddLinkUI.transform.Find("NumeroDado").gameObject;
        GameObject numDice = AddLinkUI.transform.Find("Tirada").gameObject;
        GameObject addDiceButton = AddLinkUI.transform.Find("AddTirada").gameObject;
        GameObject removeDiceButton = AddLinkUI.transform.Find("RemoveTirada").gameObject;

        GameObject winButton = AddLinkUI.transform.Find("LinkGanador").gameObject;

        //Añado la funcionalidad correspondiente a cada boton
        returnButton.onClick.AddListener(() =>
        {
            UIManager.Instance.EnableObject("Jugadores");
            UIManager.Instance.EnableObject("Tablero");
            UIManager.Instance.EnableObject("Links");
            UIManager.Instance.EnableObject("CrearTablero");

            gameObject.SetActive(false);
        });

        addLinkButton.onClick.AddListener(() =>
        {
            noWinner = false;

            AddLinkUI.SetActive(true);

            TMP_Dropdown boxDropdown = AddLinkUI.transform.Find("DropCasilla").GetComponent<TMP_Dropdown>();
            boxDropdown.options.Clear();
            foreach (TableBoxData b in boxes)
            {
                boxDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Casilla: {b.id}" });
            }

            if (boxDropdown.options.Count < 2)
            {
                boxDropdown.value = boxDropdown.options.Count - 1;
                boxDropdown.RefreshShownValue();
                boxDropdown.value = 0;
                boxDropdown.RefreshShownValue();
            }
            else if (boxDropdown.options.Count > 0)
            {
                boxDropdown.value = boxDropdown.options.Count - 1;
                boxDropdown.RefreshShownValue();
                boxDropdown.value = 0;
                boxDropdown.RefreshShownValue();
            }
        });

        removeLinkButton.onClick.AddListener(() =>
        {
            Destroy(linkObjects[linkObjects.Count - 1]);
            linkObjects.RemoveAt(linkObjects.Count - 1);

            createdLinks.RemoveAt(createdLinks.Count - 1);
        });

        AddLinkUI.transform.Find("Cancelar").GetComponent<Button>().onClick.AddListener(() =>
        {
            minDice = 1;
            playerCondition = false;
            diceCondition = false;
            winnerLink = false;

            numDice.GetComponent<TextMeshProUGUI>().text = minDice.ToString();

            playerConditionButton.GetComponent<Image>().color = Color.white;
            playerDropdown.SetActive(false);

            winButton.GetComponent<Image>().color = Color.white;

            diceConditionButton.GetComponent<Image>().color = Color.white;
            numDice.SetActive(false);
            addDiceButton.SetActive(false);
            removeDiceButton.SetActive(false);

            foreach (GameObject l in linkCreationObjects)
            {
                Destroy(l);
            }
            linkCreationObjects.Clear();
            usedBoxes.Clear();

            AddLinkUI.SetActive(false);
        });

        playerConditionButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            playerCondition = !playerCondition;

            if (playerCondition == true)
            {
                playerConditionButton.GetComponent<Image>().color = Color.green;
                playerDropdown.SetActive(true);
            }
            else
            {
                playerConditionButton.GetComponent<Image>().color = Color.white;
                playerDropdown.SetActive(false);
            }
        });

        winButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (noWinner == false)
            {
                winnerLink = true;
                winButton.GetComponent<Image>().color = Color.green;

                TMP_Dropdown boxDropdown = AddLinkUI.transform.Find("DropCasilla").GetComponent<TMP_Dropdown>();
                boxDropdown.options.Clear();
                foreach (TableBoxData b in boxes)
                {
                    if (b.winner == true)
                    {
                        boxDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Casilla: {b.id}" });
                    }
                }

                if (boxDropdown.options.Count < 2)
                {
                    boxDropdown.value = boxDropdown.options.Count - 1;
                    boxDropdown.RefreshShownValue();
                    boxDropdown.value = 0;
                    boxDropdown.RefreshShownValue();
                }
                else if (boxDropdown.options.Count > 0)
                {
                    boxDropdown.value = boxDropdown.options.Count - 1;
                    boxDropdown.RefreshShownValue();
                    boxDropdown.value = 0;
                    boxDropdown.RefreshShownValue();
                }
            }
        });

        diceConditionButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            diceCondition = !diceCondition;

            if (diceCondition == true)
            {
                diceConditionButton.GetComponent<Image>().color = Color.green;
                numDice.SetActive(true);
                addDiceButton.SetActive(true);
                removeDiceButton.SetActive(true);
            }
            else
            {
                diceConditionButton.GetComponent<Image>().color = Color.white;
                numDice.SetActive(false);
                addDiceButton.SetActive(false);
                removeDiceButton.SetActive(false);
            }
        });

        addDiceButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (minDice < 6)
            {
                minDice++;
                numDice.GetComponent<TextMeshProUGUI>().text = minDice.ToString();
            }
        });
        removeDiceButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (minDice > 1)
            {
                minDice--;
                numDice.GetComponent<TextMeshProUGUI>().text = minDice.ToString();
            }
        });

        AddLinkUI.transform.Find("RemoveCasilla").GetComponent<Button>().onClick.AddListener(() =>
        {
            int boxId = int.Parse(linkCreationObjects[linkCreationObjects.Count - 1].GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);

            if (linkCreationObjects.Count > 1)
            {
                Destroy(linkCreationObjects[linkCreationObjects.Count - 1]);
                linkCreationObjects.RemoveAt(linkCreationObjects.Count - 1);
                Destroy(linkCreationObjects[linkCreationObjects.Count - 1]);
                linkCreationObjects.RemoveAt(linkCreationObjects.Count - 1);
            }
            else
            {
                Destroy(linkCreationObjects[0]);
                linkCreationObjects.RemoveAt(0);
            }

            usedBoxes.Remove(boxId);
            TMP_Dropdown boxDropdown = AddLinkUI.transform.Find("DropCasilla").GetComponent<TMP_Dropdown>();
            boxDropdown.options.Clear();
            foreach (TableBoxData b in boxes)
            {
                if (usedBoxes.Contains(b.id) == false)
                {
                    boxDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Casilla: {b.id}" });
                }
            }

            if (boxDropdown.options.Count < 2)
            {
                boxDropdown.value = boxDropdown.options.Count - 1;
                boxDropdown.RefreshShownValue();
                boxDropdown.value = 0;
                boxDropdown.RefreshShownValue();
            }
            else if (boxDropdown.options.Count > 0)
            {
                boxDropdown.value = boxDropdown.options.Count - 1;
                boxDropdown.RefreshShownValue();
                boxDropdown.value = 0;
                boxDropdown.RefreshShownValue();
            }
        });

        AddLinkUI.transform.Find("AddCasilla").GetComponent<Button>().onClick.AddListener(addLink);
        AddLinkUI.transform.Find("AddLink").GetComponent<Button>().onClick.AddListener(saveLink);
    }

    /// <summary>
    /// Cargo los elementos del tablero ya creados y relleno los dropdowns.
    /// </summary>
    /// <param name="playersData">Datos de los jugadores.</param>
    /// <param name="linksData">Datos de los links.</param>
    /// <param name="boxesData">Datos de las casillas.</param>
    public void loadDropData(List<TablePlayerData> playersData, List<TableLinkData> linksData, List<TableBoxData> boxesData)
    {
        players = playersData;
        boxes = boxesData;
        if (createdLinks == null)
        {
            createdLinks = linksData;
        }
        else
        {
            createdLinks.AddRange(linksData);
        }

        TMP_Dropdown playerDropdown = AddLinkUI.transform.Find("DropJugadores").GetComponent<TMP_Dropdown>();
        playerDropdown.options.Clear();
        foreach (TablePlayerData p in players)
        {
            playerDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Jugador: {p.id}" });
        }

        TMP_Dropdown boxDropdown = AddLinkUI.transform.Find("DropCasilla").GetComponent<TMP_Dropdown>();
        boxDropdown.options.Clear();
        foreach (TableBoxData b in boxes)
        {
            boxDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Casilla: {b.id}" });
        }
    }

    /// <summary>
    /// Añade una nueva casilla al link teniendo en cuenta las condiciones del link
    /// </summary>
    void addLink()
    {
        noWinner = true;

        if (winnerLink == true || linkCreationObjects.Count < 2)
        {
            TMP_Dropdown playerDropdown = AddLinkUI.transform.Find("DropJugadores").GetComponent<TMP_Dropdown>();
            TMP_Dropdown boxDropdown = AddLinkUI.transform.Find("DropCasilla").GetComponent<TMP_Dropdown>();

            if (linkCreationObjects.Count > 0)
            {
                GameObject newItem2 = Instantiate(PrefabLink, linksCreationList);
                newItem2.GetComponent<TextMeshProUGUI>().text = "\u2193" + (winnerLink ? "WIN" : "")
                    + (playerCondition ? $" J{playerDropdown.options[playerDropdown.value].text.Split(' ')[1]}" : "")
                    + (diceCondition ? $" D{minDice}" : "");
                linkCreationObjects.Add(newItem2);
            }
            GameObject newItem1 = Instantiate(PrefabLink, linksCreationList);
            newItem1.GetComponent<TextMeshProUGUI>().text = boxDropdown.options[boxDropdown.value].text;
            linkCreationObjects.Add(newItem1);

            usedBoxes.Add(int.Parse(boxDropdown.options[boxDropdown.value].text.Split(' ')[1]));
            boxDropdown.options.Clear();
            foreach (TableBoxData b in boxes)
            {
                if (winnerLink == true)
                {
                    if (usedBoxes.Contains(b.id) == false && b.winner == true)
                    {
                        boxDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Casilla: {b.id}" });
                    }
                }
                else
                {
                    if (usedBoxes.Contains(b.id) == false)
                    {
                        boxDropdown.options.Add(new TMP_Dropdown.OptionData() { text = $"Casilla: {b.id}" });
                    }
                }
            }

            if (boxDropdown.options.Count < 2)
            {
                boxDropdown.value = boxDropdown.options.Count - 1;
                boxDropdown.RefreshShownValue();
                boxDropdown.value = 0;
                boxDropdown.RefreshShownValue();
            }
            else if (boxDropdown.options.Count > 0)
            {
                boxDropdown.value = boxDropdown.options.Count - 1;
                boxDropdown.RefreshShownValue();
                boxDropdown.value = 0;
                boxDropdown.RefreshShownValue();
            }
        }
    }

    /// <summary>
    /// Guarda los datos del link creado y los muestra por pantalla
    /// </summary>
    void saveLink()
    {
        if (linkCreationObjects.Count >= 3)
        {
            TMP_Dropdown playerDropdown = AddLinkUI.transform.Find("DropJugadores").GetComponent<TMP_Dropdown>();

            //Creo el link y su descripción, lo añado a la lista de links y muestro la descripción
            TableLinkData link = new TableLinkData();
            string linkType = "";
            string linkBoxes = "";
            string linkDescription = "";
            if (winnerLink == true)
            {
                linkType += "WIN ";

                link.winner = true;
                link.winnerBoxes = new List<int>();
                int i = 0;
                foreach (GameObject l in linkCreationObjects)
                {
                    if (i == linkCreationObjects.Count - 1)
                    {
                        linkBoxes += l.GetComponent<TextMeshProUGUI>().text.Split(' ')[1];

                        link.winnerBoxes.Add(int.Parse(l.GetComponent<TextMeshProUGUI>().text.Split(' ')[1]));
                    }
                    else if (i % 2 == 0)
                    {
                        linkBoxes += l.GetComponent<TextMeshProUGUI>().text.Split(' ')[1] + "-> ";

                        link.winnerBoxes.Add(int.Parse(l.GetComponent<TextMeshProUGUI>().text.Split(' ')[1]));
                    }
                    i++;
                }
            }
            else
            {
                link.fromId = int.Parse(linkCreationObjects[0].GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);
                link.toId = int.Parse(linkCreationObjects[2].GetComponent<TextMeshProUGUI>().text.Split(' ')[1]);

                linkBoxes += linkCreationObjects[0].GetComponent<TextMeshProUGUI>().text.Split(' ')[1] + " -> " + linkCreationObjects[2].GetComponent<TextMeshProUGUI>().text.Split(' ')[1];
            }
            if (playerCondition == true)
            {
                linkType += $"J{playerDropdown.options[playerDropdown.value].text.Split(' ')[1]} ";

                link.playerId = int.Parse(playerDropdown.options[playerDropdown.value].text.Split(' ')[1]);
            }
            else
            {
                link.playerId = -1;
            }
            if (diceCondition == true)
            {
                linkType += $"D{minDice} ";

                link.minNum = minDice;
            }
            else
            {
                link.minNum = -1;
            }
            createdLinks.Add(link);

            linkDescription = linkType + " | " + linkBoxes;
            GameObject newItem = Instantiate(PrefabLink, linksList);
            newItem.GetComponent<TextMeshProUGUI>().text = linkDescription;
            linkObjects.Add(newItem);

            //Cierro la interfaz de creacion y limpio los datos
            GameObject playerConditionButton = AddLinkUI.transform.Find("CondicionJugador").gameObject;

            GameObject diceConditionButton = AddLinkUI.transform.Find("NumeroDado").gameObject;
            GameObject numDice = AddLinkUI.transform.Find("Tirada").gameObject;
            GameObject addDiceButton = AddLinkUI.transform.Find("AddTirada").gameObject;
            GameObject removeDiceButton = AddLinkUI.transform.Find("RemoveTirada").gameObject;

            GameObject winButton = AddLinkUI.transform.Find("LinkGanador").gameObject;

            minDice = 1;
            playerCondition = false;
            diceCondition = false;
            winnerLink = false;

            numDice.GetComponent<TextMeshProUGUI>().text = minDice.ToString();

            playerConditionButton.GetComponent<Image>().color = Color.white;
            playerDropdown.gameObject.SetActive(false);

            winButton.GetComponent<Image>().color = Color.white;

            diceConditionButton.GetComponent<Image>().color = Color.white;
            numDice.SetActive(false);
            addDiceButton.SetActive(false);
            removeDiceButton.SetActive(false);

            foreach (GameObject l in linkCreationObjects)
            {
                Destroy(l);
            }
            linkCreationObjects.Clear();
            usedBoxes.Clear();

            AddLinkUI.SetActive(false);
        }
    }

    public void loadLinks(List<TableLinkData> links)
    {
        createdLinks = links;

        foreach (TableLinkData l in createdLinks)
        {
            string linkType = "";
            string linkBoxes = "";
            string linkDescription = "";

            if (l.winner == true)
            {
                linkType += "WIN ";

                int i = 0;
                foreach (int boxId in l.winnerBoxes)
                {
                    if (i == l.winnerBoxes.Count - 1)
                    {
                        linkBoxes += boxId.ToString();
                    }
                    else
                    {
                        linkBoxes += boxId.ToString() + "-> ";
                    }
                    i++;
                }
            }
            else
            {
                linkBoxes += l.fromId.ToString() + " -> " + l.toId.ToString();
            }
            if (l.playerId != -1)
            {
                linkType += $"J{l.playerId} ";
            }
            if (l.minNum != -1)
            {
                linkType += $"D{l.minNum} ";
            }

            linkDescription = linkType + " | " + linkBoxes;
            GameObject newItem = Instantiate(PrefabLink, linksList);
            newItem.GetComponent<TextMeshProUGUI>().text = linkDescription;
            linkObjects.Add(newItem);
        }
    }
}
