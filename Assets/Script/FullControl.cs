using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;


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

    private GameObject MainCamera;

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

    [SyncVar(hook = "OnChangeNumber")]
    public int selfNumber;

    private NetworkManager nm;
    
    void Start()
    {

        playerNumber = cmptPlayers();
        var ZL = GetComponent<ZoneLimitations>();
        if (isLocalPlayer)
        {
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


            cam = GameObject.FindGameObjectWithTag("MainCamera");
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            controller = gameObject.GetComponent<CharacterController>();
            Assert.IsNotNull(groundCheck);
            selfNumber = cmptPlayers();
            teamManager();
        }
        /*Transform[] ColorChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child2 in ColorChildren)
        {

            if (child2.CompareTag("Body"))
            {
                child2.transform.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }µ*/

        //Debug.Log(cmptPlayers());
        //teamManager();
        //StartCoroutine("LoadTeam");
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
            return;

        Jump();
        if (Input.GetMouseButtonDown(0))
        {
            CmdFire();

        }



        transform.rotation = new Quaternion(transform.localRotation.x, MainCamera.transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
        gun.transform.rotation = new Quaternion(MainCamera.transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);

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
        if (cmptPlayers() != playerNumber)
        {
            selfNumber = selfNumber;
            teamManager();
            playerNumber = cmptPlayers();
        }
    }


    /*private void MovePlayer()
    {
        _move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        _move *= speed * Time.deltaTime;

        controller.Move(_move);
    }*/


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

    //[Command]
    void CmdFire()
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        int layerMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log(hit.point);
            Vector3 dir = hit.point - bullet.transform.position;
            dir = dir.normalized;
            bullet.GetComponent<Rigidbody>().AddForce(dir * 10000);
        }
        else
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;

        //Network.Spawn(bullet);
        Destroy(bullet, 2.0f);
    }

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
                child.transform.position = GameObject.FindGameObjectWithTag("BlueFieldSpawner").transform.position;
                Transform[] ColorChildren = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in ColorChildren)
                {

                    if (child2.CompareTag("Body"))
                    {
                        child2.transform.GetComponent<MeshRenderer>().material.color = Color.blue;
                    }
                }
            }

            else
            {
                ZL.teamBlue = false;
                child.transform.position = GameObject.FindGameObjectWithTag("RedFieldSpawner").transform.position;
                Transform[] ColorChildren = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in ColorChildren)
                {

                    if (child2.CompareTag("Body"))
                    {
                        child2.transform.GetComponent<MeshRenderer>().material.color = Color.red;
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

    void OnChangeNumber(int nb)
    {
        selfNumber = nb;
    }

    /*IEnumerator LoadTeam()
    {
        yield return new WaitForSeconds(5f);
        teamManager();
        Debug.Log(cmptPlayers());
    }*/
}
