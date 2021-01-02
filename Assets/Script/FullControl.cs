using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Mirror;
using Cinemachine;
using UnityEngine.SceneManagement;

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
    public bool isGrounded;
    private Vector3 _move;
    [SerializeField]
    private Vector3 velocity; // for falling speed

    //FireVFX
    public GameObject bulletVFXPrefab;

    //Fire  
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public GameObject BodyPrefab;
    public GameObject StartCanvas;

    public GameObject gun;

    private GameObject GameMng;

    [SyncVar(hook = nameof(OnChangeNumber))]
    public int selfNumber;

    private NetworkManager nm;

    public bool isLocal;

    [SyncVar(hook = nameof(OnChangeGotBall))]
    public bool GotBall;

    public int killNbr;

    public bool isBlue;

    [SyncVar(hook = nameof(OnDeadChange))]
    public bool dead;

    public int PlayerID;

    public bool Replay;


    public bool InGame;
    public bool OnLobby;

    void Start()
    {

        InitPlayerBody();
        Replay = false;
        OnLobby = true;

        PlayerID = (int)netId;
        playerNumber = 0;
        var ZL = GetComponent<ZoneLimitations>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        killNbr = 0;

        if (isLocalPlayer)
        {
            //DontDestroyOnLoad(transform.gameObject);
            InGame = false;

            Replay = false;
            dead = false;
            GameObject.FindGameObjectWithTag("cinemachineCamera").GetComponent<CinemachineFreeLook>().enabled = true;

            GameMng = GameObject.FindGameObjectWithTag("GameManager");
            GetComponent<GameInfos>().addGetNames();
            GotBall = false;
            isLocal = true;
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {

                if (child.CompareTag("CameraTop"))
                {
                    Transform topCam = child;
                    LocalPlayer = GameObject.FindGameObjectWithTag("traveling");
                    LocalPlayer.transform.parent = topCam;
                    LocalPlayer.transform.localPosition = new Vector3();
                    LocalPlayer.tag = "oldTraveling";
                }
            }


            //MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            controller = gameObject.GetComponent<CharacterController>();
            Assert.IsNotNull(groundCheck);
            selfNumber = cmptPlayers();
            InitSelfNb(cmptPlayers());
            //teamManager();

            //Set Position
            /*controller.enabled = false;
            if (selfNumber % 2 == 1)
                transform.position = GameObject.FindGameObjectWithTag("BlueFieldSpawner").transform.position;

            else
                transform.position = GameObject.FindGameObjectWithTag("RedFieldSpawner").transform.position;
            controller.enabled = true;
            */


        }
        else
        {
            //gameObject.layer = 9;
            selfNumber = cmptPlayers();
            //InitSelfNb(cmptPlayers());

            isLocal = false;
            Transform[] Children = GetComponentsInChildren<Transform>();
            foreach (Transform child in Children)
            {
                //Destroy(child.gameObject.GetComponent <GameInfos>());
                //Destroy(child.gameObject.GetComponent<Health>());
                if (child.CompareTag("Untagged") || child.CompareTag("endCanvas"))
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
        if (GetComponent<GameInfos>().selfColor == 0)
            return;


        Jump();

        if (cam == null)
        {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("oldTraveling");
            foreach (GameObject child in characters)
            {
                Destroy(child);
            }
            cam = GameObject.FindGameObjectWithTag("MainCamera");
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.CompareTag("CameraTop"))
                {
                    Transform topCam = child;
                    LocalPlayer = GameObject.FindGameObjectWithTag("traveling");
                    LocalPlayer.transform.parent = topCam;
                    LocalPlayer.transform.localPosition = new Vector3();
                    LocalPlayer.tag = "oldTraveling";
                }
            }
            GameObject.FindGameObjectWithTag("cinemachineCamera").GetComponent<CinemachineFreeLook>().enabled = true;

        }
        Vector3 posit = cam.transform.position;
        Vector3 forwa = cam.transform.TransformDirection(Vector3.forward);
        //int layerMask = 1 << 8;

        //RaycastHit hit;
        //Physics.Raycast(posit, forwa, out hit, Mathf.Infinity, layerMask);
        //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);


        if (Input.GetMouseButtonDown(1) && InGame && !dead)
        {

            if (GotBall)
            {
                GotBall = false;
                Vector3 position = cam.transform.position;
                Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
                BallFire(PlayerID, position, forward);
            }
        }

        if (Input.GetMouseButtonDown(0) && InGame && !dead)
        {
            Vector3 position = cam.transform.position;
            Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
            CmdFireVFX(PlayerID, position);
            CmdFire(position, forward);

        }

        /*if (Input.GetMouseButtonDown(1) && GetComponent<GameInfos>().teamsReady && dead)
        {
            UpdateDeadCam();
        }

        if (Input.GetMouseButtonDown(0) && GetComponent<GameInfos>().teamsReady && dead)
        {
            UpdateDeadCam();

        }*/


        transform.rotation = new Quaternion(cam.transform.localRotation.x, cam.transform.localRotation.y, cam.transform.localRotation.z, cam.transform.localRotation.w);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        //gun.transform.rotation = new Quaternion(cam.transform.localRotation.x, cam.transform.localRotation.y, cam.transform.localRotation.z, cam.transform.localRotation.w);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
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
        if (cmptPlayers() != playerNumber && isLocalPlayer)
        {
            var ZL = GetComponent<ZoneLimitations>();
            //teamManager(ZL.teamBlue);
            playerNumber = cmptPlayers();

            //var GI = GetComponent<GameInfos>();
            //GI.CmdUpDateName(GI.selfName, PlayerID);
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
            var GI = GetComponent<GameInfos>().enabled = false; //toggle this script to re-invoke it
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jumping : y = √(h * -2 * g)
        }

        velocity.y += gravity * Time.deltaTime; // falling : ∆y = 1/2g * t^2 

        controller.Move(velocity * Time.deltaTime);


    }




    #region Unity FireVFXBall
    [Command] //Appelé par le client mais lu par le serveur
    void CmdFireVFX(int nb, Vector3 position)
    {
        ClientFireVFX(nb, position);
    }

    [ClientRpc]
    void ClientFireVFX(int nb, Vector3 position)
    {
        //Transform camInfo = CamInfo;
        var bullet = (GameObject)Instantiate(
            bulletVFXPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);


        //NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients
        bullet.GetComponent<FireVFX>().player = nb;

        Destroy(bullet, 0.1f);
    }



    #endregion

    void BallFire(int nb, Vector3 position, Vector3 forward)
    {
        //int layerMask = 1 << 11;
        int grnd = 1 << LayerMask.NameToLayer("Ground");
        int plyr = 1 << LayerMask.NameToLayer("Player");
        int mask = grnd | plyr;
        RaycastHit hit;
        Vector3 dir;
        if (Physics.Raycast(position, forward, out hit, Mathf.Infinity, mask))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log(hit.point);
            dir = hit.point - bulletSpawn.position;
            dir = dir.normalized;
            //bullet.GetComponent<Rigidbody>().AddForce(dir * 15000);
        }
        else
        {
            Debug.Log("WRONNG");
            dir = Vector3.zero;
            //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;
        }

        CmdBallFire(nb, dir);
    }


    #region Unity BallKill
    [Command] //Appelé par le client mais lu par le serveur
    void CmdBallFire(int nb, Vector3 dir)
    {
        var bullet = (GameObject)Instantiate(
        bulletPrefab,
        bulletSpawn.position,
        bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody>().AddForce(dir * 13000);

        NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        bullet.GetComponent<Bullet>().player = nb;
    }

    //[ClientRpc]
    /*void ClientBallFire(int nb, Vector3 position, Vector3 forward)
    {
        //Transform camInfo = CamInfo;
        

        NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        bullet.GetComponent<Bullet>().player = nb;
        //Destroy(bullet, 10.0f);
    }*/



    #endregion



    #region Unity Shoot
    [Command]
    void CmdFire(Vector3 position, Vector3 forward)
    {
        //int layerMask = 1 << 11;
        int grnd = 1 << LayerMask.NameToLayer("StartWall");
        int plyr = 1 << LayerMask.NameToLayer("Player");
        int mask = grnd | plyr;
        RaycastHit hit;

        if (Physics.Raycast(position, forward, out hit, Mathf.Infinity, mask))
        {
            //Vector3 dir = hit.point - bullet.transform.position;
            //dir = dir.normalized;
            //bullet.GetComponent<Rigidbody>().AddForce(dir * 10000);
            if (hit.transform.gameObject.GetComponent<FullControl>() != null)
            {
                int playerTouched = hit.transform.gameObject.GetComponent<FullControl>().PlayerID;
                int shooter = PlayerID;

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

                if (child.GetComponent<FullControl>().PlayerID == playerTouched)
                {

                    bool kill = child.GetComponent<Health>().TakeDamage(20);

                    if (kill) //Si y a kill le joueur redescend
                    {


                        if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill) //Si permier kill, on donne la balle et on bloque  
                            child.GetComponent<Health>().KillManager(shooter, playerTouched, true);
                        else
                            child.GetComponent<Health>().KillManager(shooter, playerTouched, false);

                        child.GetComponent<ZoneLimitations>().UpState();
                        //child.GetComponent<ZoneLimitations>().UpdateZone();

                    }
                }

            }

        }
    }
    #endregion



    #region Unity FirstKill
    //NON UTILISE
    [Command]
    void CmdFirstKill()
    {
        ClientFirstKill();
    }

    [ClientRpc]
    void ClientFirstKill()
    {
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill = true;
    }

    #endregion

    public void TeamChoice(bool teamBlue)
    {
        isBlue = teamBlue;
    }

    #region Unity TeamManager
    public void TeamManager()
    {
        /*var ZL = GetComponent<ZoneLimitations>();

        if (isBlue)
        {
            ZL.teamBlue = true; //Edit en direct ou le joueur prend des degats psq trop long
            CmdChangeTeam(true);

        }

        else
        {
            ZL.teamBlue = false; //Edit en direct ou le joueur prend des degats psq trop long
            CmdChangeTeam(false);
        }*/

        if (isServer)
        {
            CmdTeamManager();
        }

    }


    [Command]
    public void CmdTeamManager()
    {
        if (isServer)
            ClientTeamManager();
    }

    [ClientRpc]
    public void ClientTeamManager()
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {

            if (child.GetComponent<FullControl>().isLocal)
            {
                GameObject.FindGameObjectWithTag("startImage").GetComponent<Image>().enabled = false;

                var ZL = child.GetComponent<ZoneLimitations>();

                child.GetComponent<FullControl>().controller.enabled = false;

                if (child.GetComponent<FullControl>().isBlue)
                    ZL.UpdateZone();
                //child.transform.position = GameObject.FindGameObjectWithTag("BlueFieldSpawner").transform.position;
                else
                    ZL.UpdateZone();

                //child.transform.position = GameObject.FindGameObjectWithTag("RedFieldSpawner").transform.position;

                child.GetComponent<FullControl>().controller.enabled = true;
            }

        }

    }
    #endregion



    //Edit sur le server pour synchroniser, besoin pour color manager

    [Command]
    public void CmdChangeTeam(bool blueTeam)
    {
        if (!isServer)
            return;
        var ZL = GetComponent<ZoneLimitations>();

        if (blueTeam)
        {
            ZL.teamBlue = true; //Edit en direct ou le joueur prend des degats psq trop long

        }

        else
        {
            ZL.teamBlue = false; //Edit en direct ou le joueur prend des degats psq trop long
        }
    }

    public void ColorManager()
    {


        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            var ZL = child.GetComponent<ZoneLimitations>();
            if (ZL.teamBlue)
            {
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

    #region PickUp
    [Command]
    public void CmdPickUp(int plyr)
    {
        ClientPickUp(plyr);
    }

    [ClientRpc]
    void ClientPickUp(int plyr)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == plyr)
            {
                child.GetComponent<FullControl>().GotBall = true;
            }
        }
        GameObject ball = GameObject.FindGameObjectWithTag("Bullet"); // destroy ball here to late time gotball process
        Destroy(ball);
    }

    void OnChangeGotBall(bool oldValue, bool newValue)
    {
        GotBall = newValue;
    }
    #endregion

    //Canvas team 
    public void MouseLock(bool Lock)
    {
        if (!Lock)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

    }


    #region StartGame
    [Command]
    public void CmdStartTimer()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            child.GetComponent<FullControl>().OnLobby = false;
        }
        ClientStartTimer();
    }

    [ClientRpc]
    void ClientStartTimer()
    {
        Instantiate(StartCanvas, new Vector3(0, 0, 0), Quaternion.identity);

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            var FCScript = child.GetComponent<FullControl>();
            if (FCScript.isLocal)
            {
                child.GetComponent<ZoneLimitations>().UpState();
                child.GetComponent<FullControl>().InGame = true;
            }
        }
        /*GameObject timer;
        if (timer = GameObject.FindGameObjectWithTag("TimerText"))
        {
            timer.GetComponent<StartTimer>().top = true;
            timer.GetComponent<Text>().enabled = true;
            timer.GetComponent<StartTimer>().enabled = true;
        }*/
    }
    #endregion

    public void UpdateDeadCam()
    {
        /*GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (!child.GetComponent<FullControl>().isLocal)
            {
                Transform[] children = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in children)
                {
                    if (child2.CompareTag("CameraTop"))
                    {
                        GameObject LocalPlayer;
                        Transform topCam = child2;
                        LocalPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
                        LocalPlayer.transform.parent = topCam;
                        LocalPlayer.transform.localPosition = new Vector3();
                        //transform.gameObject.SetActive(false);
                    }
                }
            }
        }*/
    }

    void OnDeadChange(bool oldValue, bool newValue)
    {
        dead = newValue;
    }

    [Command]
    public void CmdDisplayPlayer(int player, bool isdead)
    {
        ClientDisplayPlayer(player, isdead);
    }

    [ClientRpc]
    public void ClientDisplayPlayer(int player, bool display)
    {
        bool creatPlayer = true;

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        if (!display)
        {
            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().PlayerID == player)
                {
                    child.GetComponent<FullControl>().dead = true;
                    Transform[] children = GetComponentsInChildren<Transform>();
                    foreach (Transform child2 in children)
                    {
                        if (child2.CompareTag("LocalPlayer"))
                            Destroy(child2.gameObject);
                    }
                }
            }
        }

        else
        {
            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().PlayerID == player)
                {
                    //child.GetComponent<FullControl>().dead = false;
                    Transform[] children = GetComponentsInChildren<Transform>();
                    foreach (Transform child2 in children)
                    {
                        if (child2.CompareTag("LocalPlayer"))
                        {
                            creatPlayer = false;
                        }
                    }
                    if (creatPlayer)
                    {
                        child.GetComponent<FullControl>().InitPlayerBody();
                    }
                }
            }
        }

        //ClientDesablePlayer(player);
        GameEnd();
    }

    [Command]
    public void CmdDeadPlayer(int player)
    {
        ClientDeadPlayer(player);
    }

    [ClientRpc]
    public void ClientDeadPlayer(int player)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == PlayerID)
                child.GetComponent<FullControl>().dead = true;
        }
        GameEnd();
    }

    void GameEnd()
    {
        int blueDead = 0;
        int redDead = 0;
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            var FC = child.GetComponent<FullControl>();
            var ZL = child.GetComponent<ZoneLimitations>();

            if (ZL.teamBlue && FC.dead)
            {
                blueDead++;
            }
            if (!ZL.teamBlue && FC.dead)
            {
                redDead++;
            }


        }

        int teamSize = GetComponent<GameInfos>().teamSize;

        characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                Transform[] children = child.GetComponentsInChildren<Transform>();
                foreach (Transform child2 in children)
                {

                    if (child2.CompareTag("endCanvas"))
                    {
                        if (blueDead >= teamSize)
                        {
                            if (child.GetComponent<ZoneLimitations>().teamBlue)
                                child2.GetComponent<endGame>().displayEndGame(false);
                            else
                                child2.GetComponent<endGame>().displayEndGame(true);

                        }

                        if (redDead >= teamSize)
                        {
                            if (!child.GetComponent<ZoneLimitations>().teamBlue)
                                child2.GetComponent<endGame>().displayEndGame(false);
                            else
                                child2.GetComponent<endGame>().displayEndGame(true);
                        }
                    }
                }
            }
        }
    }

    public void InitPlayerBody()
    {
        var body = Instantiate(BodyPrefab, gameObject.transform);
        body.transform.parent = gameObject.transform;
        GetComponent<AnimationStateControler>().player = body;
        GetComponent<AnimationStateControler>().animator = GetComponent<AnimationStateControler>().player.GetComponent<Animator>(); ;

        bulletSpawn = body.GetComponent<BulletSpawn>().BulletPose.transform;


    }


    [Command]
    public void CmdBackToLobby(int id)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == id)
            {
                child.GetComponent<FullControl>().OnLobby = true;
            }
        }

        characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (!child.GetComponent<FullControl>().OnLobby)
            {
                return;
            }
        }
        GameObject GameMng = GameObject.FindGameObjectWithTag("GameManager");
        GameMng.GetComponent<StartManager>().InteratableEnable();
    }
}