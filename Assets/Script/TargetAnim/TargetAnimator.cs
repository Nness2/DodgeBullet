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
