using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider[] wheels = new WheelCollider[4];
    public float torque = 200;
    public float stoppingTorque = 100;
    public float steeringMax = 4;
    public InputManager input;


    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    public float gas;

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = torque;
            }
            Debug.Log("Adding Torque");
        }
        else
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = 0;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = stoppingTorque;
            }
            Debug.Log("Adding Torque");
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].steerAngle = Input.GetAxis("Horizontal") * steeringMax;
            }
        }


        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].motorTorque = torque * input.gasInput;
        }
        //Debug.Log("Adding Torque");

        

        // --------------------------------------------------
        //if (Input.GetKey(KeyCode.W))
        //{
        //    for (int i = 0; i < wheels.Length; i++)
        //    {
        //        wheels[i].motorTorque = torque;
        //    }
        //    Debug.Log("Adding Torque");
        //}
        //else
        //{
        //    for (int i = 0; i < wheels.Length; i++)
        //    {
        //        wheels[i].motorTorque = 0;
        //    }
        //}

        //if (Input.GetKey(KeyCode.S))
        //{
        //    for (int i = 0; i < wheels.Length; i++)
        //    {
        //        wheels[i].brakeTorque = stoppingTorque;
        //    }
        //    Debug.Log("Adding Torque");
        //}

        //// Calculate the new velocity
        //currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        //if( currentVelocity.magnitude <= 0.5f)
        //{
        //    currentVelocity = Vector3.zero;
        //}

        //// Update previous position for the next frame
        //previousPosition = transform.position;


        //if (Input.GetAxis("Horizontal") != 0)
        //{

        //    float Rotationspeed = Input.GetKey(KeyCode.A) ? (-20 * currentVelocity.magnitude) : (20 * currentVelocity.magnitude);
        //    this.transform.Rotate(0, Rotationspeed * Time.deltaTime, 0);
        //}

        //for (int i = 0; i < wheels.Length; i++)
        //{
        //    wheels[i].motorTorque = torque * input.gasInput;
        //}



    }

}
