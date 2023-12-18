using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CustomPrefabPool : MonoBehaviour, IPunPrefabPool
{
    public GameObject driverPrefab; // Reference to the driver prefab
    public GameObject assistantPrefab; // Reference to the assistant prefab

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject prefabToInstantiate = null;

        if (prefabId == "Driver")
        {
            prefabToInstantiate = driverPrefab;
        }
        else if (prefabId == "Assistant")
        {
            prefabToInstantiate = assistantPrefab;
        }

        if (prefabToInstantiate != null)
        {
            GameObject instantiatedObject = Instantiate(prefabToInstantiate, position, rotation);
            instantiatedObject.SetActive(false);  // Make sure the object is inactive
            return instantiatedObject;
        }
        else
        {
            Debug.LogError("Prefab not found for id: " + prefabId);
            return null;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}

