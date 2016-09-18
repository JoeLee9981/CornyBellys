using UnityEngine;
using System.Collections;

public class VRFlashlightController : MonoBehaviour {

    public GameObject flashLight;
    private GameManager manager;

	// Use this for initialization
	void Start () {
        manager = GameManager.getInstance();
        GameObject flashlightGameObject = GameObject.Find("Flashlight");
        flashlightGameObject.SetActive(false);
        VR_CustomTrackedController rc = manager.VRControllerRight.GetComponent<VR_CustomTrackedController>();
        rc.OnTriggerDown += OnTriggerDown;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerDown(object sender, InputEventArgs e) {
        flashLight.SetActive(!flashLight.activeSelf);
    }
}
