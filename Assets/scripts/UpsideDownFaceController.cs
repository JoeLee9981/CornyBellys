using UnityEngine;
using System.Collections;

public class UpsideDownFaceController : MonoBehaviour {

    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private float sightRange = 30;
    [SerializeField]
    private float walkSpeed = 3.5f;
    [SerializeField]
    private float runSpeed = 7;
    //how long to pursue after LoS is lost
    [SerializeField]
    private float pursueInterval = 10;

    private bool playerDetected;
    private float timeOutOfLos;
    private GameManager manager;
    private int waypointIndex;

	// Use this for initialization
	void Start () {
        manager = GameManager.getInstance();
        waypointIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
        bool playerLOS = false;
        VRPlayerController player = manager.PlayerCharacter;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if(playerDistance < sightRange) {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, player.transform.position - transform.position, out hit)) {
                if(hit.transform == player.transform) {
                    //agent.destination = player.transform.position;
                    agent.destination = waypoints[waypointIndex].position;
                    playerLOS = true;
                    playerDetected = true;
                    agent.speed = runSpeed;
                    timeOutOfLos = 0;
                }
            }
        }
        if(!playerLOS) {
            if(playerDetected) {
                agent.destination = player.transform.position;
                timeOutOfLos += Time.deltaTime;
                if(timeOutOfLos > pursueInterval) {
                    timeOutOfLos = 0;
                    playerDetected = false;
                }
            }
            else {
                agent.destination = waypoints[waypointIndex].position;
                agent.speed = walkSpeed;
            }
        }
        
        if(Vector3.Distance(transform.position, waypoints[waypointIndex].position) < 0.5f) {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
        
	}
}
