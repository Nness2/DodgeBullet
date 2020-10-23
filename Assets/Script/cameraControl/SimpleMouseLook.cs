using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float rotationX;
    public float rotationY;
    public float sensitivity = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMouseOffScreen())
        {
            rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
            rotationY += Input.GetAxis("Mouse X") * sensitivity;

            rotationX = Mathf.Clamp(rotationX, -90, 90);
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);


        }

    }

    private bool isMouseOffScreen() { 
    {
        if (Input.mousePosition.x <= 2 || Input.mousePosition.y <=2 || Input.mousePosition.x >= Screen.width - 2 || Input.mousePosition.y >= Screen.height - 2)
            return true;
        }
        return false;
    }
}
