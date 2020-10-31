using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotate : MonoBehaviour
{
    private GameObject MainCamera;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = new Quaternion(transform.localRotation.x, MainCamera.transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
    }
}
