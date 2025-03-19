using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Enumeración encargada de representar los diferentes tipos de escenas.
/// </summary>
public enum SceneType
{
    PAINTING,
    SHOOTING,
    TABLE
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar los datos para una actividad de pintar.
/// </summary>
public class PaintingSceneData
{
    public string sceneName; //Nombre de la escena
    public SceneType sceneType; //Tipo de escena
    public string sceneThemeEnglish; //Tema de las imágenes en inglés
    public string sceneThemeSpanish; //Tema de las imágenes en español
    public string characterIndex; //Indice del personaje
    public string locationIndex; //Localización del personaje
    public List<PaintingTools> paintingTools; //Herramientas para pintar de las que dispone la escena
}


[System.Serializable]
/// <summary>
/// Clase encargada de representar una lista de escenas creadas o en proceso.
/// </summary>
public class CreatedScenes
{
    public List<PaintingSceneData> paintingScenes = new List<PaintingSceneData>();
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
