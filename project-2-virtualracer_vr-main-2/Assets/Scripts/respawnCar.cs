using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawnCar : MonoBehaviour
{

    public InputManager input;
    public GameObject spawn;
    public GameObject car;
    public NewCarPhysics physics;
    public WheelCollider[] wheels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            if (input.buttonPressed == 8)
            {
                Debug.Log("Respawning Car");
                car.transform.position = spawn.transform.position;
                car.transform.rotation = spawn.transform.rotation;
                // resetting car physics
                physics.speed = 0;
                physics.gasInput = 0;
                physics.brakeInput = 0;
                car.GetComponent<Rigidbody>().velocity = Vector3.zero;
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].rotationSpeed = 0;
                }
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Respawning Car");
                car.transform.position = spawn.transform.position;
                car.transform.rotation = spawn.transform.rotation;
                // resetting car physics
                physics.speed = 0;
                physics.gasInput = 0;
                physics.brakeInput = 0;
                car.GetComponent<Rigidbody>().velocity = Vector3.zero;
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].rotationSpeed = 0;
                }
            }
        }
            

    }
}
