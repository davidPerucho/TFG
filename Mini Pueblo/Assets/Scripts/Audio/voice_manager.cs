using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class voice_manager : MonoBehaviour
{
    [SerializeField]
    string apiURL;

    [SerializeField]
    string apiKey;

    string voicesDirectory;

    void Awake()
    {
        voicesDirectory = Path.Combine(Application.persistentDataPath, "Voices");
    }

    [ContextMenu("Execute Script")]
    void generateVoices()
    {
        //Obtengo las frases de todos los personajes
        CharacterTalk[] characters = FindObjectsOfType<CharacterTalk>();

        string frasePrueba = "";
        foreach (CharacterTalk character in characters)
        {
            Debug.Log(character.characterPhrase);
            frasePrueba = character.characterPhrase;
        }

        StartCoroutine(QueryHuggingFaceAPI(frasePrueba, 0));
    }

    /// <summary>
    /// Envía una solicitud a la API de Hugging Face con un texto que la api devuelve como audio y que se guarda posteriormente.
    /// </summary>
    /// <param name="inputText">El texto que se quiere convertir en audio.</param>
    IEnumerator QueryHuggingFaceAPI(string inputText, int index)
    {
        // Crear el payload
        string jsonPayload = JsonUtility.ToJson(new { inputs = inputText });

        // Crear la solicitud UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);

            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            // Enviar la solicitud y esperar la respuesta
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request successful. Saving audio...");

                // Obtener los datos de audio
                byte[] audioData = request.downloadHandler.data;

                // Guardar los datos en un archivo
                string savePath = Path.Combine(voicesDirectory, $"Voice_{index}.wav");
                SaveAudioToFile(audioData, savePath);

                Debug.Log($"Audio saved to: {savePath}");
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                Debug.Log($"Response: {request.downloadHandler.text}");
            }
        }
    }

    /// <summary>
    /// Guarda los datos de audio en un archivo WAV.
    /// </summary>
    /// <param name="audioData">Bytes de audio que se guardarán.</param>
    /// <param name="filePath">Ruta completa del archivo.</param>
    void SaveAudioToFile(byte[] audioData, string filePath)
    {
        try
        {
            File.WriteAllBytes(filePath, audioData);
            Debug.Log($"Audio file saved successfully at {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save audio file: {ex.Message}");
        }
    }
}
