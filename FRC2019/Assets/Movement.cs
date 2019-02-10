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

    public float minForce = 0.05f;
    float searchForce = 0.75f;
    float distanceRange = 1f;
    bool aligning = false;
    double degreeOffset = 0;
    double distanceOffset = 0;
    public double yaw = 0;
    double lastYaw = 0;
    public double multiplier = 0;
    Vector3 start;
    double encoderStartValue = 0;

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
            //seekPositioning();
            //seekTargetRange();
            if (!aligning)
            {
                aligning = true;
                degreeOffset = yaw - LL.txOffsetX;
                distanceOffset = Vector3.Distance(transform.position, FrontCam.GetComponent<Example>().CurrentTarget.transform.position);// EstimateDistance();
                start = transform.position;
            }
            else
            {
                double[] speedValues = seekTarget(degreeOffset, distanceOffset, encoderStartValue);
                setMotorSpeedLeft((float)speedValues[0]);
                setMotorSpeedRight((float)speedValues[1]);
            }
        }
        else
        {
            aligning = false;
            float LeftVerticalJoy = Input.GetAxis("Left_Vertical_Joystick");
            float RightVerticalJoy = Input.GetAxis("CONTROLLER_RIGHT_STICK_HORIZONTAL");
            setMotorSpeedLeft(LeftVerticalJoy);
            setMotorSpeedRight(LeftVerticalJoy);
            setMotorSpeedLeft(RightVerticalJoy);
            setMotorSpeedRight(-RightVerticalJoy);
        }

    }
    float EstimateDistance()
    {
        if (LL.tvValidTarget == 0.0)
            return 0.0f;
        float h1 = transform.position.y;
        float h2 = FrontCam.GetComponent<Example>().CurrentTarget.transform.position.y;
        float a1 = Mathf.Abs(360 - transform.localEulerAngles.x);
        float a2 = (float)LL.tsSkewRotation;
        float d = (h2 - h1) / (float)(Mathf.Tan(Mathf.Deg2Rad * (a1 + a2)));
        return d;
    }
    float getDistanceTraveled(double encoderStartValue)
    {
        return Vector3.Distance(start, transform.position);
    }
    double[] seekTargetRange(double distanceOffset,double encoderStartValue)
    {
        float KpDistance = 1f;  // Proportional control constant for distance
        float distance_error = (float)distanceOffset -.2f - getDistanceTraveled(encoderStartValue);
        float minForwardForce = 0.1f;
        if (distance_error < 0)
        {
            minForwardForce = -0.1f;
        }
        float driving_adjust = KpDistance * distance_error + minForwardForce;
        return new double[] { driving_adjust, driving_adjust };
    }
    double[] seekTarget(double degreeOffset, double distanceOffset, double encoderStartValue)
    {

        double leftOut = seekTargetRange(distanceOffset,encoderStartValue)[0]+PigeonTurn(degreeOffset)[0];
        double rightOut = seekTargetRange(distanceOffset, encoderStartValue)[1] + PigeonTurn(degreeOffset)[1];
        double[] values = new double[] {  leftOut , rightOut };
        return values;
    }
    public double[] PigeonTurn(double degrees)
    {
        double KpAim = 0.1f;
        double AimMinCmd = 0.4f;
        //90 - 0
        //-90
        double aim_error = (degrees - yaw);
        double steering_adjust = 0;

        if (aim_error > 0)
        {
            if (aim_error < 1) AimMinCmd = 0.025;
            steering_adjust = (KpAim * aim_error) + AimMinCmd;
        }
        else if (aim_error < 0)
        {
            if (aim_error > -1) AimMinCmd = 0.025;
            steering_adjust = (KpAim * aim_error) - AimMinCmd;
        }
        double leftOut = steering_adjust;
        double rightOut = -steering_adjust;
        double[] output = { leftOut, rightOut };
        return output;
    }
    // Update is called once per frame
    void Update()
    {
        if (lastYaw - (360 * multiplier) > 270 && transform.eulerAngles.y >= 0 && transform.eulerAngles.y <= 90)
        {
            ++multiplier;
        }
        if (lastYaw - (360 * multiplier) < 90 && transform.eulerAngles.y >= 270 && transform.eulerAngles.y <= 360)
        {
            --multiplier;
        }
        yaw = transform.eulerAngles.y + (360 * multiplier);
        lastYaw = yaw;
        float moveLeftVertical = Input.GetAxis("WSVertical");
        float moveRightVertical = Input.GetAxis("ArrowsVertical");

        setMotorSpeedLeft(moveLeftVertical);
        setMotorSpeedRight(moveRightVertical);

        JoystickInputs();
    }
}