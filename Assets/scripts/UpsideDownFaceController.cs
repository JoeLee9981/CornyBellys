using UnityEngine;
using System.Collections;

public class UpsideDownFaceController : BasicEnemyController {

    public override bool OnFlashLightEnter(NavMeshAgent agent, VRPlayerController player) {
        agent.destination = player.transform.position;
        playerDetected = true;
        agent.speed = 0;
        timeOutOfLos = 0;
        return true;
    }

    public override bool OnFlashLightLeave(NavMeshAgent agent, VRPlayerController player) {
        agent.speed = runSpeed;
        return false;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.name == "FPSController") {
            manager.TriggerGameOver();
        }
    }
}
