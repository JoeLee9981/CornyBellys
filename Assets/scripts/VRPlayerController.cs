using UnityEngine;
using System.Collections;

public class VRPlayerController : MonoBehaviour {

    public int Speed;
    public int RotateSpeed;
    public Vector2 Deadzone;
    public GameObject rightMotionController;
    public GameObject leftMotionController;

	// Use this for initialization
	void Start () {
        VR_CustomTrackedController rc = rightMotionController.GetComponent<VR_CustomTrackedController>();
        VR_CustomTrackedController lc = leftMotionController.GetComponent<VR_CustomTrackedController>();
        rc.OnTouchPadTouched += OnRightTouch;
        lc.OnTouchPadTouched += OnLeftTouch;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    

    public void OnRightTouch(object sender, InputEventArgs e) {
        if (e.touchpad.y > Deadzone.y || e.touchpad.y < -Deadzone.y) {
            transform.position += transform.forward * Time.deltaTime * e.touchpad.y * Speed;
        }
        if (e.touchpad.x > Deadzone.x || e.touchpad.x < -Deadzone.x) {
            transform.position += transform.right * Time.deltaTime * e.touchpad.x * Speed;
        }
    }

    public void OnLeftTouch(object sender, InputEventArgs e) {
        if(e.touchpad.x > Deadzone.x || e.touchpad.x < -Deadzone.x) {
            transform.Rotate(0, e.touchpad.x * RotateSpeed, 0);
        }
    }
}
