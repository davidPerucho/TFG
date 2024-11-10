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

    UnityAction function;
    Animator playerAnimator;
    Quaternion targetRotation;

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
        bool isAtDestination = Vector3.Distance(player.destination, transform.position) < 0.1f;

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

        //Moverse con el click izquierdo o al tocar la pantalla
        if (Input.GetMouseButtonDown(0) && stop == false)
        {
            moveToMouse();
        }

        //Cuando se termina el zoom se inicia la conversación con el NPC
        if (stop == true && FindAnyObjectByType<camera_movement>().zoomInFinished == true && FindAnyObjectByType<camera_movement>().zoomOutNow == false)
        {
            function();
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
                stop = true;
                var cameraMovement = FindAnyObjectByType<camera_movement>();
                cameraMovement.zoomOutNow = false;
                cameraMovement.zoomInNow = true;
                cameraMovement.zoomInFinished = false;

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
