using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public float steerInput;
    public float gasInput;
    public float clutchInput;
    public float brakeInput;
    public int buttonPressed;


    // Start is called before the first frame update
    void Start()
    {
        steerInput = 0;
        gasInput = 0;
        brakeInput = 0;
        clutchInput = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            steerInput = LogitechInput.GetAxis("Steering Horizontal");
            gasInput = LogitechInput.GetAxis("Gas Vertical");
            clutchInput = LogitechInput.GetAxis("Clutch Vertical");
            brakeInput = LogitechInput.GetAxis("Brake Vertical");

            // Shows Which button is pressed
            for (int i = 0; i < 128; i++)
            {
                if (rec.rgbButtons[i] == 128)
                {
                    buttonPressed = i;
                }

            }
        }

    }
}
