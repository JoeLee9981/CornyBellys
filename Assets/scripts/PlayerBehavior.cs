using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class PlayerBehavior : MonoBehaviour {

    private GameManager manager;

    public float Speed;
    public Vector3 Direction;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;

    Quaternion originalRotation;

    // Use this for initialization
    void Start () {
        manager = GameManager.getInstance();
        if(OpenVR.IsHmdPresent() && (SteamVR.instance != null)) {
            manager.ViveConnected = true;
            Debug.Log("HMD detected, setting to VR");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        updateDirectionalMovement();
        if (!manager.ViveConnected) {
            updateMouseLook();
        }
	}

    private void updateDirectionalMovement() {
        if (Input.GetKey(KeyCode.A)) {
            Direction.x = -1;
        }
        else if (Input.GetKey(KeyCode.D)) {
            Direction.x = 1;
        }
        else {
            Direction.x = 0;
        }

        if (Input.GetKey(KeyCode.W)) {
            Direction.z = 1;
        }
        else if (Input.GetKey(KeyCode.S)) {
            Direction.z = -1;
        }
        else {
            Direction.z = 0;
        }

        if (Direction.x != 0 || Direction.z != 0) {
            transform.position += Direction.normalized * Speed * Time.deltaTime;
        }
    }

    private void updateMouseLook() {
        if (axes == RotationAxes.MouseXAndY) {
            rotAverageY = 0f;
            rotAverageX = 0f;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            rotArrayY.Add(rotationY);
            rotArrayX.Add(rotationX);

            if (rotArrayY.Count >= frameCounter) {
                rotArrayY.RemoveAt(0);
            }
            if (rotArrayX.Count >= frameCounter) {
                rotArrayX.RemoveAt(0);
            }

            for (int j = 0; j < rotArrayY.Count; j++) {
                rotAverageY += rotArrayY[j];
            }
            for (int i = 0; i < rotArrayX.Count; i++) {
                rotAverageX += rotArrayX[i];
            }

            rotAverageY /= rotArrayY.Count;
            rotAverageX /= rotArrayX.Count;

            rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
            rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX) {
            rotAverageX = 0f;

            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            rotArrayX.Add(rotationX);

            if (rotArrayX.Count >= frameCounter) {
                rotArrayX.RemoveAt(0);
            }
            for (int i = 0; i < rotArrayX.Count; i++) {
                rotAverageX += rotArrayX[i];
            }
            rotAverageX /= rotArrayX.Count;

            rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else {
            rotAverageY = 0f;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotArrayY.Add(rotationY);

            if (rotArrayY.Count >= frameCounter) {
                rotArrayY.RemoveAt(0);
            }
            for (int j = 0; j < rotArrayY.Count; j++) {
                rotAverageY += rotArrayY[j];
            }
            rotAverageY /= rotArrayY.Count;

            rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }

    private float ClampAngle(float angle, float min, float max) {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F)) {
            if (angle < -360F) {
                angle += 360F;
            }
            if (angle > 360F) {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}
