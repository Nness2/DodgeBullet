using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class TargetAnimator : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int PlayerId;
    //public GameObject HeadTarget;
    //public GameObject LeftHandTarget;

    // Start is called before the first frame update
    void Start()
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == PlayerId)
            {
                //transform.parent = child.transform;
            } 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnChangePlayer(int oldValue, int newValue)
    {
        PlayerId = newValue;
    }
}
