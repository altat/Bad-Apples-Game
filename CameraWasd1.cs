using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWasd1 : MonoBehaviour {

    public GameObject camera;

    public float Force;
    public float xForce;
    public float yForce;

	void Start () {
		
	}
	
	void Update () {

       transform.rotation = Quaternion.Euler(0, camera.GetComponent<CameraMouse1>().currentYRotation, 0);  
        
        xForce = Input.GetAxis("Horizontal") * Force;
        yForce = Input.GetAxis("Vertical") * Force;

        transform.Translate(new Vector3(xForce, 0, yForce));
    }
}
