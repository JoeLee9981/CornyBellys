using UnityEngine;
using System.Collections;

public class VRFlashlightController : MonoBehaviour {

    public GameObject flashLight;
    public GameObject rightMotionController;

	// Use this for initialization
	void Start () {
        parentFlashlightToMotionControl();
        VR_CustomTrackedController rc = rightMotionController.GetComponent<VR_CustomTrackedController>();
        rc.OnTriggerDown += OnTriggerDown;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void parentFlashlightToMotionControl() {
        GameObject flashlight = GameObject.Find("Flashlight");
        flashlight.SetActive(false);
    }

    public void OnTriggerDown(object sender, InputEventArgs e) {
        flashLight.SetActive(!flashLight.activeSelf);
    }
}
