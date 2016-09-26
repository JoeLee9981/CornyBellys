using UnityEngine;
using System.Collections;

public class UpsideDownFaceController : MonoBehaviour {

    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private float seeDistance = 30;
    [SerializeField]
    private float seeAngle = 180;
    [SerializeField]
    private float hearDistance = 15;
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
    private float halfViewAngle;

	// Use this for initialization
	void Start () {
        manager = GameManager.getInstance();
        waypointIndex = 0;
        halfViewAngle = seeAngle / 2;
	}

    // Update is called once per frame
    void Update() {
        bool playerLOS = false;
        VRPlayerController player = manager.PlayerCharacter;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        Vector3 playerDirection = player.transform.position - this.transform.position;

        if (playerDistance < hearDistance && !player.IsSneaking()) {
            playerLOS = OnPlayerHeard(agent, player);
        }
        else if(playerDistance < seeDistance) {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, playerDirection, out hit)) {
                if(hit.transform == player.transform) {
                    float angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
                    if(angleToPlayer > -halfViewAngle && angleToPlayer < halfViewAngle) { 
                        playerLOS = OnPlayerLineOfSight(agent, player);
                    }
                }
            }
        }
        if(!playerLOS) {
            if(playerDetected) {
                playerDetected = OnPlayerLineOfSightLost(agent, player);
            }
            else {
                agent.destination = waypoints[waypointIndex].position;
                agent.speed = walkSpeed;
            }
        }
        
        if(Vector3.Distance(transform.position, waypoints[waypointIndex].position) < 0.5f) {
            OnWaypointReached();
        }
        
	}

    public virtual bool OnPlayerLineOfSight(NavMeshAgent agent, VRPlayerController player) {
        agent.destination = player.transform.position;
        playerDetected = true;
        agent.speed = runSpeed;
        timeOutOfLos = 0;
        return true;
    }

    public virtual bool OnPlayerLineOfSightLost(NavMeshAgent agent, VRPlayerController player) {
        agent.destination = player.transform.position;
        timeOutOfLos += Time.deltaTime;
        if (timeOutOfLos > pursueInterval) {
            timeOutOfLos = 0;
            return false;
        }
        return true;
    }

    public virtual bool OnPlayerHeard(NavMeshAgent agent, VRPlayerController player) {
        agent.destination = player.transform.position;
        playerDetected = true;
        agent.speed = runSpeed;
        timeOutOfLos = 0;
        return true;
    }

    public virtual void OnWaypointReached() {
        waypointIndex = (waypointIndex + 1) % waypoints.Length;
    }
}
