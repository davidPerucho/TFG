using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CharacterCreationManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] characterPrefabs; //Objetos prefabricados de los NPC

    void Awake()
    {
        //En de que exista el fichero NewCharacter.json creo un personaje con la información del fichero
        string filePath = Path.Combine(Application.persistentDataPath, "Managers/" + "NewCharacters.json");
        if (File.Exists(filePath))
        {
            //Leo la información de los personajes y los creo en la escena
            string json = File.ReadAllText(filePath);
            CharacterList characterList = JsonUtility.FromJson<CharacterList>(json);

            foreach (CharacterData characterData in characterList.characters)
            {
                Debug.Log(characterData.yRotation);
                GameObject character = Instantiate(characterPrefabs[characterData.cIndex], characterData.coordinates, Quaternion.Euler(0, characterData.yRotation, 0));
                CharacterTalk characterTalk = character.GetComponent<CharacterTalk>();
                characterTalk.characterPhrase = characterData.phrase;
                characterTalk.sceneName = characterData.scene;
                characterTalk.characterSex = characterData.sex;
            }
        }
    }
}
