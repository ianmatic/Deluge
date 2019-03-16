using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject target;

    public Camera cam;

    private float smoothSpeed = 0.125f;
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(-5, 7, -8);
        target = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = target.transform.position;

        Vector3 desiredPosition = targetPos + offset;
        Vector3 smoothedPosition = Vector3.Lerp(cam.transform.position, desiredPosition, smoothSpeed);

        cam.transform.position = smoothedPosition;
    }
}
