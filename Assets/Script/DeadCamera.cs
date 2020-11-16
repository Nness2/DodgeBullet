using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeadCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (GetComponent<CinemachineFreeLook>().Follow == null && GetComponent<CinemachineFreeLook>().LookAt == null) {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("CameraTop");

            GetComponent<CinemachineFreeLook>().Follow = characters[0].transform;
            GetComponent<CinemachineFreeLook>().LookAt = characters[0].transform;
        }
    }
}
