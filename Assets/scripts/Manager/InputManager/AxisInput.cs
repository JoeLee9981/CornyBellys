using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class AxisInput : GenericInput {

    private string horizontal;
    private string vertical;

    public AxisInput(string horizontal, string vertical) {
        this.horizontal = horizontal;
        this.vertical = vertical;
    }

    public Vector2 GetAxis(int index) {
        return new Vector2(CrossPlatformInputManager.GetAxis(horizontal), CrossPlatformInputManager.GetAxis(vertical));
    }

    public Vector2 GetAxisRaw(int index) {
        return new Vector2(CrossPlatformInputManager.GetAxisRaw(horizontal), CrossPlatformInputManager.GetAxisRaw(vertical));
    }

    public bool GetButton(int index) {
        return CrossPlatformInputManager.GetButton(horizontal) || CrossPlatformInputManager.GetButton(vertical);
    }

    public bool GetButtonDown(int index) {
        return CrossPlatformInputManager.GetButtonDown(horizontal) || CrossPlatformInputManager.GetButtonDown(vertical);
    }

    public bool GetButtonUp(int index) {
        return CrossPlatformInputManager.GetButtonUp(horizontal) || CrossPlatformInputManager.GetButtonUp(vertical);
    }

    public bool GetTouch(int index) {
        return CrossPlatformInputManager.GetButton(horizontal) || CrossPlatformInputManager.GetButton(vertical);
    }

    public bool GetTouchDown(int index) {
        return CrossPlatformInputManager.GetButtonDown(horizontal) || CrossPlatformInputManager.GetButtonDown(vertical);
    }

    public bool GetTouchUp(int index) {
        return CrossPlatformInputManager.GetButtonUp(horizontal) || CrossPlatformInputManager.GetButtonUp(vertical);
    }
}
