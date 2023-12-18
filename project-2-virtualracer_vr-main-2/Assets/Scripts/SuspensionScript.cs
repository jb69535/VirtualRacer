using System.Net.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SuspensionScript : MonoBehaviour
{
    private Rigidbody rb;

    public CalculateMPH speed;

    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearLeft;
    public bool wheelRearRight;

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    public float springForce;
    private float damperForce;
    private float springVelocity;

    [Header("Wheel")]
    public float steerAngle;

    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;
    public float Fx;
    private float Fy;

    [Header("Wheel")]
    public float wheelRadius;

    [Header("Gas")]
    public float rpm;
    public float maxRPM = 9000;
    public float torque;
    public float mph;

    [Header("Transmission")]
    public int gear = 0;
    public float[] gearRatios;

    [Header("Braking")]
    public float brakeForce = 5000f;
    public float brakeTorque = 300f;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + steerAngle, transform.localRotation.z);

        Debug.DrawRay(transform.position, -transform.up * (springLength + wheelRadius), Color.green);

        // Acceleration and RPM logic
        if (Input.GetAxis("Vertical") > 0)
        {
            rpm = Mathf.Clamp(rpm + 20, 1000, maxRPM);
            // Update the torque based on the current RPM and gear ratio
            torque = CalculateTorque(rpm) * gearRatios[gear];
        } 
        else if (Input.GetAxis("Vertical") < 0)
        {
            rpm = Mathf.Clamp(rpm - 50, 1000, maxRPM);
        } 
        else
        {
            // Natural deceleration when no input is provided
            rpm = Mathf.Clamp(rpm - 10, 1000, maxRPM);
            // Adding drag for natural deceleration (air resistance, engine braking)
            rb.AddForce(-rb.velocity * 0.05f);  // Adjust the 0.05f value as needed for realistic drag
        } 

        // Gear shifting logic
        if (Input.GetKeyDown(KeyCode.E))
        {
            gear = Mathf.Clamp(gear + 1, 0, gearRatios.Length - 1);
            rpm = Mathf.Clamp(rpm * 0.7f, 1000, maxRPM);
        } 
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            gear = Mathf.Clamp(gear - 1, 0, gearRatios.Length - 1);
            rpm = Mathf.Clamp(rpm * 1.5f, 1000, maxRPM);
        }

        // Calculate MPH
        if (speed != null) {
            mph = speed.mph;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            lastLength = springLength;

            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;
            wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

            // Applying forces based on acceleration input and torque
            if (Input.GetAxis("Vertical") != 0)
            {
                Fx = Input.GetAxis("Vertical") * torque;
                Fy = wheelVelocityLS.x * torque;
                rb.AddForceAtPosition(suspensionForce + (Fx * transform.forward) + (Fy * -transform.right), hit.point);
            }

            // Braking logic
            if (Input.GetButton("Fire1"))
            {
                Vector3 brakingForce = -brakeForce * transform.forward;
                rb.AddForceAtPosition(brakingForce, hit.point);

                // Applying brake torque to slow down the wheel's rotation
                if (wheelFrontLeft || wheelFrontRight)
                {
                    rb.AddTorque(-brakeTorque * transform.up);
                }
            }
        } 
    }

    // Custom function to calculate torque based on RPM
    private float CalculateTorque(float rpm)
    {
        // Example torque curve formula - adjust as needed
        return -0.000005f * (rpm - 5000) * (rpm - 5000) + 400;
    }
}
