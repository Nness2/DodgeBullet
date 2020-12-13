using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBord : MonoBehaviour
{

    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
