using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogitechInput : MonoBehaviour
{
    static LogitechGSDK.DIJOYSTATE2ENGINES rec;

    public static float GetAxis(string axisName)
    {

        rec = LogitechGSDK.LogiGetStateUnity(0);
        
        // Calculations are to turn the numbers into a range from 0 - 1
        switch(axisName)
        {
            case "Steering Horizontal": return rec.lX / 32768f; // returns a value from -1 to 1
            case "Gas Vertical": return (rec.lY - 32768f) / (-32768f * 2);
            case "Clutch Vertical": return (rec.rglSlider[0] - 32768f) / (-32768f * 2);
            case "Brake Vertical": return (rec.lRz - 32768f) / (-32768f * 2);
        }


        return 0.0f;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
