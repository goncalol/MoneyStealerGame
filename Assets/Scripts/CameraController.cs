using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smooth = 3f;
    public Transform Target;
    Transform standardPos;
    public Transform tiltPosLeft;
    public Transform tiltPosRight;
    public Transform death;
    private Transform CurrentTiltSide;
    private PlayerController player;
    public bool tiltMode { get; set; }

    private bool IsDead;

    private static CameraController _instance;

    public static CameraController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        var camPos = GameObject.Find("CamPos");
        //camPos.transform.position = gameObject.transform.position;
        //camPos.transform.rotation = gameObject.transform.rotation;
        standardPos = camPos.transform;
        player = PlayerController.Instance;
    }

    void FixedUpdate()
    {
        if (!IsDead)
        {
            if (tiltMode)
            {
                // lerp the camera position to the look at position, and lerp its forward direction to match 
                transform.position = Vector3.Lerp(transform.position, CurrentTiltSide.position, Time.deltaTime * smooth);
                transform.forward = Vector3.Lerp(transform.forward, CurrentTiltSide.forward, Time.deltaTime * smooth);
            }
            else if(player.IsWalking)
            {
                // return the camera to standard position and direction
                transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.deltaTime * smooth);
                transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.deltaTime * smooth);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, death.position, Time.deltaTime * smooth * 2);
            transform.forward = Vector3.Lerp(transform.forward, death.forward, Time.deltaTime * smooth * 2);
        }
    }

    public Transform GetStandardPos()
    {
        return standardPos;
    }

    internal void Tilt(bool isLeft)
    {
        tiltMode = true;
        CurrentTiltSide = isLeft ? tiltPosLeft : tiltPosRight;
    }

    internal void Death()
    {
        IsDead = true;
    }
}
