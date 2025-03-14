using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Estructura encargada de representar a un NPC.
/// </summary>
public struct CharacterData
{
    public Vector3 coordinates; //Coordenadas del NPC
    public string phrase; //Frase del NPC
    public int cIndex; //Index del NPC
    public int lIndex; //Index de la localización del NPC
    public string scene; //Escena del NPC
}

[System.Serializable]
/// <summary>
/// Clase encargada de representar una lista de NPCs.
/// </summary>
public class CharacterList
{
    public List<CharacterData> characters = new List<CharacterData>();
}
