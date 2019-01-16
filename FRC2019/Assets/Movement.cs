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
    bool isSandStorm = true;
    GameObject SandStorm;
    GameObject LeftMidWheel;
    GameObject RightMidWheel;

    GameObject LeftFrontWheel;
    GameObject RightFrontWheel;
    GameObject LeftBackWheel;
    GameObject RightBackWheel;
    LimeLightData LL;

    private Rigidbody rb;

    float minForce = 0.05f;
    float minForwardForce = 0.375f;
    float searchForce = 0.75f;
    float distanceRange = 1f;

    void ToggleSandStorm()
    {
        isSandStorm = !isSandStorm;
        SandStorm.GetComponent<MeshRenderer>().enabled = isSandStorm;
    }
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
        LL = GetComponent<LimeLightData>();
        rb = GetComponent<Rigidbody>();
        SandStorm = GameObject.Find("SandStorm");
        RightMidWheel = transform.Find("RightMid").gameObject;
        LeftMidWheel = transform.Find("LeftMid").gameObject;

        RightFrontWheel = transform.Find("RightFront").gameObject;
        LeftFrontWheel = transform.Find("LeftFront").gameObject;
        RightBackWheel = transform.Find("RightBack").gameObject;
        LeftBackWheel = transform.Find("LeftBack").gameObject;
        ToggleCam();
        ToggleSandStorm();
    }
    void setMotorSpeedLeft(float vspeed)
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
    void setMotorSpeedRight(float vspeed)
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
        if (Input.GetButtonDown("XBOX_LEFT_BUMPER"))
        {
            ToggleSandStorm();
        }
        if (Input.GetButtonDown("XBOX_RIGHT_BUMPER"))
        {
            ToggleCam();
        }
        if (Input.GetButton("XBOX_X"))
        {
            seekPositioning();
            //seekTargetRange();
            //seekTarget();
        }
        else
        {
            float LeftVerticalJoy = Input.GetAxis("Left_Vertical_Joystick");
            float RightVerticalJoy = Input.GetAxis("Right_Vertical_Joystick");

            if (Input.GetButton("XBOX_B"))
            {
                setMotorSpeedLeft(LeftVerticalJoy);
                setMotorSpeedRight(LeftVerticalJoy);
            }
            else
            {
                setMotorSpeedLeft(LeftVerticalJoy);
                setMotorSpeedRight(RightVerticalJoy);
            }
        }

    }
    float EstimateDistance()
    {
        float h1 = transform.position.y;
        float h2 = FrontCam.GetComponent<Example>().CurrentTarget.transform.position.y;
        float a1 = Mathf.Abs(360 - transform.localEulerAngles.x);
        float a2 = (float)LL.tsSkewRotation;
        float d = (h2 - h1) / (float)(Mathf.Tan(Mathf.Deg2Rad * (a1 + a2)));
        return d;
    }

    void seekPositioning()
    {
        float KpDistance = -0.3f;
        float steering_adjust = 0.0f;
        float distance_adjust = 0.0f;
        if (LL.tvValidTarget == 0.0)
        {
            // We don't see the target, seek for the target by spinning in place at a safe speed.
            if (Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL") > 0)
                steering_adjust = -searchForce - minForce;
            else if (Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL") < 0)
                steering_adjust = searchForce + minForce;
            print("Seeking");
        }
        else
        {
            // We do see the target, execute aiming code
            //double heading_error = LL.txOffsetX;
            //steering_adjust = 0.3 * LL.txOffsetX;
            float heading_error = (float)LL.txOffsetX;
            float distance_error = distanceRange - EstimateDistance();
            minForwardForce = Mathf.Abs(minForwardForce);
            if (distance_error > 0)
            {
                minForwardForce = -Mathf.Abs(minForwardForce);
            }
            steering_adjust = 0.0f;
            if (LL.txOffsetX > 1.0)
            {
                steering_adjust = 0.1f * (float)heading_error - (float)minForce;
            }
            else if (LL.txOffsetX < 1.0)
            {
                steering_adjust = 0.1f * (float)heading_error + (float)minForce;
            }
            distance_adjust = KpDistance * distance_error + minForwardForce;
            print("Adjusting");
        }

        setMotorSpeedLeft(-(float)steering_adjust+ distance_adjust);
        setMotorSpeedRight((float)steering_adjust+ distance_adjust);
    }
    void seekTargetRange()
    {
        if (LL.tvValidTarget == 0.0)
            return;
        float KpDistance = -0.3f;  // Proportional control constant for distance
        float distance_error = distanceRange - EstimateDistance();
        minForwardForce = Mathf.Abs(minForwardForce);
        if (distance_error > 0)
        {
            minForwardForce = -Mathf.Abs(minForwardForce);
        }
        float driving_adjust = KpDistance * distance_error+ minForwardForce;
        print(driving_adjust);
        setMotorSpeedLeft(driving_adjust);
        setMotorSpeedRight(driving_adjust);
    }
    void seekTarget()
    {
        double steering_adjust = 0.0f;
        if (LL.tvValidTarget == 0.0)
        {
            // We don't see the target, seek for the target by spinning in place at a safe speed.
            if (Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL") > 0)
                steering_adjust = -searchForce - minForce;
            else if (Input.GetAxis("CONTROLLER_LEFT_STICK_HORIZONTAL") < 0)
                steering_adjust = searchForce + minForce;
            print("Seeking");
        }
        else
        {
            // We do see the target, execute aiming code
            //double heading_error = LL.txOffsetX;
            //steering_adjust = 0.3 * LL.txOffsetX;
            float heading_error = (float)LL.txOffsetX;
            steering_adjust = 0.0f;
            if (LL.txOffsetX > 1.0)
            {
                steering_adjust = 0.1 * heading_error - minForce;
            }
            else if (LL.txOffsetX < 1.0)
            {
                steering_adjust = 0.1 * heading_error + minForce;
            }
            print("Adjusting");
        }

        setMotorSpeedLeft(-(float)steering_adjust);
        setMotorSpeedRight((float)steering_adjust);
    }
    // Update is called once per frame
    void Update()
    {
        float moveLeftVertical = Input.GetAxis("WSVertical");
        float moveRightVertical = Input.GetAxis("ArrowsVertical");

        setMotorSpeedLeft(moveLeftVertical);
        setMotorSpeedRight(moveRightVertical);

        JoystickInputs();
    }
}
