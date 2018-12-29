using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float LookSensitivity = 5f;
    public float xRotation;
    public float yRotation;
    public float currentXRotation;
    public float currentYRotation;
    public float xRotationVelocity;
    public float yRotationVelocity;
    public float Smoothness = 0.1f;

    // Use this for initialization
    void Start()
    {

    }

    void Update(){

        xRotation -= Input.GetAxis("Mouse Y") * LookSensitivity;
        yRotation += Input.GetAxis("Mouse X") * LookSensitivity;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

    }
}
