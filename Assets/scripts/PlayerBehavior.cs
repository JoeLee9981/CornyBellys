using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour {

    public float Speed;
    public Vector3 Direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKey(KeyCode.A)) {
            Direction.x = -1;
        }
        else if(Input.GetKey(KeyCode.D)) {
            Direction.x = 1;
        }
        else {
            Direction.x = 0;
        }

        if(Input.GetKey(KeyCode.W)) {
            Direction.z = 1;
        }
        else if(Input.GetKey(KeyCode.S)) {
            Direction.z = -1;
        }
        else {
            Direction.z = 0;
        }

        if(Direction.x != 0 || Direction.z != 0) {
            transform.position += Direction.normalized * Speed * Time.deltaTime;
        }
	}
}
