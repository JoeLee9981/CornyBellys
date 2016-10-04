using UnityEngine;
using System.Collections;
using System;

public class ViveMotionInput : GenericInput {

    private ulong buttonMask;

    public ViveMotionInput(ulong buttonMask) {
        this.buttonMask = buttonMask;
    }

    public bool GetButton(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetPress(buttonMask);
    }

    public bool GetButtonUp(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetPressUp(buttonMask);
    }

    public bool GetButtonDown(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
    }

    public Vector2 GetAxis(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        if(device.GetTouch(buttonMask)) {
            return device.GetAxis();
        }
        return Vector2.zero;
        
    }

    public Vector2 GetAxisRaw(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetAxis();
    }

    public bool GetTouch(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetTouch(buttonMask);
    }

    public bool GetTouchDown(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetTouchDown(buttonMask);
    }

    public bool GetTouchUp(int index) {
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        return device.GetTouchUp(buttonMask);
    }
}
