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
        string filePath = Path.Combine(Application.persistentDataPath, "Managers/" + "NewCharacter.json");
        if (File.Exists(filePath))
        {
            //Leo la información del personaje y elimimino el fichero
            string json = File.ReadAllText(filePath);
            CharacterList characterList = JsonUtility.FromJson<CharacterList>(json);

            foreach (CharacterData character in characterList.characters)
            {
                //Instanciar los personajes
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
