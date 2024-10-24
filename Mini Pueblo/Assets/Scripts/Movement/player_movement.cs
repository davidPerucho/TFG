using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class player_movement : MonoBehaviour, IDataPersistence
{
    public float rotationSpeed = 3f;
    NavMeshAgent player;
    [SerializeField] LayerMask clickLayers;
    public bool stop = false;
    UnityAction function;
    string[] characters = { "PaperPlaneCharacter", "ShootingCharacter" }; //Contains the characters tags

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (stop == false)
            {
                face_mouse();
                move_to_mouse();
            }
        }

        if (stop == true && FindAnyObjectByType<camera_movement>().zoomInFinished == true && FindAnyObjectByType<camera_movement>().zoomOutNow == false)
        {
            function();
        }
    }

    void move_to_mouse()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickLayers))
        {
            player.destination = hit.point;

            if (characters.Contains(hit.collider.tag) == true)
            {
                stop = true;
                FindAnyObjectByType<camera_movement>().zoomOutNow = false;
                FindAnyObjectByType<camera_movement>().zoomInNow = true;
                FindAnyObjectByType<camera_movement>().zoomInFinished = false;
                GameObject hitObject = GameObject.FindGameObjectWithTag(hit.collider.tag);
                CharacterTalk character = hitObject.GetComponent<CharacterTalk>();
                function = () => { character.talk(); };
            }
        }
    }

    void face_mouse()
    {
        Vector3 direction = (player.destination - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x , 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation,Time.deltaTime * rotationSpeed);
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
