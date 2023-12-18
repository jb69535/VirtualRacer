using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{

    public GameObject steeringWheel;
    public NewCarPhysics wheelInput;
    private Vector3 rotationPoint;

    // Start is called before the first frame update
    void Start()
    {
        Quaternion newRotation = Quaternion.AngleAxis(90, Vector3.up);
        steeringWheel.transform.rotation = newRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //rotationPoint = new Vector3(steeringWheel.transform.rotation.x, steeringWheel.transform.rotation.y, wheelInput.steeringInput * 180);
        //rotationPoint = new Vector3(steeringWheel.transform.eulerAngles.x, wheelInput.steeringInput * 180, steeringWheel.transform.eulerAngles.z);
        //steeringWheel.transform.eulerAngles = rotationPoint;

        //Quaternion newRotation = Quaternion.AngleAxis(wheelInput.steeringInput * 180, Vector3.forward);
        //steeringWheel.transform.rotation = newRotation;

        steeringWheel.transform.eulerAngles = new Vector3(steeringWheel.transform.eulerAngles.x, steeringWheel.transform.eulerAngles.y, wheelInput.steeringInput * 180);

    }
}