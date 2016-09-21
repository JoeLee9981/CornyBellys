using UnityEngine;
using System.Collections;
using Valve.VR;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour {

    public GameManager manager;
    [SerializeField] private GameObject rightMotionController;
    [SerializeField] private GameObject leftMotionController;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject vrCellPhone;
    [SerializeField] private GameObject cellPhone;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private AudioClip bgAudio;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgAudio;
        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if(manager.UseVR) {
            if (!manager.VRActive && SteamVR.active) {
                Debug.Log("Activating VR");
                manager.VRActive = true;
            }
            if (!manager.MotionControlsConnected && leftMotionController.activeInHierarchy && rightMotionController.activeInHierarchy) {
                Debug.Log("Motion Controllers detected - Connecting devices");
                manager.MotionControlsConnected = true;
                cellPhone.SetActive(false);
                vrCellPhone.SetActive(true);
             }
             else if(manager.MotionControlsConnected && (!leftMotionController.activeInHierarchy || !rightMotionController.activeInHierarchy)) {
                Debug.Log("Motion Controllers not detected - Disabling devices");
                manager.MotionControlsConnected = false;
                cellPhone.SetActive(true);
                vrCellPhone.SetActive(false);
             }
        }
   }

    void FixedUpdate() {
    }
}
