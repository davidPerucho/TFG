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
    Transform linksList;

    [SerializeField]
    GameObject AddLinkUI;

    int minDice = 1;
    bool playerCondition = false;
    bool diceCondition = false;
    bool winnerLink = false;

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
            AddLinkUI.SetActive(true);
        });

        AddLinkUI.transform.Find("Cancelar").GetComponent<Button>().onClick.AddListener(() =>
        {
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
            winnerLink = !winnerLink;

            if (winnerLink == true)
            {
                winButton.GetComponent<Image>().color = Color.green;
            }
            else
            {
                winButton.GetComponent<Image>().color = Color.white;
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
    }
}
