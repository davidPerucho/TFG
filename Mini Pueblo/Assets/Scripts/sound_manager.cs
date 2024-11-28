using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; } //Instancia para el singleton
    AudioSource audioSourceMusic; //Fuente de audio para la música del juego

    [SerializeField]
    AudioSource audioSFX; //Fuente de audio para los efectos de sonido del juego

    public float increaseDecrase = 0.4f; //Cuanto se modifica el volumen al subir o bajar

    //Singleton
    void Awake()
    {
        if (instance == null) 
        {
            instance = this; 
        }
        else
        {
            Destroy(instance);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSourceMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void volumeMusicDown()
    {
        audioSourceMusic.volume -= increaseDecrase;
    }

    public void volumeMusicUp()
    {
        audioSourceMusic.volume += increaseDecrase;
    }

    public AudioSource addSFXLoop(Transform audioPosition, AudioClip audioClip, float volume)
    {
        AudioSource sfx = Instantiate(audioSFX, audioPosition);

        sfx.clip = audioClip;
        sfx.volume = volume;
        sfx.Play();

        return sfx;
    }
}
