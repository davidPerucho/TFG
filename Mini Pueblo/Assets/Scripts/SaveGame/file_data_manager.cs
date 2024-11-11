using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Clase que se encarga de administrar los ficheros de guardado.
/// </summary>
public class FileDataManager
{
    string directoryPath = ""; //Dirección del directorio
    string fileName = ""; //Nombre del fichero

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    /// <param name="directoryPath">Dirección del directorio.</param>
    /// <param name="fileName">Nombre del fichero.</param>
    public FileDataManager(string directoryPath, string fileName)
    {
        this.directoryPath = directoryPath;
        this.fileName = fileName;
    }

    /// <summary>
    /// Carga los datos del fichero.
    /// </summary>
    /// <returns>Devuelve los datos almacenados en en un objeto GameData.</returns>
    public GameData load() 
    {
        string path = Path.Combine(directoryPath, fileName);
        GameData loadData = null;
        string dataToLoad = "";

        if (File.Exists(path))
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
            }
        }

        return loadData;
    }

    /// <summary>
    /// Alamcena los datos de juego en un fichero.
    /// </summary>
    /// <param name="data">Datos de juego.</param>
    public void save(GameData data) 
    { 
        string path = Path.Combine(directoryPath, fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(path, FileMode.Create)) 
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Comprueba si existe un fichero en el directorio directoryPath con nombre fileName.
    /// </summary>
    /// <returns>True si el fichero existe.</returns>
    public bool file_exists()
    {
        string path = Path.Combine(directoryPath, fileName);

        return File.Exists(path);
    }

    /// <summary>
    /// Elimina el fichero guardado.
    /// </summary>
    public void delete_save()
    {
        string path = Path.Combine(directoryPath, fileName);

        if (File.Exists(path))
        {
            try
            {

                File.Delete(path);
                Debug.Log("Archivo eliminado correctamente: " + path);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            Debug.LogError("No se encontró el archivo para eliminar: " + path);
        }
    }
}