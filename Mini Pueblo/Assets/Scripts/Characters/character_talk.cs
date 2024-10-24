using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterTalk : MonoBehaviour
{
    public string characterPhrase;
    public string sceneName;

    //Asings the character phrase to the text on the dialog manager
    public void talk()
    {
        FindAnyObjectByType<dialog_manager>().showText(characterPhrase, loadScene);
    }

    //Loads the scene that has the name sceneName
    public void loadScene()
    {
        SceneManager.LoadScene(sceneName);
        DataPersitence.instance.saveGame();
    }
}
