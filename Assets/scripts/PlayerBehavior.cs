﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class PlayerBehavior : MonoBehaviour {

    private GameManager manager;

    public float Speed;

    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    public GameObject characterBody;

    // Use this for initialization
    void Start () {
        manager = GameManager.getInstance();
        if(OpenVR.IsHmdPresent() && (SteamVR.instance != null)) {
            manager.ViveConnected = true;
            Debug.Log("HMD detected, setting to VR");
            SteamVR.instance.hmd.ResetSeatedZeroPose();
        }

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody) targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        updateDirectionalMovement();
        if (!manager.ViveConnected) {
            updateMouseLook();
        }
        else {
            updateVRMouseLook();
            if(Input.GetKeyUp(KeyCode.F12)) {
                SteamVR.instance.hmd.ResetSeatedZeroPose();
            }
        }
	}

    private void updateDirectionalMovement() {

        float h = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            Vector3 transformVector = transform.right * Time.deltaTime * Speed;
            transformVector.y = 0;
            transform.position -= transformVector;
        }
        else if (Input.GetKey(KeyCode.D)) {
            Vector3 transformVector = transform.right * Time.deltaTime * Speed;
            transformVector.y = 0;
            transform.position += transformVector;
        }

        if (Input.GetKey(KeyCode.W)) {
            Vector3 transformVector = transform.forward * Time.deltaTime * Speed;
            transformVector.y = 0;
            transform.position += transformVector;
        }
        else if (Input.GetKey(KeyCode.S)) {
            Vector3 transformVector = transform.forward * Time.deltaTime * Speed;
            transformVector.y = 0;
            transform.position -= transformVector;
        }
    }

    private void updateVRMouseLook() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), 0);

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
        transform.localRotation = xRotation * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody) {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, characterBody.transform.up);
            characterBody.transform.localRotation = yRotation;
            characterBody.transform.localRotation *= targetCharacterOrientation;
        }
        else {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }

    }

    private void updateMouseLook() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
        transform.localRotation = xRotation * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody) {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, characterBody.transform.up);
            characterBody.transform.localRotation = yRotation;
            characterBody.transform.localRotation *= targetCharacterOrientation;
        }
        else {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}
