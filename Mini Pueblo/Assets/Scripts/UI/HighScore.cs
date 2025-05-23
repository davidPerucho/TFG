using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScore : MonoBehaviour, IDataPersistence
{
    List<int> scores; //Lista que almacena las mejores puntuaciones

    [SerializeField]
    int numScores = 5; //El número de mejores puntuaciones que se almacenaran
    [SerializeField]
    GameObject newRecordText; //Texto que se muestra cuando hay un nuevo record
    [SerializeField]
    GameObject scoresTitleText; //Texto que muestra el titulo de los records
    [SerializeField]
    GameObject scoresText; //Texto que muestra las mejores puntuaciones
    [SerializeField]
    Button tryAgainButton; //Botón para reiniciar el minijuego
    [SerializeField]
    Button exitButton; //Botón para salir del minijuego

    public static HighScore instance { get; private set; } //Instancia de la clase

    //Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error en singleton HighScore.");
            return;
        }

        instance = this;

        scores = new List<int>();
    }

    public void showHighScores(int newScore)
    {
        Time.timeScale = 0;

        UIManager.Instance.DisableObject("LifesText");
        UIManager.Instance.DisableObject("Tutorial");
        UIManager.Instance.EnableObject("ScoresTitle");
        UIManager.Instance.EnableObject("Scores");
        UIManager.Instance.EnableObject("BotonReintentar");
        UIManager.Instance.EnableObject("BotonSalir");
        
        if (addNewScore(newScore) == true)
        {
            UIManager.Instance.EnableObject("NewRecord");

            TextMeshProUGUI records = scoresText.GetComponent<TextMeshProUGUI>();
            records.text = "";

            int i = 1;
            foreach (int score in scores)
            {
                records.text += i.ToString() + ".  " + score.ToString() + "\n";
                i++;
            }
        }
        else
        {
            TextMeshProUGUI records = scoresText.GetComponent<TextMeshProUGUI>();
            records.text = "";

            int i = 1;
            foreach (int score in scores)
            {
                records.text += i.ToString() + ".  " + score.ToString() + "\n";
                i++;
            }
        }

        UIManager.Instance.AddListenerToButton("BotonSalir", exitMiniGame);
        UIManager.Instance.AddListenerToButton("BotonReintentar", retryMiniGame);
    }

    bool addNewScore(int score)
    {
        int high = 0;
        int low = 0;
        if (scores.Count > 0)
        {
            high = scores.Max();
        }
        if (scores.Count >= numScores)
        {
            low = scores.Min();
        }
        if (score >= low)
        {
            if (scores.Count == numScores)
            {
                scores.RemoveAt(numScores-1);
                scores.Add(score);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            else
            {
                scores.Add(score);
                scores = scores.OrderByDescending(x => x).ToList();
            }
        }
        if (score > high)
        {
            return true;
        }
        
        return false;
    }

    void exitMiniGame()
    {
        DataPersitence.instance.saveGame();
        Time.timeScale = 1;
        SceneManager.LoadScene("Hub");
    }

    void retryMiniGame()
    {
        DataPersitence.instance.saveGame();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadData(GameData data)
    {
        if (data.pointsShootingGame != null && data.pointsShootingGame.Count > 0)
        {
            scores = new List<int>(data.pointsShootingGame);
        }
        else
        {
            scores = new List<int>();
        }
        Debug.Log("Carga datos: " + string.Join(", ", data.pointsShootingGame) + "; puntos: " + string.Join(", ", scores));
    }

    public void saveData(ref GameData data)
    {
        data.pointsShootingGame = scores;
        Debug.Log("Guarda datos: " + string.Join(", ", data.pointsShootingGame) + "; puntos: " + string.Join(", ", scores));
    }
}
