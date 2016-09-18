using UnityEngine;
using System.Collections;
using Valve.VR;

public struct InputEventArgs {
   public Vector2 touchpad;
}

public delegate void InputEventHandler(object sender, InputEventArgs e);

public class VR_CustomTrackedController : MonoBehaviour {

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

    public event InputEventHandler OnTriggerDown;
    public event InputEventHandler OnTrigger;
    public event InputEventHandler OnTriggerUp;
    public event InputEventHandler OnGripDown;
    public event InputEventHandler OnGrip;
    public event InputEventHandler OnGripUp;
    public event InputEventHandler OnTouchPadTouched;

    // Use this for initialization
    void Start() {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    private void onTouchPadTouched(InputEventArgs e) {
        if (OnTouchPadTouched != null) {
            OnTouchPadTouched(this, e);
        }
    }

    private void onTriggerDown(InputEventArgs e) {
        if(OnTriggerDown != null) {
            OnTriggerDown(this, e);
        }
    }

    private void onTriggerUp(InputEventArgs e) {
        if(OnTriggerUp != null) {
            OnTriggerUp(this, e);
        }
    }

    private void onTrigger(InputEventArgs e) {
        if (OnTrigger != null) {
            OnTrigger(this, e);
        }
    }

    private void onGripDown(InputEventArgs e) {
        if (OnGripDown != null) {
            OnGripDown(this, e);
        }
    }

    private void onGrip(InputEventArgs e) {
        if(OnGrip != null) {
            OnGrip(this, e);
        }
    }

    // Update is called once per frame
    void Update() {

        device = SteamVR_Controller.Input((int)trackedObject.index);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            InputEventArgs e;
            e.touchpad = device.GetAxis();
            onTriggerDown(e);
        }
        else if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            InputEventArgs e;
            e.touchpad = device.GetAxis();
            onTrigger(e);
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
            InputEventArgs e;
            e.touchpad = device.GetAxis();
            onTriggerUp(e);
        }

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
            InputEventArgs e;
            e.touchpad = device.GetAxis();
            onTouchPadTouched(e);
        }

        if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {

        }
        else if(device.GetPress(SteamVR_Controller.ButtonMask.Grip)) {

        }
        else if(device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {

        }
    }

    
}
