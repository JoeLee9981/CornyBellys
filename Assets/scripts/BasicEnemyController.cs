using UnityEngine;
using System.Collections;

public class BasicEnemyController : MonoBehaviour {

    [SerializeField]
    protected Transform[] waypoints;
    [SerializeField]
    protected float seeDistance = 30;
    [SerializeField]
    protected float seeAngle = 180;
    [SerializeField]
    protected float hearDistance = 15;
    [SerializeField]
    protected float walkSpeed = 3.5f;
    [SerializeField]
    protected float runSpeed = 7;
    
    //how long to pursue after LoS is lost
    [SerializeField]
    protected float pursueInterval = 10;
    protected GameManager manager;

    protected GameObject flashLight;
    protected bool playerDetected;
    protected float timeOutOfLos;
    protected int waypointIndex;
    protected float halfViewAngle;
    private bool flashLightDetected;

	// Use this for initialization
	void Start () {
        manager = GameManager.getInstance();
        waypointIndex = 0;
        halfViewAngle = seeAngle / 2;
	}

    // Update is called once per frame
    void Update() {

        if(!manager.IsGameAcitve()) {
            return;
        }

        bool playerLOS = false;

        VRPlayerController player = manager.PlayerCharacter;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        Vector3 playerDirection = player.transform.position - this.transform.position;

        if(CheckFlashLightCollision()) {
            flashLightDetected = OnFlashLightEnter(agent, player);
        }
        else if(flashLightDetected) {
            flashLightDetected = OnFlashLightLeave(agent, player);
        }

        if (!flashLightDetected && playerDistance < hearDistance && !player.IsSneaking()) {
            playerLOS = OnPlayerHeard(agent, player);
        }
        else if(!flashLightDetected && playerDistance < seeDistance) {
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

    public virtual bool CheckFlashLightCollision() {
        Light light = manager.CellPhone.GetComponentInChildren<Light>();

        if(light == null) {
            return false;
        }

        float lightAngle = light.spotAngle;
        float distance = Vector3.Distance(manager.CellPhone.transform.position, transform.position);

        if(distance < light.range) {
            Vector3 phoneToMonster = manager.CellPhone.transform.position - transform.position;
            float lightTheta = Vector3.Angle(phoneToMonster, manager.CellPhone.transform.forward);
            if(lightTheta > -lightAngle && lightTheta < lightAngle) {
                float monsterTheta = Vector3.Angle(phoneToMonster, transform.forward);
                if(monsterTheta > -halfViewAngle && monsterTheta < halfViewAngle) {
                    return true;
                }
            }
        }
        return false;
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

    public virtual bool OnFlashLightEnter(NavMeshAgent agent, VRPlayerController player) {
        agent.destination = player.transform.position;
        playerDetected = true;
        agent.speed = runSpeed;
        timeOutOfLos = 0;
        return true;
    } 

    public virtual bool OnFlashLightLeave(NavMeshAgent agent, VRPlayerController player) {
        return false;
    }

}
