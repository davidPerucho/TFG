using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Clase encargada de manejar los movimientos y animaciones del jugador.
/// </summary>
public class player_movement : MonoBehaviour, IDataPersistence
{
    public float rotationSpeed = 3f; //Velocidad a la que rota el jugador

    [HideInInspector]
    public NavMeshAgent player; //Agente jugador encargado del movimiento

    [SerializeField] 
    LayerMask clickLayers; //Capas sobre las que se puede hacer click para moverse

    [HideInInspector]
    public bool stop = false; //True si el jugador esta parado
    [HideInInspector]
    public RaycastHit hit; //Almacena la informaci�n del punto en el que se ha hecho click

    bool prepareToStop = false; //True si el jugador se est� preparando para parar
    UnityAction function; //Almacena las distintas funciones de conversaci�n dee los NPCs
    Animator playerAnimator; //El animador del jugador
    Quaternion targetRotation; //Rotaci�n deseada
    bool talking = false; //True si el jugador est� en una conversaci�n

    AudioSource playerAudio; //Reproductor de los sonidos del jugador

    [SerializeField]
    AudioClip runningAudio; //Running audio of the player

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<NavMeshAgent>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Control de animaciones del jugador
        bool isAtDestination = isAtDestination = Vector3.Distance(player.destination, transform.position) < 0.1f;

        if (isAtDestination == true || stop == true)
        {
            playerAnimator.SetBool("moving", false); //Se inicia animaci�n est�tica
            if (playerAudio != null)
            {
                Destroy(playerAudio); //Se para el sonido de correr del jugador
            }
        }
        else
        {
            playerAnimator.SetBool("moving", true); //Se inicia animaci�n de movimiento
            if(playerAudio == null)
            {
                playerAudio = SoundManager.instance.addSFXLoop(transform, runningAudio, 0.8f); //Se inicia el sonido de correr del jugador
            }
            faceMouse();  // Continuar rotando hasta la posici�n de destino
        }

        if (stop == true)
        {
            playerAnimator.SetBool("talking", true); //Se inicia la animaci�n de hablar
            faceMouse();  // Rotar hacia el NPC
        }
        else
        {
            playerAnimator.SetBool("talking", false); //Se inicia la aimaci�n est�tica
        }

        //En caso de que el jugador este preparado para parar y este suficientemente cerca del destino se realiza la parada
        if (prepareToStop == true)
        {
            if (Vector3.Distance(player.destination, transform.position) < 0.7f)
            {
                prepareToStop = false;

                //Una vez se para el jugador comienza el proceso de zoomIn
                stop = true;
                var cameraMovement = FindAnyObjectByType<camera_movement>();
                cameraMovement.zoomOutNow = false;
                cameraMovement.zoomInNow = true;
                cameraMovement.zoomInFinished = false;
            }
        }

        //Moverse con el click izquierdo o al tocar la pantalla
        if (Input.GetMouseButtonDown(0) && stop == false)
        {
            moveToMouse();
        }

        //Cuando se termina el zoom se inicia la conversaci�n con el NPC
        if (stop == true && FindAnyObjectByType<camera_movement>().zoomInFinished == true && FindAnyObjectByType<camera_movement>().zoomOutNow == false && talking == false)
        {
            function();
            talking = true;
        }
        else
        {
            if (stop == false) 
            {
                talking = false;
            }
        }
    }

    /// <summary>
    /// Se encarga de que el jugador se mueva en diecci�n al punto en el que se ha hecho click.
    /// </summary>
    void moveToMouse()
    {
        //Se calcula el punto en el que se ha hecho click
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickLayers))
        {
            player.destination = hit.point;
            setTargetRotation(hit.point);

            //En caso de que se haga click sobre un NPC el jugador se prepara para la parada y se almacena la conversaci�n del NPC
            if (hit.collider.CompareTag("NPC"))
            {
                prepareToStop = true;

                GameObject hitObject = hit.collider.gameObject;
                CharacterTalk character = hitObject.GetComponent<CharacterTalk>();
                function = () => { character.talk(); };
            }
        }
    }

    /// <summary>
    /// Asigna la direcci�n de rotaci�n desada.
    /// </summary>
    /// <param name="targetPosition">La posici�n hacia la cual se quiere rotar.</param>
    void setTargetRotation(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }

    /// <summary>
    /// Rota al jugador en direcci�n al rat�n
    /// </summary>
    void faceMouse()
    {
        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary>
    /// Carga los datos guardados del jugador.
    /// </summary>
    /// <param name="data">Datos del juego.</param>
    public void loadData(GameData data)
    {
        transform.position = data.playerPosition;
        transform.rotation = data.playerRotation;
    }

    /// <summary>
    /// Guarda los datos del jugador.
    /// </summary>
    /// <param name="data">Referencia a los datos del juego.</param>
    public void saveData(ref GameData data)
    {
        data.playerPosition = transform.position;
        data.playerRotation = transform.rotation;
    }
}
