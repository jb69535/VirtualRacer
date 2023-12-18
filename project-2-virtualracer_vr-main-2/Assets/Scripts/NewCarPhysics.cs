using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine.EventSystems;

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
};

public class NewCarPhysics : MonoBehaviour
{

    [Header("Wheel Inputs")]
    public InputManager wheelInput;
    public float pedalGas;
    public float pedalBrake;
    public float wheelBrakeInput;
    public float pedalClutch;
    public float wheelSteering;
    public float wheelSensitivity = 1.0f;
    public int wheelGear;

    [Header("Wheel Outputs")]
    public int dirtEffect;

    [Header("Wheel Colliders")]
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelCollider groundedWheel;
    public WheelMeshes wheelMeshes;
    //public WheelParticles wheelParticles;

    [Header("Car Power")]
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    //public GameObject smokePrefab;
    public float motorPower;
    public float brakePower;
    public float slipAngle;
    public float speed;
    private float speedClamped;
    public float maxSpeed;
    public AnimationCurve steeringCurve;

    //public Mybutton gasPedal;
    //public Mybutton brakePedal;
    //public Mybutton leftButton;
    //public Mybutton rightButton;
    public int isEngineRunning;

    [Header("RPM")]
    public float RPM;
    public float redLine;
    public float idleRPM;
    //public TMP_Text rpmText;
    //public TMP_Text gearText;
    //public Transform rpmNeedle;
    //public float minNeedleRotation;
    //public float maxNeedleRotation;
    public int currentGear;
    public CalculateMPH mph;
    public int realMPH;

    [Header("Power Delivery")]
    public float[] gearRatios;
    public float differentialRatio;
    public float currentTorque;
    private float clutch;
    private float wheelRPM;
    public AnimationCurve hpToRPMCurve;
    private GearState gearState;
    public float increaseGearRPM;
    public float decreaseGearRPM;
    public float changeGearTime = 0.5f;

    //public GameObject tireTrail;
    //public Material brakeMaterial;
    //public Color brakingColor;
    //public float brakeColorIntensity;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        InitiateParticles();
    }

    void InitiateParticles()
    {
        /*
        if (smokePrefab)
        {
            wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
                .GetComponent<ParticleSystem>();
            wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FLWheel.transform)
                .GetComponent<ParticleSystem>();
            wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
                .GetComponent<ParticleSystem>();
            wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RLWheel.transform)
                .GetComponent<ParticleSystem>();
        }
        if (tireTrail)
        {
            wheelParticles.FRWheelTrail = Instantiate(tireTrail, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
                .GetComponent<TrailRenderer>();
            wheelParticles.FLWheelTrail = Instantiate(tireTrail, colliders.FLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FLWheel.transform)
                .GetComponent<TrailRenderer>();
            wheelParticles.RRWheelTrail = Instantiate(tireTrail, colliders.RRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
                .GetComponent<TrailRenderer>();
            wheelParticles.RLWheelTrail = Instantiate(tireTrail, colliders.RLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RLWheel.transform)
                .GetComponent<TrailRenderer>();
        }
        */
    }
    // Update is called once per frame

    void Update()
    {
        //rpmNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLine * 1.1f)));
        //rpmText.text = RPM.ToString("0,000") + "rpm";
        //gearText.text = (gearState == GearState.Neutral) ? "N" : (currentGear + 1).ToString();
        speed = colliders.RRWheel.rpm * colliders.RRWheel.radius * 2f * Mathf.PI / 10f;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.deltaTime);
        pedalGas = wheelInput.gasInput;
        pedalBrake = wheelInput.brakeInput;
        pedalClutch = wheelInput.clutchInput;
        wheelSteering = wheelInput.steerInput;
        wheelGear = wheelInput.buttonPressed - 12; // the button for shifting gears start at 11
        dirtEffect = (int)(mph.mph / 2);
        realMPH = (int)mph.mph;
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)) // If the wheel is connected
        {
            CheckWheelInput();
            ApplyWheelEffects();
            //CheckInput();
        } else // if wheel is not connected
        {
            CheckInput();

            
        }
       // CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        CheckParticles();
        ApplyWheelPositions();
        ChangingGears();
        
        

    }

    void CheckInput()
    {
        
        gasInput = Input.GetAxis("Vertical");
        //gasInput = gasInput + 1;

        if (Input.GetKey(KeyCode.W))
        {
            gasInput = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            gasInput = -1;
        }
        if (Mathf.Abs(gasInput) > 0 && isEngineRunning == 0)
        {
            //StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
        }
        steeringInput = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.D))
        {
            steeringInput += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            steeringInput -= 1;
        }
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);

        float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (Mathf.Abs(gasInput) > 0) gearState = GearState.Running;
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0;
        }
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0;
        }




    }

    void CheckWheelInput()
    {
        if (pedalGas > 0.1)
        {
            if (wheelGear == -1)
            {
                gasInput = pedalGas;
                currentGear = 6;
            } else
            {
                gasInput = pedalGas;
            }
            
        } else
        {
            gasInput = 0;
        }     
        if (pedalBrake > 0.2) {
            //gasInput = (-pedalBrake) * realMPH;
            brakeInput = Mathf.Abs(pedalBrake);
        } else
        {
            brakeInput = 0;
        }
        clutch = -pedalClutch + 1;
        if (Mathf.Abs(gasInput) > 0 && isEngineRunning == 0)
        {
            //StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
        }
        if (wheelGear == -1)
        {
           // Debug.Log("Reverse Gear");
        }

        wheelBrakeInput = pedalBrake;
        steeringInput = wheelSteering * wheelSensitivity;

        
        if (Mathf.Abs(gasInput) > 0 && isEngineRunning == 0)
        {
            //StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
        }
        steeringInput = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.D))
        {
            steeringInput += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            steeringInput -= 1;
        }
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);

        float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (Mathf.Abs(gasInput) > 0) gearState = GearState.Running;
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0;
        }
        /*
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0;
        }
        */


        
    }

    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;

        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        /*
        if (brakeMaterial)
        {
            if (brakeInput > 0)
            {
                brakeMaterial.EnableKeyword("_EMISSION");
                brakeMaterial.SetColor("_EmissionColor", brakingColor * Mathf.Pow(2, brakeColorIntensity));
            }
            else
            {
                brakeMaterial.DisableKeyword("_EMISSION");
                brakeMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
        */


    }
    void ApplyMotor()
    {

        currentTorque = CalculateTorque();
        colliders.RRWheel.motorTorque = currentTorque * gasInput;
        colliders.RLWheel.motorTorque = currentTorque * gasInput;
        colliders.FRWheel.motorTorque = currentTorque * gasInput;
        colliders.FLWheel.motorTorque = currentTorque * gasInput;

    }

    float CalculateTorque()
    {
        float torque = 0;
        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }
        if (gearState == GearState.Running && clutch > 0)
        {
            if (RPM > increaseGearRPM)
            {
                //StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM)
            {
                //StartCoroutine(ChangeGear(-1));
            }
        }
        if (isEngineRunning > 0)
        {
            if (clutch < 0.1f)
            {
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * gasInput) + Random.Range(-50, 50), Time.deltaTime);
            }
            else
            {
                wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
                torque = (hpToRPMCurve.Evaluate(RPM / redLine) * motorPower / RPM) * gearRatios[currentGear] * differentialRatio * 5252f * clutch;
            }
        }
        return torque;
    }

    void ApplySteering()
    {

        float steeringAngle = steeringInput * steeringCurve.Evaluate(mph.mph) * wheelSensitivity;
        if (slipAngle < 120f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
        }
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        //Debug.Log(steeringAngle);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }
    void CheckParticles()
    {
        /*
        WheelHit[] wheelHits = new WheelHit[4];
        colliders.FRWheel.GetGroundHit(out wheelHits[0]);
        colliders.FLWheel.GetGroundHit(out wheelHits[1]);

        colliders.RRWheel.GetGroundHit(out wheelHits[2]);
        colliders.RLWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.2f;
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance))
        {
            wheelParticles.FRWheel.Play();
            wheelParticles.FRWheelTrail.emitting = true;
        }
        else
        {
            wheelParticles.FRWheel.Stop();

            wheelParticles.FRWheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance))
        {
            wheelParticles.FLWheel.Play();

            wheelParticles.FLWheelTrail.emitting = true;
        }
        else
        {
            wheelParticles.FLWheel.Stop();

            wheelParticles.FLWheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance))
        {
            wheelParticles.RRWheel.Play();

            wheelParticles.RRWheelTrail.emitting = true;
        }
        else
        {
            wheelParticles.RRWheel.Stop();

            wheelParticles.RRWheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance))
        {
            wheelParticles.RLWheel.Play();

            wheelParticles.RLWheelTrail.emitting = true;
        }
        else
        {
            wheelParticles.RLWheel.Stop();

            wheelParticles.RLWheelTrail.emitting = false;
        }
        */

    }
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }
    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(Mathf.Abs(gasInput), 0.5f, 1f);
        return RPM * gas / redLine;
    }

    public void ChangingGears()
    {
        if (wheelGear >= 0 && wheelGear < 6)
        {
            shifterGears(wheelGear);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ChangeGear(1));
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ChangeGear(-1));
        }
    }
    void shifterGears(int gear)
    {
        currentGear = gear;
    }
    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if (currentGear + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                //increase the gear
                yield return new WaitForSeconds(0.7f);
                if (RPM < increaseGearRPM || currentGear >= gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                //decrease the gear
                yield return new WaitForSeconds(0.1f);

                if (RPM > decreaseGearRPM || currentGear <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGear += gearChange;
        }

        if (gearState != GearState.Neutral)
            gearState = GearState.Running;
    }

    public void ApplyWheelEffects()
    {
        WheelHit hit;
        bool GroundedL = groundedWheel.GetGroundHit(out hit); // Only plays the effects when the car is grounded
        if (GroundedL)
        {
            LogitechGSDK.LogiPlayDirtRoadEffect(0, dirtEffect); // Plays the vibration to mimic driving on dirt
            LogitechGSDK.LogiPlaySpringForce(0, 0, 50, 50); // Activates a spring force so turning is not unrealistically light
        }
        else
        {
            LogitechGSDK.LogiStopDirtRoadEffect(0);
            LogitechGSDK.LogiStopSpringForce(0);
        }

        //Dirt Road Effect-> I
        if (Input.GetKeyUp(KeyCode.I))
        {
            if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DIRT_ROAD))
            {
                LogitechGSDK.LogiStopDirtRoadEffect(0);
            }
            else
            {
                LogitechGSDK.LogiPlayDirtRoadEffect(0, dirtEffect);
            }

        }
    }

}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}
[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}
[System.Serializable]
public class WheelParticles
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;

    public TrailRenderer FRWheelTrail;
    public TrailRenderer FLWheelTrail;
    public TrailRenderer RRWheelTrail;
    public TrailRenderer RLWheelTrail;

}