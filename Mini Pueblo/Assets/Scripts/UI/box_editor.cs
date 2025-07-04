using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using SFB;

/// <summary>
/// Clase encargada de gestionar la edici�n de casillas del tablero
/// </summary>
public class BoxEditor : MonoBehaviour
{
    [SerializeField]
    GameObject freeToken; //Prefab de las fichas disponibles

    [SerializeField]
    Transform freeTokenPosition; //Lugar en el que a�adir las fichas disponibles

    [SerializeField]
    GameObject addedToken; //Prefab de los tokens a�adidos

    [SerializeField]
    Transform addedTokenPosition; //Lugar en el que a�adir las fichas

    bool eat = false; //True si se puede comer en esta casilla
    bool maxTokens = false; //True si existe un m�ximo n�mero de fichas en la casilla
    bool win = false; //True si es una casilla de victoria
    int tokensToWin = 1; //N�mero de fichas necesarias para ganar si es una casilla de victoria
    int numMaxTokens = 1; //N�mero m�ximo de fichas que puede haber en la casilla
    int numTokens = 0; //N�mero de tokens en la casilla
    List<GameObject> freeTokensUI; //Lista de las fichas disponibles
    List<GameObject> addedTokensUI; //Lista de las fichas a�adidas
    List<TablePlayerData> players; //Datos de los jugadores
    int id; //Id de la casilla que se est� editando
    string imagePath = null; //Ruta de la imagen de la casilla 

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

        if (Application.platform == RuntimePlatform.Android)
        {
            transform.Find("AddImagen").GetComponent<Button>().onClick.AddListener(addImageAndroid);
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            transform.Find("AddImagen").GetComponent<Button>().onClick.AddListener(addImageWindows);
        }

        //Obtengo el id de la casilla
        id = int.Parse(transform.Find("ImagenCasilla").transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);

        //A�ado la funcionalidad correspondiente a cada boton
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
            id = int.Parse(transform.Find("ImagenCasilla").transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
            CreationManager.Instance.loadEditingBoxData(players, eat, maxTokens, numMaxTokens, win, tokensToWin, id, imagePath);

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
    /// Carga las fichas disponibles y a�adidas a la casilla en la interfaz.
    /// </summary>
    /// <param name="data">Datos de los jugadores con sus fichas.</param>
    public void loadTokens(List<TablePlayerData> data)
    {
        freeTokensUI = new List<GameObject>();
        addedTokensUI = new List<GameObject>();
        id = int.Parse(transform.Find("ImagenCasilla").transform.Find("TextoCasilla").GetComponent<TextMeshProUGUI>().text);
        players = data;

        foreach (TablePlayerData p in players)
        {
            foreach (TableTokenData t in p.tokens)
            {
                //Cargo las fichas que ya han sido a�adidas a la casilla
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

                    //Funcionalidad para a�adir una de las fichas disponibles a la casilla
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

    /// <summary>
    /// Carga los datos de la interfaz para una casilla en concreto.
    /// </summary>
    /// <param name="box">Datos de la casilla.</param>
    public void loadBoxUI(TableBoxData box)
    {
        if (box.winner == true)
        {
            win = true;
            transform.Find("Ganadora").GetComponent<Image>().color = Color.green;
            UIManager.Instance.EnableObject("GanarText");
            UIManager.Instance.EnableObject("FichasGanarText");
            UIManager.Instance.EnableObject("AddFichasGanar");
            UIManager.Instance.EnableObject("RemoveFichasGanar");

            tokensToWin = box.tokensToWin;
            UIManager.Instance.SetText("FichasGanarText", tokensToWin.ToString());
        }
        else
        {
            win = false;
            transform.Find("Ganadora").GetComponent<Image>().color = Color.white;
            UIManager.Instance.DisableObject("GanarText");
            UIManager.Instance.DisableObject("FichasGanarText");
            UIManager.Instance.DisableObject("AddFichasGanar");
            UIManager.Instance.DisableObject("RemoveFichasGanar");
            transform.Find("Ganadora").GetComponent<Image>().color = Color.white;
        }
        if (box.eat == true)
        {
            eat = true;
            transform.Find("Comer").GetComponent<Image>().color = Color.green;
        }
        else
        {
            eat = false;
            transform.Find("Comer").GetComponent<Image>().color = Color.white;
        }
        if (box.maxTokens > 0)
        {
            maxTokens = true;
            numMaxTokens = box.maxTokens;

            UIManager.Instance.EnableObject("MaxFichasText");
            UIManager.Instance.EnableObject("AddMaxFichas");
            UIManager.Instance.EnableObject("RemoveMaxFichas");
            transform.Find("MaxFichas").GetComponent<Image>().color = Color.green;
            UIManager.Instance.SetText("MaxFichasText", numMaxTokens.ToString());
        }
        else
        {
            UIManager.Instance.DisableObject("MaxFichasText");
            UIManager.Instance.DisableObject("AddMaxFichas");
            UIManager.Instance.DisableObject("RemoveMaxFichas");
            transform.Find("MaxFichas").GetComponent<Image>().color = Color.white;
        }
        if (box.imagePath != null && box.imagePath != "")
        {
            imagePath = box.imagePath;
            Texture2D texture = NativeGallery.LoadImageAtPath(imagePath, 1024);
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

            transform.Find("ImagenCasilla").GetComponent<Image>().sprite = imageSprite;
        }
        else
        {
            transform.Find("ImagenCasilla").GetComponent<Image>().sprite = null;
        }
    }

    /// <summary>
    /// Muestra una selecci�n de imagen del dispositivo y la a�ade al fondo de la casilla para android.
    /// </summary>
    void addImageAndroid()
    {
        NativeGallery.RequestPermissionAsync((perm) =>
        {
            if (perm == NativeGallery.Permission.Granted)
            {
                NativeGallery.GetImageFromGallery((path) =>
                {
                    if (path != null && path != "")
                    {
                        imagePath = path;
                        Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024);
                        if (texture == null)
                        {
                            Debug.LogWarning("No se ha podido cargar la imagen");
                            return;
                        }

                        Sprite imageSprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f)
                        );

                        transform.Find("ImagenCasilla").GetComponent<Image>().sprite = imageSprite;
                    }
                }, "Selecciona una imagen", "image/*");
            }
            else
            {
                Debug.LogWarning("Permiso denegado para acceder a la galer�a.");
            }
        }, NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
    }

    /// <summary>
    /// Muestra una selecci�n de imagen del dispositivo y la a�ade al fondo de la casilla para windows.
    /// </summary>
    void addImageWindows()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };

        // Abrir selector de archivos
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Selecciona una imagen", "", extensions, false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            imagePath = paths[0];
            byte[] imageData = File.ReadAllBytes(imagePath);

            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                transform.Find("ImagenCasilla").GetComponent<Image>().sprite = sprite;
            }
            else
            {
                Debug.LogWarning("No se pudo cargar la imagen");
            }
        }
    }
}
