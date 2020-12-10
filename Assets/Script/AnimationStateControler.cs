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

    public float fastRun;
    public float slowRun;

    bool isRunning;
    bool currentRunnig;

    //sound
    private AudioSource[] mySounds;
    private AudioSource RunSd;

    // Start is called before the first frame update
    void Start()
    {
        fastRun = 15;
        slowRun = 8;
        isRunning = false;
        currentRunnig = false;
        mySounds = GetComponents<AudioSource>();
        RunSd = mySounds[0];



        currentState = 0;
        state = 0;
        animator = player.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        var FCScritp = GetComponent<FullControl>();
        bool isGround = FCScritp.isGrounded;

        //Debug.Log(currentState);
        if (isLocalPlayer)
        {
            if (isGround)
            {
                int upDateState = 0;

                if (Input.GetButtonDown("Jump"))
                    upDateState = 9;

                else if (Input.GetKey("a") && Input.GetKey("w"))
                {
                    upDateState = 5;
                    FCScritp.speed = fastRun;
                }

                else if (Input.GetKey("d") && Input.GetKey("w")) 
                {
                    upDateState = 6;
                    FCScritp.speed = fastRun;


                }

                else if (Input.GetKey("a") && Input.GetKey("s"))
                {
                    upDateState = 7;
                    FCScritp.speed = slowRun;
                }

                else if (Input.GetKey("d") && Input.GetKey("s"))
                {
                    upDateState = 8;
                    FCScritp.speed = slowRun;
                }

                else if (Input.GetKey("w"))
                {
                    upDateState = 1;
                    FCScritp.speed = fastRun;


                }

                else if (Input.GetKey("s"))
                {
                    upDateState = 2;
                    FCScritp.speed = slowRun;
                }

                else if (Input.GetKey("d"))
                {
                    upDateState = 3;
                    FCScritp.speed = slowRun;
                }

                else if (Input.GetKey("a"))
                {
                    upDateState = 4;
                    FCScritp.speed = slowRun;
                }

                else
                    upDateState = 0;

                

                if (state != upDateState)
                {
                    CmdUpdateState(upDateState);
                    state = upDateState;
                }

                if (state > 0 && state < 9)
                    isRunning = true;
                else
                    isRunning = false;

                if (currentRunnig != isRunning)
                {
                    currentRunnig = isRunning;
                    if (isRunning)
                        RunSd.Play();
                    else
                        RunSd.Stop();

                }

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
    