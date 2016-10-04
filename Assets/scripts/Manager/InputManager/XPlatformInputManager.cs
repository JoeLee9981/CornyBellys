using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public abstract class XPlatformInputManager {

    private static readonly string LEFT_AXIS = "Axis_L";
    private static readonly string RIGHT_AXIS = "Axis_R";

    public enum Axis {
        LEFT_AXIS,
        RIGHT_AXIS
    }

    private Dictionary<string, List<GenericInput>> inputMap;

    /// <summary>
    /// Standard Constructor when no VR motion controls are being implemented - If you want to implement VR motion controls, use other constructor
    /// </summary>
    public XPlatformInputManager() {
        inputMap = new Dictionary<string, List<GenericInput>>();
        MapInputs();
    }

    /// <summary>
    /// Maps input using RegisterInput - All inputs should be mapped through this
    /// </summary>
    public abstract void MapInputs();

    /// <summary>
    /// Clears the mapped inputs and calls MapInputs() to remap them
    /// </summary>
    public void RemapInputs() {
        inputMap.Clear();
        MapInputs();
    }

    /// <summary>
    ///  Registers input for the HTC Vive Motion Controls
    /// </summary>
    /// <param name="inputGroup">The group to map to</param>
    /// <param name="vrButtonMask">The ButtonMask to map</param>
    /// <param name="controllerIndex">Index of the controller</param>
    public void RegisterInput(string inputGroup, ulong vrButtonMask) {
        RegisterInput(inputGroup, new ViveMotionInput(vrButtonMask));
    }

    /// <summary>
    /// Registers input for a unity InputManager group
    /// </summary>
    /// <param name="inputGroup">The group to map to</param>
    /// <param name="unityInputGroup">The unity group to map</param>
    public void RegisterInput(string inputGroup, string unityInputGroup) {
        RegisterInput(inputGroup, new UnityGroupInput(unityInputGroup));
    }

    /// <summary>
    /// Registers input for a KeyCode
    /// </summary>
    /// <param name="inputGroup">The group to map to</param>
    /// <param name="positive">The primary key to map to</param>
    /// <param name="altPositive">A secondary key to map (may be left out)</param>
    public void RegisterInput(string inputGroup, KeyCode positive, KeyCode altPositive = KeyCode.None) {
        RegisterInput(inputGroup, new KeyCodeInput(positive, altPositive));
    }

    /// <summary>
    /// Registers input for an axis from the InputManager in Unity - Requires both Horizontal and Vertical mappings
    /// </summary>
    /// <param name="axis">The axis (Left or Right) to map to</param>
    /// <param name="horizontalGroup">The horizontal InputManager group</param>
    /// <param name="verticalGroup">The Vertical InputManager group</param>
    public void RegisterAxis(Axis axis, string horizontalGroup, string verticalGroup) {
        RegisterAxis(axis, new AxisInput(horizontalGroup, verticalGroup));
    }

    /// <summary>
    /// Registers input for an axis on the Vive Motion controller
    /// </summary>
    /// <param name="axis">The axis (Left or Right) to map to</param>
    /// <param name="vrButtonMask">The buttonMask to map, defaults to touchpad</param>
    /// <param name="controllerIndex">The index of the controller to map</param>
    public void RegisterAxis(Axis axis, ulong vrButtonMask = SteamVR_Controller.ButtonMask.Touchpad) {
        RegisterAxis(axis, new ViveMotionInput(vrButtonMask));
    }

    private void RegisterAxis(Axis axis, GenericInput input) {
        string axisString = null;
        switch (axis) {
            case Axis.LEFT_AXIS:
                axisString = LEFT_AXIS;
                break;
            case Axis.RIGHT_AXIS:
                axisString = RIGHT_AXIS;
                break;
            default:
                Debug.LogError("<XPlatformInputManager.RegisterAxis> Invalid axis: " + axis.ToString());
                axisString = "";
                break;
        }
        RegisterInput(axisString, input);
    }

    /// <summary>
    /// Registers any input that implements the GenericInput interface - Generally use the others to create the GenericInput
    /// </summary>
    /// <param name="group">The input group to map to</param>
    /// <param name="input">The GenericInput to map</param>
    public void RegisterInput(string group, GenericInput input) {
        if (!inputMap.ContainsKey(group)) {
            inputMap.Add(group, new List<GenericInput>());
        }
        inputMap[group].Add(input);
    }

    /// <summary>
    /// Checks Button Down on an input group
    /// </summary>
    /// <param name="inputGroup"></param>
    /// <returns></returns>
    public bool GetInputDown(string inputGroup, int index = -1) {
        foreach(GenericInput input in inputMap[inputGroup]) {
            if(input.GetButtonDown(index)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks Button on an input group
    /// </summary>
    /// <param name="inputGroup"></param>
    /// <returns></returns>
    public bool GetInput(string inputGroup, int index = -1) {
        foreach (GenericInput input in inputMap[inputGroup]) {
            if (input.GetButton(index)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks ButtonUp on an input
    /// </summary>
    /// <param name="inputGroup"></param>
    /// <returns></returns>
    public bool GetInputUp(string inputGroup, int index = -1) {
        foreach (GenericInput input in inputMap[inputGroup]) {
            if (input.GetButtonUp(index)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Sets the Vector2 of the Right Axis or Stick
    /// </summary>
    /// <param name="vector">The Vector2 to set</param>
    /// <returns>true if an axis is pressed</returns>
    public bool GetRightAxis(out Vector2 vector, int index = -1) {
        foreach(GenericInput input in inputMap[RIGHT_AXIS]) {
            vector = input.GetAxis(index);
            if(vector.x != 0 || vector.y != 0) {
                return true;
            }
        }
        vector = Vector2.zero;
        return false;
    }

    /// <summary>
    /// Sets the Vector2 of the Left Axis or Stick
    /// </summary>
    /// <param name="vector">The Vector2 to set</param>
    /// <returns>true if an axis is pressed</returns>
    public bool GetLeftAxis(out Vector2 vector, int index = -1) {
        foreach (GenericInput input in inputMap[LEFT_AXIS]) {
            vector = input.GetAxis(index);
            if (vector.x != 0 || vector.y != 0) {
                return true;
            }
        }
        vector = Vector2.zero;
        return false;
    }

    /// <summary>
    /// Checks Touch for an input group - Should just check Button on inputs that don't use touch
    /// </summary>
    /// <param name="inputGroup"></param>
    /// <returns></returns>
    public bool GetTouch(string inputGroup, int index = -1) {
        foreach (GenericInput input in inputMap[inputGroup]) {
            if (input.GetTouch(index)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks TouchUp for an input group - Should just check ButtonUp on inputs that don't use touch
    /// </summary>
    /// <param name="inputGroup"></param>
    /// <returns></returns>
    public bool GetTouchUp(string inputGroup, int index = -1) {
        foreach (GenericInput input in inputMap[inputGroup]) {
            if (input.GetTouchUp(index)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks TouchDown for an input group - Should just check ButtonDown on inputs that don't use touch
    /// </summary>
    /// <param name="inputGroup"></param>
    /// <returns></returns>
    public bool GetTouchDown(string inputGroup, int index = -1) {
        foreach (GenericInput input in inputMap[inputGroup]) {
            if (input.GetTouchDown(index)) {
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Interface to define the handling for an input
/// </summary>
public interface GenericInput {
    bool GetButton(int index);
    bool GetButtonUp(int index);
    bool GetButtonDown(int index);
    Vector2 GetAxis(int index);
    Vector2 GetAxisRaw(int index);
    bool GetTouch(int index);
    bool GetTouchDown(int index);
    bool GetTouchUp(int index);
}
