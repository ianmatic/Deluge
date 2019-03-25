using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public Camera cam;

    private float smoothSpeed = 0.125f;
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(-5, 7, -8);
        target = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;

        AudioManager audioManager = FindObjectOfType<AudioManager>();

        //put on top of camera and link to camera
        audioManager.gameObject.transform.position = cam.transform.position;
        audioManager.gameObject.transform.parent = cam.transform;
    }

    void FixedUpdate()
    {
        //in combat
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>().inCombat)
        {
            //find whichever GO is taking its turn
            foreach (GameObject entity in GetComponent<TurnManager>().combatEntities)
            {
                if (entity != null)
                {
                    //switch to that GO for camera
                    if (entity.GetComponent<Entity>().doingTurn)
                    {
                        target = entity;
                        break;
                    }
                }

            }
        }

        //update camera
        Vector3 targetPos = target.transform.position;

        Vector3 desiredPosition = targetPos + offset;
        Vector3 smoothedPosition = Vector3.Lerp(cam.transform.position, desiredPosition, smoothSpeed);

        cam.transform.position = smoothedPosition;
    }
}
