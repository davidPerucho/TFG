using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Enumeraci�n encargada de representar los diferentes tipos de escenas.
/// </summary>
public enum SceneType
{
    PAINTING,
    SHOOTING,
    TABLE
}

[System.Serializable]
/// <summary>
/// Enumeraci�n encargada de representar los diferentes tipos de estilos de las imagenes para pintar.
/// </summary>
public enum PaintingStyle
{
    COLORBOOK,
    ABSTRACT,
    CUBIST
}

[System.Serializable]
/// <summary>
/// Enumeraci�n encargada de representar los diferentes tipos de escenas en las que pintar.
/// </summary>
public enum PaintingSceneType
{
    NORMAL,
    NUMBERS
}

[System.Serializable]
/// <summary>
/// Enumeraci�n encargada de representar los diferentes tipos de jugadores de juegos de mesa.
/// </summary>
public enum TablePlayerType
{
    IA,
    LOCAL
}

[System.Serializable]
/// <summary>
/// Enumeraci�n encargada de representar los diferentes tipos de escenas de juegos de mesa.
/// </summary>
public enum TableConditionType
{
    DICENUMBER,
    PLAYERTYPE
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar las condiciones de movimiento entre casillas
/// </summary>
public class TableLinkData
{
    public int toId; //Id de la casilla a la que se quiere ir
    public int fromId; //Id de la casilla desde la que parte el link
    public int minNum; //N�mero minimo que tiene que sacar el dado en caso de que esa sea la condici�n -1 si no
    public List<int> playerId; //Id de los jugadores en caso de que esa se la condici�n null si no
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos de las casillas del tablero
/// </summary>
public class TableBoxData
{
    public int id; //N�mero de la casilla
    public Vector2 position; //Posici�n de la casilla en el tablero
    public int backLinkId; //Link con la casilla de atr�s -1 si no existe
    public int maxTokens; //M�ximo n�mero de jugadores en la casilla -1 si es infinito
    public bool eat; //True si se comen las fichas
    public int tokensToWin; //N�mero de fichas que se necesitan en la casilla para ganar
    public bool winner; //True si es la casilla de la victoria
    public bool pushBack; //True si la casilla de la victoria tiene rebote
    public bool starter; //True si la casilla es una posible casilla de salida
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos de los jugadores
/// </summary>
public class TablePlayerData
{
    public int id; //N�mero de la casilla en la que se encuentra el token -1 si no se ha colocado
    public List<TableTokenData> tokens; //Lista de fichas
    public Color tokenColor; //Color de las fichas del jugador
    public TablePlayerType playerType; //Tipo de jugador
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos de las fichas del jugador
/// </summary>
public class TableTokenData
{
    public int id; //Id del token
    public int boxId; //N�mero de la casilla en la que se encuentra el token
    public int startingBoxId; //N�mero de la casilla de salida del token
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos para un minijuego de pintar.
/// </summary>
public class TableSceneData
{
    public string sceneName; //Nombre de la escena
    public SceneType sceneType; //Tipo de escena
    public int numPlayers; //N�mero de jugadores
    public int numBoxes; //N�mero de casillas del tablero
    public int numTokens; //N�mero de fichas de cada jugador
    public bool dice = true; //True si el juego cuenta con un dado
    public List<TableBoxData> boxes; //Casillas del tablero
    public List<TablePlayerData> players; //Jugadores
    public string characterIndex; //Indice del personaje
    public string locationIndex; //Localizaci�n del personaje
    public List<TableLinkData> links; //Links con otras casillas
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos para una actividad de pintar.
/// </summary>
public class PaintingSceneData
{
    public string sceneName; //Nombre de la escena
    public SceneType sceneType; //Tipo de escena
    public PaintingSceneType paintingSceneType; //Tipo de actividad de pintura
    public string sceneThemeEnglish; //Tema de las im�genes en ingl�s
    public string sceneThemeSpanish; //Tema de las im�genes en espa�ol
    public string characterIndex; //Indice del personaje
    public string locationIndex; //Localizaci�n del personaje
    public PaintingStyle paintingStyle; //Estilo de las im�genes generadas
}


[System.Serializable]
/// <summary>
/// Clase encargada de representar una lista de escenas creadas o en proceso.
/// </summary>
public class CreatedScenes
{
    public List<PaintingSceneData> paintingScenes = new List<PaintingSceneData>();
    public List<TableSceneData> tableScenes = new List<TableSceneData>();
}

[System.Serializable]
/// <summary>
/// Clase encargada de almacenar el nombre y el tipo de una escena.
/// </summary>
public class SceneTuple
{
    public string name; //Nombre de la escena
    public SceneType type; //Tipo de la escena
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar una lista de escenas creadas.
/// </summary>
public class ScenesTypes
{
    public List<SceneTuple> scenes = new List<SceneTuple>();
}
