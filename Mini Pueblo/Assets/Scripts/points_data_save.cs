using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Clase encargada de representar un objeto del tipo points_system.
/// </summary>
public class PointsDataSave
{
    public string pointsScene; //Nombre de la escena en la que se encuentra el script
    public string pointsObject; //Nombre del objeto asociado al script
    public int pointsCount; //Número de puntos
    public bool pointsEachSecond; //True si se suman puntos cada segundo
}
