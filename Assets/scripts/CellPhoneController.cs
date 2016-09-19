using UnityEngine;
using System.Collections;

public class CellPhoneController : MonoBehaviour {

    [SerializeField] private bool isMotionControlled = false;
    public GameObject flashLight;
    private GameManager manager;

	// Use this for initialization
	void Start () {

        if(isMotionControlled) { 
            manager = GameManager.getInstance();
            VR_CustomTrackedController lc = manager.VRControllerLeft.GetComponent<VR_CustomTrackedController>();
            lc.OnTriggerDown += OnTriggerDown;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnTriggerDown(object sender, InputEventArgs e) {
        flashLight.SetActive(!flashLight.activeSelf);
    }
}
