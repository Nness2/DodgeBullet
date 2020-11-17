﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;


public class FullControl : NetworkBehaviour
{
    private int playerNumber;

    private GameObject LocalPlayer;

    public CharacterController controller;
    private GameObject cam;

    public float speed = 6f;
    public float turnSmoothTime = 0.0f;
    float turnSmoothVelocity;
    // Start is called before the first frame update

    //private GameObject MainCamera;

    //JUMP
    public float gravity = -9.81f; // default : Earth gravity
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    private bool isGrounded;
    private Vector3 _move;
    [SerializeField]
    private Vector3 velocity; // for falling speed

    //Fire  
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public GameObject gun;

    [SyncVar(hook = nameof(OnChangeNumber))]
    public int selfNumber;

    private NetworkManager nm;

    public bool isLocal;

    public bool GotBall;

    void Start()
    {
        
        playerNumber = 0;
        var ZL = GetComponent<ZoneLimitations>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (isLocalPlayer)
        {

            GotBall = true;
            isLocal = true;
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {

                if (child.CompareTag("CameraTop"))
                {
                    Transform topCam = child;
                    LocalPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
                    LocalPlayer.transform.parent = topCam;
                    LocalPlayer.transform.localPosition = new Vector3();
                }
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            controller = gameObject.GetComponent<CharacterController>();
            Assert.IsNotNull(groundCheck);
            selfNumber = cmptPlayers();
            InitSelfNb(cmptPlayers());
            teamManager();

            //Set Position
            controller.enabled = false;
            if (selfNumber % 2 == 1)
                transform.position = GameObject.FindGameObjectWithTag("BlueFieldSpawner").transform.position;

            else
                transform.position = GameObject.FindGameObjectWithTag("RedFieldSpawner").transform.position;
            controller.enabled = true;



        }
        else
        {
            //gameObject.layer = 9;
            selfNumber = cmptPlayers();
            isLocal = false;
            Transform[] Children = GetComponentsInChildren<Transform>();
            foreach (Transform child in Children)
            {
                //Destroy(child.gameObject.GetComponent<CharacterController>());
                //Destroy(child.gameObject.GetComponent<Health>());
                if (child.CompareTag("Untagged"))
                {
                    Destroy(child.gameObject);
                }

            }
        }

    }



    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
            return;

        Jump();

        if (Input.GetMouseButtonDown(1))
        {
            if (GotBall)
            {
                GotBall = true;
                Vector3 position = cam.transform.position;
                Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
                CmdBallFire(selfNumber, position, forward);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = cam.transform.position;
            Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
            CmdFire(position, forward);

        }


        transform.rotation = new Quaternion(cam.transform.localRotation.x, cam.transform.localRotation.y, cam.transform.localRotation.z, cam.transform.localRotation.w);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        gun.transform.rotation = new Quaternion(cam.transform.localRotation.x, cam.transform.localRotation.y, cam.transform.localRotation.z, cam.transform.localRotation.w);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
         
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        if (cmptPlayers() != playerNumber)
        {
            teamManager();
            playerNumber = cmptPlayers();
            
        }
    }
    
    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jumping : y = √(h * -2 * g)
        }

        velocity.y += gravity * Time.deltaTime; // falling : ∆y = 1/2g * t^2 

        controller.Move(velocity * Time.deltaTime);


    }

    #region Unity BallKill
    [Command] //Appelé par le client mais lu par le serveur
    void CmdBallFire(int nb, Vector3 position, Vector3 forward)
    {
        ClientBallFire(nb, position, forward);
    }

    [ClientRpc]
    void ClientBallFire(int nb, Vector3 position, Vector3 forward)
    {
        //Transform camInfo = CamInfo;
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        

        int layerMask = 1 << 8;
        RaycastHit hit;

        if (Physics.Raycast(position, forward, out hit, Mathf.Infinity, layerMask)) 
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log(hit.point);
            Vector3 dir = hit.point - bullet.transform.position;
            dir = dir.normalized;
            bullet.GetComponent<Rigidbody>().AddForce(dir * 20000);
        }
        else
        {
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;
        }

        //NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        bullet.GetComponent<Bullet>().player = nb;
        //Destroy(bullet, 10.0f);
    }

#endregion

    #region Unity Shoot
    [Command]
    void CmdFire(Vector3 position, Vector3 forward)
    {
        int layerMask = 1 << 8;
        RaycastHit hit;

        if (Physics.Raycast(position, forward, out hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log(hit.point);
            //Vector3 dir = hit.point - bullet.transform.position;
            //dir = dir.normalized;
            //bullet.GetComponent<Rigidbody>().AddForce(dir * 10000);
            if (hit.transform.gameObject.GetComponent<FullControl>() != null)
            {
                //Debug.Log(hit.transform.gameObject.GetComponent<FullControl>().selfNumber);
                int playerTouched = hit.transform.gameObject.GetComponent<FullControl>().selfNumber;
                int shooter = selfNumber;
                ClientFire(shooter, playerTouched);
            }
        }
    }

    [ClientRpc]
    void ClientFire(int shooter, int playerTouched)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {

                if (child.GetComponent<FullControl>().selfNumber == playerTouched)
                {

                    bool kill = child.GetComponent<Health>().TakeDamage(20);

                    if (kill) //Si y a kill le joueur redescend
                    {

                        
                        if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill) //Si permier kill, on donne la balle et on bloque  
                            child.GetComponent<Health>().KillManager(shooter, playerTouched, true);
                        else
                            child.GetComponent<Health>().KillManager(shooter, playerTouched, false);

                        child.GetComponent<ZoneLimitations>().UpState();
                        
                    }
                }

            }
            
        }
    }
#endregion

    #region Unity FirstKill

    [Command]
    void CmdFirstKill()
    {
        ClientFirstKill();
    }

    [ClientRpc]
    void ClientFirstKill()
    {
        Debug.Log("Il y a eu un kill.");
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill = true;
    }

    #endregion


    void teamManager()
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            var childFC = child.GetComponent<FullControl>();
            var ZL = child.GetComponent<ZoneLimitations>();
            if (childFC.selfNumber % 2 == 1)
            {
                ZL.teamBlue = true;
                    
                Transform[] ColorChildren = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in ColorChildren)
                {

                    if (child2.CompareTag("Body"))
                    {
                        
                        child2.transform.GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
                    }
                }
            }

            else
            {
                ZL.teamBlue = false;
                Transform[] ColorChildren = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in ColorChildren)
                {

                    if (child2.CompareTag("Body"))
                    {
                        child2.transform.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                    }
                }
            }

        }
    }

    int cmptPlayers()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        return characters.Length;
    }

    [Command] //Appelé par le client mais lu par le serveur
    void InitSelfNb(int nb)
    {
        selfNumber = nb;
    }

    void OnChangeNumber(int oldValue, int newValue)
    {
        selfNumber = newValue;
    }

}
