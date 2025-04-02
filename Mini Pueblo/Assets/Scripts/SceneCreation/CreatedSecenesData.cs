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
/// Enumeraci�n encargada de representar los diferentes tipos de escenas de juegos de mesa.
/// </summary>
public enum TableSceneType
{
    IA,
    MULTIPLAYER
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos para un minijuego de pintar.
/// </summary>
public class TableSceneData
{
    public string sceneName; //Nombre de la escena
    public SceneType sceneType; //Tipo de escena
    public TableSceneType tableSceneType; //Tipo de escena de juego de mesa
    public int numPlayers; //N�mero de jugadores
    public int numBoxes; //N�mero de casillas del tablero
    public string characterIndex; //Indice del personaje
    public string locationIndex; //Localizaci�n del personaje
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
