using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que se encarga de manejar la musica y los sonidos del juego.
/// </summary>
public class SoundManager : MonoBehaviour, IDataPersistence
{
    public static SoundManager instance { get; private set; } //Instancia para el singleton
    AudioSource audioSourceMusic; //Fuente de audio para la música del juego
    float musicVolume = 0.5f; //Volumen de la música
    float sfxVolume = 1f; //Volumen de los efectos de sonido

    [SerializeField]
    GameObject audioSFX; //Fuente de audio para los efectos de sonido del juego

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

    /// <summary>
    /// Baja el volumen de la musica del juego.
    /// </summary>
    public void volumeMusicDown()
    {
        if (increaseDecrase > musicVolume)
        {
            increaseDecrase = musicVolume;
        }
        audioSourceMusic.volume -= increaseDecrase;
    }

    /// <summary>
    /// Sube el volumen de la musica del juego.
    /// </summary>
    public void volumeMusicUp()
    {
        audioSourceMusic.volume += increaseDecrase;
    }

    /// <summary>
    /// Crea una fuente de audio para reproducir los efectos de en bucle.
    /// </summary>
    /// <param name="audioPosition">Posicion en la que se va a crear la fuente de audio.</param>
    /// <param name="audioClip">Clip de audio que de va a reproducir.</param>
    /// <param name="volume">Volumen que tendrá la fuente de audio.</param>
    /// <returns>Instancia de la furnte de audio.</returns>
    public GameObject addSFXLoop(Transform audioPosition, AudioClip audioClip, float volume)
    {
        GameObject sfx = Instantiate(audioSFX, audioPosition);

        sfx.GetComponent<AudioSource>().loop = true;
        sfx.GetComponent<AudioSource>().clip = audioClip;
        sfx.GetComponent<AudioSource>().volume = volume * sfxVolume;
        sfx.GetComponent<AudioSource>().Play();

        return sfx;
    }

    /// <summary>
    /// Crea una fuente de audio para reproducir los efectos de sonido, una vez terminado el audio la fuente se destrulle.
    /// </summary>
    /// <param name="audioPosition">Posicion en la que se va a crear la fuente de audio.</param>
    /// <param name="audioClip">Clip de audio que de va a reproducir.</param>
    /// <param name="volume">Volumen que tendrá la fuente de audio.</param>
    public void addSFX(Transform audioPosition, AudioClip audioClip, float volume)
    {
        GameObject sfx = Instantiate(audioSFX, audioPosition);

        sfx.GetComponent<AudioSource>().loop = false;
        sfx.GetComponent<AudioSource>().clip = audioClip;
        sfx.GetComponent<AudioSource>().volume = volume * sfxVolume;
        sfx.GetComponent<AudioSource>().Play();

        //Se destruye el objeto tras reproducir el audio
        Destroy(sfx, audioClip.length);
    }

    /// <summary>
    /// Carga el volumen de la música y de los SFX que se ha asignado en las opciones del menu.
    /// </summary>
    /// <param name="data">Datos del juego.</param>
    public void loadData(GameData data)
    {
        musicVolume = data.musicVolume / 2;
        sfxVolume = data.sfxVolume;

        audioSourceMusic.volume = musicVolume;
    }

    public void saveData(ref GameData data){}
}
