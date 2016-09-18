using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (CharacterController))]
public class VRPlayerController : MonoBehaviour {

    [SerializeField] private int walkSpeed = 10;
    [SerializeField] private int runSpeed = 20;
    [SerializeField] private int rotateSpeed = 3;
    [SerializeField] private Vector2 deadzone = new Vector2(0.2f, 0.2f);
    [SerializeField] private float gravityMultiplier = 2;
    [SerializeField] private float stickToGroundForce = 10;
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(20, 10);
    [SerializeField] private int smoothing = 10;
    [SerializeField] private Vector2 clampInDegrees = new Vector2(360, 180);

    private Vector2 smoothMouse;
    private Vector2 mouseAbsolute;
    private Vector3 moveDir = Vector3.zero;
    private CharacterController characterController;
    private Vector2 inputVector;
    private bool sprinting;
    private bool altSprinting;
    private bool mouseLocked = true;
    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    private GameManager manager;

    // Use this for initialization
    void Start () {

        characterController = GetComponent<CharacterController>();

        manager = GameManager.getInstance();
        VR_CustomTrackedController rc = manager.VRControllerRight.GetComponent<VR_CustomTrackedController>();
        VR_CustomTrackedController lc = manager.VRControllerLeft.GetComponent<VR_CustomTrackedController>();
        rc.OnTouchPadTouched += OnRightTouch;
        lc.OnTouchPadTouched += OnLeftTouch;
        lc.OnTriggerDown += OnLeftTriggerDown;
        lc.OnTriggerUp += OnLeftTriggerUp;
    }
	
	// Update is called once per frame
	void Update () {
        MouseLook();
        if (characterController.isGrounded) {
            moveDir.y = 0;
        }
       
	}

    void FixedUpdate() {
        float speed = HandleInput();

        if (inputVector.sqrMagnitude > 1) {
            inputVector.Normalize();
        }

        Vector3 projectedMove = transform.forward * inputVector.y + transform.right * inputVector.x;

        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                            characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        projectedMove = Vector3.ProjectOnPlane(projectedMove, hitInfo.normal).normalized;

        moveDir.x = projectedMove.x * speed;
        moveDir.z = projectedMove.z * speed;

        if (!characterController.isGrounded) {
            moveDir += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }

        characterController.Move(moveDir * Time.fixedDeltaTime);
        //zero these vectors to handle input events from motion controls
        inputVector.x = 0;
        inputVector.y = 0;
    }

    private float HandleInput() {
        //sprinting is input from gamepad / keyboard, altsprinting is input from motion controls
        sprinting = CrossPlatformInputManager.GetButton("Sprint");
        float speed = sprinting || altSprinting ? runSpeed : walkSpeed;

        if (inputVector.x != 0 || inputVector.y != 0) {
            //motion control input detected, takes priority
           
        } 
        else { 
            inputVector.x = CrossPlatformInputManager.GetAxis("Horizontal");
            inputVector.y = CrossPlatformInputManager.GetAxis("Vertical");
        }

        return speed;
    }

    public void MouseLook() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            mouseLocked = !mouseLocked;
        }

        if(mouseLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Allow the script to clamp based on a desired target value.
            Quaternion targetOrientation = Quaternion.Euler(targetDirection);
            Quaternion targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(mouseSensitivity.x * smoothing, mouseSensitivity.y * smoothing));

            // Interpolate mouse movement over time to apply smoothing delta.
            smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing);
            smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing);

            // Find the absolute mouse movement value from point zero.
            mouseAbsolute += smoothMouse * Time.deltaTime;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            if(manager.VRActive) {
                mouseAbsolute.y = 0;
            }
            Quaternion xRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right);
            transform.localRotation = xRotation * targetOrientation;
            
            Quaternion yRotation = Quaternion.AngleAxis(mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnRightTouch(object sender, InputEventArgs e) {
        if (e.touchpad.y > deadzone.y || e.touchpad.y < -deadzone.y) {
            inputVector.y = e.touchpad.y;
        }
        if (e.touchpad.x > deadzone.x || e.touchpad.x < -deadzone.x) {
            inputVector.x = e.touchpad.x;
        }
    }

    public void OnLeftTouch(object sender, InputEventArgs e) {
        if(e.touchpad.x > deadzone.x || e.touchpad.x < -deadzone.x) {
            transform.Rotate(0, e.touchpad.x * rotateSpeed, 0);
        }
    }

    public void OnLeftTriggerDown(object sender, InputEventArgs e) {
        altSprinting = true;
    }

    public void OnLeftTriggerUp(object sender, InputEventArgs e) {
        altSprinting = false;
    }
}
