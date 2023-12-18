using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dashboard : MonoBehaviour
{

    public NewCarPhysics physicsScript;
    public CalculateMPH speed;
    //public TMP_Text gearText;
    public TMP_Text MPHText;
    //public TMP_Text RPMText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int intSpeed = (int)speed.mph;
        int intRPM = (int)physicsScript.RPM;

        //gearText.text = physicsScript.currentGear.ToString();
        MPHText.text = intSpeed.ToString();
        //RPMText.text = intRPM.ToString();
    }
}
