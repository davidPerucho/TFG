using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Clase encargada de representar un objeto con una probabilidad asociada.
/// </summary>
public struct ObjectWithProbability
{
    public GameObject gameObject; //Objeto
    public float probability; //Probabilidad asociada al objeto
}