using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roleManager : MonoBehaviour
{

    public RoleSelection role;
    public GameObject logitecWheel;
    public GameObject car;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (role.playerDriver())
        {
            logitecWheel.SetActive(true);
            car.GetComponent<NewCarPhysics>().enabled = true;
            car.GetComponent<respawnCar>().enabled = true;
        } else
        {
            logitecWheel.SetActive(false);
            //car.GetComponent<NewCarPhysics>().enabled = false;
            car.GetComponent<respawnCar>().enabled = false;
        }
    }
}
