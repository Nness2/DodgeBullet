using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FireVFX : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int player;
    void Start()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().selfNumber == player)// && ZLScript.state > 0)
            {
                Transform[] Children = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in Children)
                {
                    if (child2.CompareTag("BallPose"))
                    {
                        gameObject.transform.parent = child2.transform;

                    }

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnChangePlayer(int oldValue, int newValue)
    {
        player = newValue;
    }
}
