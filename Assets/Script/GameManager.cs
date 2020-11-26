using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool firstKill;
    public string LocalName;
    void Start()
    {
        LocalName = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
        Debug.Log(LocalName);
        firstKill = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
