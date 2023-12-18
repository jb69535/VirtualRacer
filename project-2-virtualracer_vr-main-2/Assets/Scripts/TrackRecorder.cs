using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;  // Ensure Newtonsoft.Json is added to your project

public class TrackRecorder : MonoBehaviour
{
    private List<RaceDataPoint> raceData = new List<RaceDataPoint>();
    private Vector3 previousPosition;
    private float previousTime;

    void Start()
    {
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        previousPosition = transform.position;
        previousTime = Time.time;
    }

    void FixedUpdate()
    {
        float currentTime = Time.time;
        float deltaTime = currentTime - previousTime;
        float speed = Vector3.Distance(transform.position, previousPosition) / deltaTime;

        RaceDataPoint dataPoint = new RaceDataPoint
        {
            position = transform.position,
            rotation = transform.rotation,
            speed = speed,
            time = currentTime
        };

        raceData.Add(dataPoint);

        previousPosition = transform.position;
        previousTime = currentTime;
    }

    public void SaveRaceData(string fileName)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        string jsonData = JsonConvert.SerializeObject(raceData, Formatting.Indented);
        File.WriteAllText(fullPath, jsonData);
        Debug.Log("Race data saved to: " + fullPath);
    }

    public List<RaceDataPoint> GetRaceData()
    {
        return raceData;
    }
}

