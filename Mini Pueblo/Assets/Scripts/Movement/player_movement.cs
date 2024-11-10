using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class player_movement : MonoBehaviour, IDataPersistence
{
    public float rotationSpeed = 3f;
    NavMeshAgent player;

    [SerializeField] 
    LayerMask clickLayers;

    [HideInInspector]
    public bool stop = false;
    bool prepareToStop = false;

    UnityAction function;
    Animator playerAnimator;
    Quaternion targetRotation;
    bool talking = false;

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
            playerAnimator.SetBool("moving", false);
        }
        else
        {
            playerAnimator.SetBool("moving", true);
            faceMouse();  // Continuar rotando hasta la posición de destino
        }

        if (stop == true)
        {
            playerAnimator.SetBool("talking", true);
            faceMouse();  // Rotar hacia el NPC
        }
        else
        {
            playerAnimator.SetBool("talking", false);
        }

        //En caso de que el jugador se dirija a un NPC se comprueba si se ha acercado lo suficiente como para anunciar la parada
        if (prepareToStop == true)
        {
            if (Vector3.Distance(player.destination, transform.position) < 0.7f)
            {
                prepareToStop = false;

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

    void moveToMouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickLayers))
        {
            player.destination = hit.point;
            setTargetRotation(hit.point);

            if (hit.collider.CompareTag("NPC"))
            {
                prepareToStop = true;

                GameObject hitObject = hit.collider.gameObject;
                CharacterTalk character = hitObject.GetComponent<CharacterTalk>();
                function = () => { character.talk(); };
            }
        }
    }

    void setTargetRotation(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }

    void faceMouse()
    {
        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void loadData(GameData data)
    {
        transform.position = data.playerPosition;
        transform.rotation = data.playerRotation;
    }

    public void saveData(ref GameData data)
    {
        data.playerPosition = transform.position;
        data.playerRotation = transform.rotation;
    }
}
