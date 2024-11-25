using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource; //Fuente de audio que contiene los clips de audio y la configuración
    public float increaseDecrase = 0.4f; //Cuanto se modifica el volumen al subir o bajar

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void volumeDown()
    {
        audioSource.volume -= increaseDecrase;
    }

    public void volumeUp()
    {
        audioSource.volume += increaseDecrase;
    }
}
