using UnityEngine;
using System.Collections;
using System;

public class KeyCodeInput : GenericInput {

    private KeyCode positive;
    private KeyCode altPositive;

    public KeyCodeInput(KeyCode positive, KeyCode altPositive = KeyCode.None) {
        this.positive = positive;
        this.altPositive = altPositive; 
    }

    public Vector2 GetAxis(int index) {
        return Vector2.zero;
    }

    public Vector2 GetAxisRaw(int index) {
        return Vector2.zero;
    }

    public bool GetButton(int index) {
        return Input.GetKey(positive) || Input.GetKey(altPositive);
    }

    public bool GetButtonDown(int index) {
        return Input.GetKeyDown(positive) || Input.GetKeyDown(altPositive);
    }

    public bool GetButtonUp(int index) {
        return Input.GetKeyUp(positive) || Input.GetKeyUp(altPositive);
    }

    public bool GetTouch(int index) {
        return Input.GetKey(positive) || Input.GetKey(altPositive);
    }

    public bool GetTouchDown(int index) {
        return Input.GetKeyDown(positive) || Input.GetKeyDown(altPositive);
    }

    public bool GetTouchUp(int index) {
        return Input.GetKeyUp(positive) || Input.GetKeyUp(altPositive);
    }
}
