using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class TargetAnimator : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int PlayerId;
    public bool parented;

    //public GameObject HeadTarget;
    //public GameObject LeftHandTarget;

    // Start is called before the first frame update
    void Start()
    {
        parented = false;
    }


// Update is called once per frame
void Update()
    {
        if (PlayerId != 0 && !parented)
        {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().PlayerID == PlayerId)
                {
                    gameObject.transform.parent = child.transform;
                }
            }
            parented = true;
        }
    }

    void OnChangePlayer(int oldValue, int newValue)
    {
        PlayerId = newValue;

    }
}
