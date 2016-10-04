using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class UnityGroupInput : GenericInput {

    private string unityGroup;

    public UnityGroupInput(string unityGroup) {
        this.unityGroup = unityGroup;
    }

    public Vector2 GetAxis(int index) {
        return Vector2.zero;
    }

    public Vector2 GetAxisRaw(int index) {
        return Vector2.zero;
    }

    public bool GetButton(int index) {
        return CrossPlatformInputManager.GetButton(unityGroup);
    }

    public bool GetButtonDown(int index) {
        return CrossPlatformInputManager.GetButtonDown(unityGroup);
    }

    public bool GetButtonUp(int index) {
        return CrossPlatformInputManager.GetButtonUp(unityGroup);
    }

    public bool GetTouch(int index) {
        return CrossPlatformInputManager.GetButton(unityGroup);
    }

    public bool GetTouchDown(int index) {
        return CrossPlatformInputManager.GetButtonDown(unityGroup);
    }

    public bool GetTouchUp(int index) {
        return CrossPlatformInputManager.GetButtonUp(unityGroup);
    }
}
