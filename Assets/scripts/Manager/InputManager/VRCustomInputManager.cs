using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class VRCustomInputManager : XPlatformInputManager {

    /*
     * These maps are used to map the input values. Change these maps to change the input
     *  and then call RemapInputs()
     */
    public static Dictionary<string, ulong> LeftMotionControllerInput = new Dictionary<string, ulong>();
    public static Dictionary<string, ulong> RightMotionControllerInput = new Dictionary<string, ulong>();
    public static Dictionary<string, KeyCode> KeyboardInput = new Dictionary<string, KeyCode>();
    public static Dictionary<string, KeyCode> JoystickInput = new Dictionary<string, KeyCode>();
    public static Dictionary<string, string> UnityManagedInput = new Dictionary<string, string>();

    public static readonly string AXIS = "Axis";
    public static readonly string HORIZONTAL = "Horizontal";
    public static readonly string VERTICAL = "Vertical";
    public static readonly string FLASHLIGHT = "FlashLight";
    public static readonly string MOUSE_X = "Mouse X";
    public static readonly string MOUSE_Y = "Mouse Y";
    public static readonly string SPRINT = "Sprint";
    public static readonly string SNEAK = "Sneak";
    public static readonly string SUBMIT = "Submit";
    public static readonly string CANCEL = "Cancel";

    static VRCustomInputManager() {

        /*
         *  Set up the input for the left motion controller
         */
        LeftMotionControllerInput.Add(AXIS, SteamVR_Controller.ButtonMask.Touchpad);
        LeftMotionControllerInput.Add(FLASHLIGHT, SteamVR_Controller.ButtonMask.Trigger);

        /*
         * Set up the input for the right motion controller
         */
        RightMotionControllerInput.Add(AXIS, SteamVR_Controller.ButtonMask.Touchpad);
        RightMotionControllerInput.Add(SPRINT, SteamVR_Controller.ButtonMask.Trigger);

        /*
         * Set up the input for the keyboard
         */
        KeyboardInput.Add(FLASHLIGHT, KeyCode.F);
        KeyboardInput.Add(SNEAK, KeyCode.LeftControl);
        KeyboardInput.Add(SPRINT, KeyCode.LeftShift);
        KeyboardInput.Add(SUBMIT, KeyCode.Return);
        KeyboardInput.Add(CANCEL, KeyCode.Escape);

        /*
         *  Set up the input for the gamepad
         */
        JoystickInput.Add(FLASHLIGHT, KeyCode.Joystick1Button3);
        JoystickInput.Add(SUBMIT, KeyCode.Joystick1Button7);
        JoystickInput.Add(CANCEL, KeyCode.Joystick1Button6);
        JoystickInput.Add(SPRINT, KeyCode.Joystick1Button1);


        /*
         * Set up unity input manager settings 
         */
        UnityManagedInput.Add(VERTICAL, VERTICAL);
        UnityManagedInput.Add(HORIZONTAL, HORIZONTAL);
        UnityManagedInput.Add(MOUSE_X, MOUSE_X);
        UnityManagedInput.Add(MOUSE_Y, MOUSE_Y);
    }

    public VRCustomInputManager() : base() {
    }

    public override void MapInputs() {

        //Registering Input for the left motion controller
        foreach(string key in LeftMotionControllerInput.Keys) {
            if(AXIS.Equals(key)) {
                RegisterAxis(Axis.LEFT_AXIS, LeftMotionControllerInput[key]);
            }
            else {
                RegisterInput(key, LeftMotionControllerInput[key]);
            }
        }

        //Registering Input for the right motion controller
        foreach (string key in RightMotionControllerInput.Keys) {
            if (AXIS.Equals(key)) {
                RegisterAxis(Axis.RIGHT_AXIS, RightMotionControllerInput[key]);
            }
            else {
                RegisterInput(key, RightMotionControllerInput[key]);
            }
        }

        //Registering keyboard input
        foreach(string key in KeyboardInput.Keys) {
            RegisterInput(key, KeyboardInput[key]);
        }

        //Registering gamepad input
        foreach (string key in JoystickInput.Keys) {
            RegisterInput(key, JoystickInput[key]);
        }

        //Register unity managed left and right axis - We use the mapped value (not the key) because it may be changed
        RegisterAxis(Axis.LEFT_AXIS, UnityManagedInput[HORIZONTAL], UnityManagedInput[VERTICAL]);
        RegisterAxis(Axis.RIGHT_AXIS, UnityManagedInput[MOUSE_X], UnityManagedInput[MOUSE_Y]);
    }
}
