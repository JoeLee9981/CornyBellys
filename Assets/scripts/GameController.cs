using UnityEngine;
using System.Collections;
using Valve.VR;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour {

    public GameManager manager;
    public GameObject rightMotionController;
    public GameObject leftMotionController;
    public GameObject playerCharacter;
    public GameObject motionFlashlight;
    public GameObject staticFlashlight;

    void Awake() {
        manager = GameManager.getInstance();
        manager.VRControllerLeft = leftMotionController;
        manager.VRControllerRight = rightMotionController;
        if (OpenVR.IsHmdPresent()) {
            Debug.Log("HMD Detected, set to use VR");
            manager.UseVR = true;
        }
    }
	// Use this for initialization
	void Start () {
 
	}
	
	// Update is called once per frame
	void Update () {
        if(manager.UseVR) {
            if (!manager.VRActive && SteamVR.active) {
                Debug.Log("Activating VR");
                manager.VRActive = true;
            }
            /*if (!manager.MotionControlsConnected && leftMotionController.activeInHierarchy && rightMotionController.activeInHierarchy) {
                 Debug.Log("Motion Controllers detected - Connecting devices");
                 manager.MotionControlsConnected = true;
                 playerCharacter.GetComponent<FirstPersonController>().enabled = false;
                 playerCharacter.GetComponent<VRPlayerController>().enabled = true;
             }
             else if(manager.MotionControlsConnected && (!leftMotionController.activeInHierarchy || !rightMotionController.activeInHierarchy)) {
                 Debug.Log("Motion Controllers not detected - Disabling devices");
                 manager.MotionControlsConnected = false;
                 playerCharacter.GetComponent<FirstPersonController>().enabled = true;
                 playerCharacter.GetComponent<VRPlayerController>().enabled = false;
             }*/
        }

   }
}
