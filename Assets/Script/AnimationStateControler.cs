using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationStateControler : NetworkBehaviour
{
    public GameObject player;
    public Animator animator;
    [SyncVar(hook = nameof(OnChangeState))]
    public int state;
    private int currentState;

    public float _fastRun;
    public float _slowRun;

    float newOffset;

    bool isRunning;
    bool currentRunnig;

    public float penalty;

    //sound
    private AudioSource[] mySounds;
    private AudioSource RunSd;

    public bool isLaunchingIdle;

    // Start is called before the first frame update
    void Start()
    {
        penalty = 1;
        newOffset = 43;

        _fastRun = 13;
        _slowRun = 7;
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
        var BMScritp = GetComponent<BulletManager>();
        bool isGround = FCScritp.isGrounded;
        float fastRun = _fastRun * penalty;
        float slowRun = _slowRun * penalty;
        if (isLocalPlayer)
        {
            //if (isGround)
            //{

            int upDateState = 0;
            bool isLaunching = animator.GetCurrentAnimatorStateInfo(0).IsName("Launch");
            isLaunchingIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("LaunchIdle");
            if (Input.GetMouseButton(0) && FCScritp.InGame)
            {
                upDateState = 11;
                FCScritp.speed = 0;
            }


            else
            {
                if (!isLaunching)
                {
                    //if (Input.GetButtonDown("Jump"))
                    //   upDateState = 9;

                    if (Input.GetKey("a") && Input.GetKey("w"))
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
                    {
                        upDateState = 0;
                    }
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("LaunchIdle") && !Input.GetMouseButton(0))
            {
               //upDateState = 12;
               FCScritp.speed = 0;
            }

            if (!isGround)
            {
                upDateState = 9;
                //FCScritp.speed = slowRun;
            }

            /*if (GetComponent<FullControl>().isFalling)
            {
                upDateState = 10;
                FCScritp.speed = 4;
            }*/


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

            //}

        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LaunchIdle") && state != 11)
        {
            state = 12;
        }

        if (currentState != state)
        {
            animator.SetInteger("AnimState", state);
            GetComponent<FullControl>().SelfBody.GetComponent<UpdateTargetOffset>().UpdateAnimationOffset(state);
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
    