using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public WheelCollider WheelL, WheelR, WheelRR, WheelRL;
    public float AntiRoll = 5000f;

    Rigidbody car;

    void Awake()
    {
        car = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float TravelL = 1f, TravelR = 1f;
        WheelHit hit;

        bool GroundedL = WheelL.GetGroundHit(out hit);
        bool GroundedR = WheelR.GetGroundHit(out hit);
        if (GroundedL)
        {
            TravelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
        }
        if (GroundedR)
        {
            TravelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
        }

        float AntiRollForce = (TravelL - TravelR) * AntiRoll;

        if (GroundedL)
        {
            car.AddForceAtPosition(WheelL.transform.up * -AntiRollForce, WheelL.transform.position);
            car.AddForceAtPosition(WheelRL.transform.up * -AntiRollForce, WheelRL.transform.position);
        }

        if (GroundedR)
        {
            car.AddForceAtPosition(WheelR.transform.up * -AntiRollForce, WheelR.transform.position);
            car.AddForceAtPosition(WheelRR.transform.up * -AntiRollForce, WheelRR.transform.position);
        }
    }
}
