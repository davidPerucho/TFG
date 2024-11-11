using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz que implementan las clases que necesitan guardar datos de forma persistente.
/// </summary>
public interface IDataPersistence
{
    /// <summary>
    /// Carga los datos almacenados de juego.
    /// </summary>
    /// <param name="data">Variable donde se almacenarán los datos guardados.</param>
    void loadData(GameData data);

    /// <summary>
    /// Guarda en data los datos de juego.
    /// </summary>
    /// <param name="data">Referencia a los datos de juego.</param>
    void saveData(ref GameData data);
}
