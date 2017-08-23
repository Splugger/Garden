using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    Rigidbody rb;
    Camera cam;
    float rotX;
    float sensitivityX = 10f;
    float rotY;
    float yClamp = 60f;
    float sensitivityY = 10f;

    Quaternion originalRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        originalRot = transform.rotation;
    }

    void Update()
    {
        rotX += Input.GetAxis("Mouse X") * sensitivityX;
        rotY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotY = Mathf.Clamp(rotY, -yClamp, yClamp);

        cam.transform.localRotation = Quaternion.AngleAxis(rotY, -Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(rotX, Vector3.up);
    }
}
