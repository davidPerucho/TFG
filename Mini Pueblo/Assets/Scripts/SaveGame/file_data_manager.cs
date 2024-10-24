using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataManager
{
    string directoryPath = "";
    string fileName = "";

    public FileDataManager(string directoryPath, string fileName)
    {
        this.directoryPath = directoryPath;
        this.fileName = fileName;
    }

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

    public bool file_exists()
    {
        string path = Path.Combine(directoryPath, fileName);

        return File.Exists(path);
    }

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