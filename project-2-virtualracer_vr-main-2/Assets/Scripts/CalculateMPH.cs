using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateMPH : MonoBehaviour
{
    public float mph;

    void Start()
    {
        StartCoroutine(CalcSpeed());
    }

    IEnumerator CalcSpeed()
    {
        Vector3 prevPos = transform.position;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            mph = Vector3.Distance(transform.position, prevPos) / Time.fixedDeltaTime * 2.23694f; // Converts meters/second to MPH
            prevPos = transform.position;
        }
    }
}

