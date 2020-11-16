using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationStateControler : NetworkBehaviour
{
    public GameObject player;
    Animator animator;
    [SyncVar(hook = nameof(OnChangeState))]
    public int state;
    private int currentState;
    // Start is called before the first frame update
    void Start()
    {
        currentState = 0;
        state = 0;
        animator = player.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            int upDateState = 0;

            if (Input.GetKey("a") && Input.GetKey("w"))
                upDateState = 5;

            else if (Input.GetKey("space"))
                upDateState = 9;

            else if (Input.GetKey("w"))
                upDateState = 1;

            else if (Input.GetKey("d"))
                upDateState = 3;


            else
                upDateState = 0;

            if (state != upDateState)
            {
                CmdUpdateState(upDateState);
                state = upDateState;
            }
        }

        if(currentState != state)
        {
            animator.SetInteger("AnimState", state);
            currentState = state;
        }
            

    }

    [Command]
    void CmdUpdateState(int value)
    {
        state = value;
    }


    void OnChangeState(int oldValue, int newValue)
    {
        state = newValue;
    }

}
    