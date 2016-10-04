using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CellPhoneController : MonoBehaviour {

    [SerializeField] private bool isMotionControlled = false;
    public GameObject flashLight;
    private GameManager manager;

	// Use this for initialization
	void Start () {
        manager = GameManager.getInstance();
    }
	
	// Update is called once per frame
	void Update () {
        int leftIndex = (int)manager.VRControllerLeft.GetComponent<SteamVR_TrackedObject>().index;
	    if(manager.InputManager.GetInputDown(VRCustomInputManager.FLASHLIGHT, leftIndex)) {
            flashLight.SetActive(!flashLight.activeSelf);
        }
	}
}
