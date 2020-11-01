using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLimitations : MonoBehaviour
{
    private GameObject BlueField;
    private GameObject RedField;
    private GameObject BluePrison;
    private GameObject RedPrison;
    private GameObject NeutreField1;
    private GameObject NeutreField2;

    public bool teamBlue;
    public int state = 0;

    // Start is called before the first frame update
    void Start()
    {
        BlueField = GameObject.FindGameObjectWithTag("BlueField");
        RedField = GameObject.FindGameObjectWithTag("RedField");
        BluePrison = GameObject.FindGameObjectWithTag("BluePrison");
        RedPrison = GameObject.FindGameObjectWithTag("RedPrison");
        NeutreField1 = GameObject.FindGameObjectWithTag("Neutre1");
        NeutreField2 = GameObject.FindGameObjectWithTag("Neutre2");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "BlueField" && !teamBlue || collision.gameObject.tag == "BlueField" && teamBlue && state != 0)
        {
            Debug.Log("35 Damages");
        }

        if (collision.gameObject.tag == "RedField" && teamBlue || collision.gameObject.tag == "RedField" && !teamBlue && state != 0)
        {
            Debug.Log("35 Damages");
        }

        if (collision.gameObject.tag == "BluePrison" && !teamBlue || collision.gameObject.tag == "BluePrison" && teamBlue && state != 1)
        {
            Debug.Log("50 Damages");
        }

        if (collision.gameObject.tag == "RedPrison" && teamBlue || collision.gameObject.tag == "RedPrison" && !teamBlue && state != 1)
        {
            Debug.Log("50 Damages");
        }

        if (collision.gameObject.tag == "Neutre1" && state != 2)
        {
            Debug.Log("20 Damage");
        }

        if (collision.gameObject.tag == "Neutre2" && state != 3)
        {
            Debug.Log("20 Damage");
        }

    }

}
