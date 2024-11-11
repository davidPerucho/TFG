using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Clase encargada de manejar los movimientos y animaciones del jugador.
/// </summary>
public class player_movement : MonoBehaviour, IDataPersistence
{
    public float rotationSpeed = 3f; //Velocidad a la que rota el jugador
    NavMeshAgent player; //Agente jugador encargado del movimiento

    [SerializeField] 
    LayerMask clickLayers; //Capas sobre las que se puede hacer click para moverse

    [HideInInspector]
    public bool stop = false; //True si el jugador esta parado

    bool prepareToStop = false; //True si el jugador se está preparando para parar
    UnityAction function; //Almacena las distintas funciones de conversación dee los NPCs
    Animator playerAnimator; //El animador del jugador
    Quaternion targetRotation; //Rotación deseada
    bool talking = false; //True si el jugador está en una conversación

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
            playerAnimator.SetBool("moving", false); //Se inicia animación estática
        }
        else
        {
            playerAnimator.SetBool("moving", true); //Se inicia animación de movimiento
            faceMouse();  // Continuar rotando hasta la posición de destino
        }

        if (stop == true)
        {
            playerAnimator.SetBool("talking", true); //Se inicia la animación de hablar
            faceMouse();  // Rotar hacia el NPC
        }
        else
        {
            playerAnimator.SetBool("talking", false); //Se inicia la aimación estática
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

        //Cuando se termina el zoom se inicia la conversación con el NPC
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
    /// Se encarga de que el jugador se mueva en diección al punto en el que se ha hecho click.
    /// </summary>
    void moveToMouse()
    {
        //Se calcula el punto en el que se ha hecho click
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickLayers))
        {
            player.destination = hit.point;
            setTargetRotation(hit.point);

            //En caso de que se haga click sobre un NPC el jugador se prepara para la parada y se almacena la conversación del NPC
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
    /// Asigna la dirección de rotación desada.
    /// </summary>
    /// <param name="targetPosition">La posición hacia la cual se quiere rotar.</param>
    void setTargetRotation(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }

    /// <summary>
    /// Rota al jugador en dirección al ratón
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
