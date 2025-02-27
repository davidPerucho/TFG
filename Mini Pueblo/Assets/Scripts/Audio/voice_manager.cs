using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Diagnostics;

/// <summary>
/// Script encargado de la creación de voces mediante un TTS.
/// </summary>
public class VoiceManager : MonoBehaviour
{
    string nodePath = @"C:\Program Files\nodejs\node.exe";
    string scriptPath;
    string voicesDirectory;

    [ContextMenu("Execute Script")]
    /// <summary>
    /// Genera los audios con las frases de los personajes utilizando TTS implementado en el script TextToSpeach.js.
    /// </summary>
    void generateVoices()
    {
        scriptPath = Path.Combine(Application.dataPath, "Scripts/TTS/TextToSpeach.js");
        voicesDirectory = Application.persistentDataPath + "/Voices/";

        //Comprobar que el directorio existe
        if (!Directory.Exists(voicesDirectory))
        {
            Directory.CreateDirectory(voicesDirectory);
            UnityEngine.Debug.Log($"El directorio no existía, pero se ha creado: {voicesDirectory}");
        }

        //Obtengo las frases de todos los personajes
        CharacterTalk[] characters = FindObjectsOfType<CharacterTalk>();

        ProcessStartInfo processInfo;
        Process process;

        //Llamo a el script TextToSpeach para generar las voces de cada personaje
        foreach (CharacterTalk character in characters)
        {
            processInfo = new ProcessStartInfo
            {
                FileName = nodePath,
                Arguments = $"\"{scriptPath}\" \"{character.characterPhrase}\" \"{character.characterSex}\" \"{character.gameObject.name}\" \"{voicesDirectory}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process = new Process { StartInfo = processInfo };
            process.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data) && e.Data == "Error")
                {
                    UnityEngine.Debug.LogError(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            UnityEngine.Debug.Log("Frase convertida a audio: " + character.characterPhrase);
        }
    }
}