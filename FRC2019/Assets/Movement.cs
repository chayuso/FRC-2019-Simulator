using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float motorForce = 1;
    public float velocityLimit = 1;
    public Camera DriverStationCam;
    public Camera RearCam;
    public Camera FrontCam;
    bool isDriverCam = true;
    GameObject LeftMidWheel;
    GameObject RightMidWheel;

    GameObject LeftFrontWheel;
    GameObject RightFrontWheel;
    GameObject LeftBackWheel;
    GameObject RightBackWheel;

    private Rigidbody rb;

    void ToggleCam()
    {
        isDriverCam = !isDriverCam;
        DriverStationCam.enabled = isDriverCam;
        FrontCam.enabled = isDriverCam;
        FrontCam.usePhysicalProperties = true;
        RearCam.enabled = !isDriverCam;
    }
    void ToggleCam(bool toDrivercam)
    {
        isDriverCam = toDrivercam;
        DriverStationCam.enabled = isDriverCam;
        FrontCam.enabled = isDriverCam;
        FrontCam.usePhysicalProperties = true;
        RearCam.enabled = !isDriverCam;
    }
    void Start()
    {
        ToggleCam();
        rb = GetComponent<Rigidbody>();
        RightMidWheel = transform.Find("RightMid").gameObject;
        LeftMidWheel = transform.Find("LeftMid").gameObject;

        RightFrontWheel = transform.Find("RightFront").gameObject;
        LeftFrontWheel = transform.Find("LeftFront").gameObject;
        RightBackWheel = transform.Find("RightBack").gameObject;
        LeftBackWheel = transform.Find("LeftBack").gameObject;
    }
    void MoveLeftSide(float vspeed)
    {
        //transform.Rotate(0.0f, vspeed, 0.0f);
        //rb.AddForce(transform.forward * vspeed * motorPower);
        vspeed = vspeed / 3f;

        rb.AddForceAtPosition(transform.forward * vspeed * motorForce, LeftMidWheel.transform.position);

        rb.AddForceAtPosition(transform.forward * vspeed * motorForce, LeftFrontWheel.transform.position);
        rb.AddForceAtPosition(transform.forward * vspeed * motorForce, LeftBackWheel.transform.position);
        //rb.AddTorque(transform.forward * vspeed * motorPower);
        if (rb.velocity.magnitude > velocityLimit)
        {
            rb.AddForceAtPosition(-transform.forward * vspeed * motorForce, LeftMidWheel.transform.position);
            rb.AddForceAtPosition(-transform.forward * vspeed * motorForce, LeftFrontWheel.transform.position);
            rb.AddForceAtPosition(-transform.forward * vspeed * motorForce, LeftBackWheel.transform.position);


        }

    }
    void MoveRightSide(float vspeed)
    {
        //transform.Rotate(0.0f, -vspeed, 0.0f);
        vspeed = vspeed / 3f;
        rb.AddForceAtPosition(transform.forward * vspeed * motorForce, RightMidWheel.transform.position);

        rb.AddForceAtPosition(transform.forward * vspeed * motorForce, RightFrontWheel.transform.position);
        rb.AddForceAtPosition(transform.forward * vspeed * motorForce, RightBackWheel.transform.position);
        //rb.AddForce(transform.forward * vspeed * motorPower);
        //rb.AddTorque(transform.forward * vspeed * motorPower);
        if (rb.velocity.magnitude > velocityLimit)
        {
            rb.AddForceAtPosition(-transform.forward * vspeed * motorForce, RightMidWheel.transform.position);
            rb.AddForceAtPosition(-transform.forward * vspeed * motorForce, RightFrontWheel.transform.position);
            rb.AddForceAtPosition(-transform.forward * vspeed * motorForce, RightBackWheel.transform.position);
        }

    }
    void JoystickInputs()
    {
        if (Input.GetButtonDown("XBOX_LEFT_BUMPER") || Input.GetButtonDown("XBOX_RIGHT_BUMPER"))
        {
            ToggleCam();
        }
    }
    // Update is called once per frame
    void Update()
    {
        float moveLeftVertical = Input.GetAxis("WSVertical");
        float moveRightVertical = Input.GetAxis("ArrowsVertical");
        float LeftVerticalJoy = Input.GetAxis("Left_Vertical_Joystick");
        float RightVerticalJoy = Input.GetAxis("Right_Vertical_Joystick");

        MoveLeftSide(moveLeftVertical);
        MoveRightSide(moveRightVertical);

        MoveLeftSide(LeftVerticalJoy);
        MoveRightSide(RightVerticalJoy);
        JoystickInputs();
    }
}
