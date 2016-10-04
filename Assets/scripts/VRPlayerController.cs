using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
public class VRPlayerController : MonoBehaviour {

    [SerializeField]
    private int walkSpeed = 10;
    [SerializeField]
    private int sneakSpeed = 5;
    [SerializeField]
    private int runSpeed = 20;
    [SerializeField]
    private float stamina = 100;
    [SerializeField]
    private float staminaDrain = 20;
    [SerializeField]
    private float staminaRegen = 10;
    [SerializeField]
    private float fatigueTime = 5;
    [SerializeField]
    [Range(0f, 1f)]
    private float lengthenRunStep = 0.7f;
    [SerializeField]
    private Vector2 deadzone = new Vector2(0.2f, 0.2f);
    [SerializeField]
    private float gravityMultiplier = 2;
    [SerializeField]
    private Vector2 mouseSensitivity = new Vector2(20, 10);
    [SerializeField]
    private int smoothing = 10;
    [SerializeField]
    private Vector2 clampInDegrees = new Vector2(360, 180);
    [SerializeField]
    private AudioClip[] footStepSounds;
    [SerializeField]
    private AudioClip[] longBreaths;
    [SerializeField]
    private AudioClip[] shallowBreaths;
    [SerializeField]
    private int footStepInterval;
    [SerializeField]
    private float breathInterval = 2;
    [SerializeField]
    private float shallowBreathInterval = 0.5f;
    

    private Vector2 smoothMouse;
    private Vector2 mouseAbsolute;
    private Vector3 moveDir = Vector3.zero;
    private Vector2 mouseDelta = Vector2.zero;
    private Vector2 touchPadMouseDelta = Vector2.zero;

    private Vector2 touchPadInputVector;
    private Vector2 inputVector;
    private bool mouseLocked = true;
    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    //footsteps
    private float stepCycle;
    private float nextStep;
    private float breathCycle;
    private float nextBreath;

    private AudioSource breathAudioSource;
    private AudioSource footstepAudioSource;
    private CharacterController characterController;
    private GameManager manager;
    private PlayerCharacter playerCharacter;

    private VRCustomInputManager inputManager;

    // Use this for initialization
    void Start() {
        manager = GameManager.getInstance();
        playerCharacter = new PlayerCharacter(stamina);
        characterController = GetComponent<CharacterController>();

        inputManager = manager.InputManager;

        stepCycle = 0f;
        nextStep = stepCycle / 2f;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        footstepAudioSource = audioSources[0];
        breathAudioSource = audioSources[1];
        manager.PlayerCharacter = this;
    }

    // Update is called once per frame
    void Update() {
        int leftIndex = (int)manager.VRControllerLeft.GetComponent<SteamVR_TrackedObject>().index;
        int rightIndex = (int)manager.VRControllerRight.GetComponent<SteamVR_TrackedObject>().index;
        MouseLook(leftIndex, rightIndex);
        if (characterController.isGrounded) {
            moveDir.y = 0;
        }
    }

    void FixedUpdate() {

        int leftIndex = (int)manager.VRControllerLeft.GetComponent<SteamVR_TrackedObject>().index;
        int rightIndex = (int)manager.VRControllerRight.GetComponent<SteamVR_TrackedObject>().index;
        float speed = HandleInput(leftIndex, rightIndex);

        /*
         *  Handle Exhaustion
         */
        if (playerCharacter.Exhausted) {
            playerCharacter.TimeExhausted += Time.fixedDeltaTime;
            if (playerCharacter.TimeExhausted > fatigueTime) {
                playerCharacter.Exhausted = false;
            }
            else {
                speed = walkSpeed;
            }
        }


        /*
         *  Handle Input
         */
        if (inputVector.sqrMagnitude > 1) {
            inputVector.Normalize();
        }

        Vector3 projectedMove = transform.forward * inputVector.y + transform.right * inputVector.x;

        //RaycastHit hitInfo;
        //Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
        // characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        //projectedMove = Vector3.ProjectOnPlane(projectedMove, hitInfo.normal).normalized;

        moveDir.x = projectedMove.x * speed;
        moveDir.z = projectedMove.z * speed;

        if (!characterController.isGrounded) {
            moveDir += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }

        characterController.Move(moveDir * Time.fixedDeltaTime);

        ProgressFootStepCycle(speed);
        HandleBreathSound();

        /*
         *  Handle Stamina Drain
         */
        if (playerCharacter.IsSprinting()) {
            playerCharacter.Stamina -= staminaDrain * Time.fixedDeltaTime;
        }
        else {
            playerCharacter.Stamina += staminaRegen * Time.fixedDeltaTime;
            if (playerCharacter.Stamina > stamina) {
                playerCharacter.Stamina = stamina;
            }
        }

    }

    private float HandleInput(int leftIndex, int rightIndex) {
        //sprinting is input from gamepad / keyboard, altsprinting is input from motion controls
        playerCharacter.Sprinting = inputManager.GetInput(VRCustomInputManager.SPRINT, rightIndex);
        playerCharacter.Sneaking = inputManager.GetInput(VRCustomInputManager.SNEAK, rightIndex);
        float speed = playerCharacter.IsSprinting() ? runSpeed : walkSpeed;
        speed = playerCharacter.Sneaking ? sneakSpeed : speed;

        //if (touchPadInputVector.x != 0 || touchPadInputVector.y != 0) {
            //motion control input detected, takes priority
            //inputVector = touchPadInputVector;
        //}
       // else {
            //inputVector.x = CrossPlatformInputManager.GetAxis("Horizontal");
            //inputVector.y = CrossPlatformInputManager.GetAxis("Vertical");
            inputManager.GetLeftAxis(out inputVector, leftIndex);
       // }

        return speed;
    }

    public void MouseLook(int leftIndex, int rightIndex) {
        if (inputManager.GetInputDown(VRCustomInputManager.CANCEL)) {
            mouseLocked = !mouseLocked;
        }

        if (mouseLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Allow the script to clamp based on a desired target value.
            Quaternion targetOrientation = Quaternion.Euler(targetDirection);
            Quaternion targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            if (touchPadMouseDelta.x == 0) {
                // Get raw mouse input for a cleaner reading on more sensitive mice.
                inputManager.GetRightAxis(out mouseDelta, rightIndex);
            }
            else {
                mouseDelta = touchPadMouseDelta;
            }

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

            if (manager.VRActive) {
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

    private void ProgressFootStepCycle(float speed) {
        if (!IsSneaking() && (inputVector.x != 0 || inputVector.y != 0)) {
            stepCycle += (characterController.velocity.magnitude + (speed * (playerCharacter.IsSprinting() ? lengthenRunStep : 1f))) * Time.fixedDeltaTime;
        }
        if (stepCycle > nextStep) {
            nextStep = stepCycle + footStepInterval;
            HandleFootStepAudio();
        }
    }

    private void HandleBreathSound() {
        if (playerCharacter.IsSprinting() || playerCharacter.Exhausted) {
            breathCycle += Time.fixedDeltaTime;
        }
        if (playerCharacter.Exhausted && breathCycle > nextBreath) {
            nextBreath = breathCycle + breathInterval;
            int n = Random.Range(1, longBreaths.Length);
            breathAudioSource.clip = longBreaths[n];
            breathAudioSource.PlayOneShot(breathAudioSource.clip);
            longBreaths[n] = longBreaths[0];
            longBreaths[0] = breathAudioSource.clip;
        }
        else if(playerCharacter.IsSprinting() && breathCycle > nextBreath) {
            nextBreath = breathCycle + shallowBreathInterval;
            int n = Random.Range(1, shallowBreaths.Length);
            breathAudioSource.clip = shallowBreaths[n];
            breathAudioSource.PlayOneShot(breathAudioSource.clip);
            shallowBreaths[n] = shallowBreaths[0];
            shallowBreaths[0] = breathAudioSource.clip;
        }
    }

    private void HandleFootStepAudio() {
        int n = Random.Range(1, footStepSounds.Length);
        footstepAudioSource.clip = footStepSounds[n];
        footstepAudioSource.PlayOneShot(footstepAudioSource.clip);
        footStepSounds[n] = footStepSounds[0];
        footStepSounds[0] = footstepAudioSource.clip;
    }

    public bool IsSneaking() {
        return characterController.velocity.magnitude < 6f;
    }
    /*
    public void OnRightTouch(object sender, InputEventArgs e) {
        if (e.touchpad.y > deadzone.y || e.touchpad.y < -deadzone.y) {
            touchPadInputVector.y = e.touchpad.y;
        }
        if (e.touchpad.x > deadzone.x || e.touchpad.x < -deadzone.x) {
            touchPadInputVector.x = e.touchpad.x;
        }
    }

    public void OnRightTouchUp(object sender, InputEventArgs e) {
        touchPadInputVector.x = 0;
        touchPadInputVector.y = 0;
    }

    public void OnLeftTouch(object sender, InputEventArgs e) {
        if (e.touchpad.x > deadzone.x || e.touchpad.x < -deadzone.x) {
            touchPadMouseDelta.x = e.touchpad.x;
        }
    }

    public void OnLeftTouchUp(object sender, InputEventArgs e) {
        touchPadMouseDelta.x = 0;
    }

    public void OnLeftTriggerDown(object sender, InputEventArgs e) {
        playerCharacter.AltSprinting = true;
    }

    public void OnLeftTriggerUp(object sender, InputEventArgs e) {
        playerCharacter.AltSprinting = false;
    }*/
}
