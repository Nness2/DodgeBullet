using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Mirror;
using Cinemachine;
using DodgeBullet;


using UnityEngine.SceneManagement;

public class FullControl : NetworkBehaviour
{
    enum BallTypes : int { Bullet, Vellet, Twollet, RainBall, ShotBall, ExplosiveBall};
    public enum BallEffects : int { Kill, Heal, Stun, Slow, TwoKill };


    private int playerNumber;

    private GameObject LocalPlayer;

    public CharacterController controller;
    public GameObject cam;

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
    public GameObject velletPrefab;
    public GameObject twolletPrefab;
    public GameObject RainBalltPrefab;
    public GameObject ShotBalltPrefab;
    public GameObject ExplosiveBalltPrefab;

    public Transform bulletSpawn;

    public GameObject BodyPrefab;
    public GameObject StartCanvas;
    public GameObject TargetAnimPrefab;
    public GameObject TargetAnimPrefabLeft;
    public GameObject BulletImpact;

    public GameObject LeftHandPose;
    public GameObject StaticLeftHandPose;
    public GameObject LeftForeArmPose;
    public GameObject SpherePose;
    public GameObject PocketPose;

    public GameObject SelfBody;

    public GameObject gun;

    private GameObject GameMng;

    [SyncVar(hook = nameof(OnChangeNumber))]
    public int selfNumber;

    private NetworkManager nm;

    public bool isLocal;

    public int killNbr;

    public bool isBlue;

    [SyncVar(hook = nameof(OnDeadChange))]
    public bool dead;

    public int PlayerID;

    public bool Replay;


    public bool InGame;
    public bool OnLobby;

    [SerializeField] private IntVariable _munition;
    [SerializeField] private IntVariable _ball;

    public GameObject PreBulletPrefab;
    [SerializeField] private IntVariable _ballPower;

    public bool isFalling;

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
        isGrounded = true;


        if (isLocalPlayer)
        {
            //DontDestroyOnLoad(transform.gameObject);
            InGame = false;

            Replay = false;
            dead = false;
            GameObject.FindGameObjectWithTag("cinemachineCamera").GetComponent<CinemachineFreeLook>().enabled = true;

            GameMng = GameObject.FindGameObjectWithTag("GameManager");
            GetComponent<GameInfos>().addGetNames();


            isLocal = true;
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {

                if (child.CompareTag("CameraTop"))
                {
                    Transform topCam = child;
                    LocalPlayer = GameObject.FindGameObjectWithTag("traveling");
                    LocalPlayer.transform.parent = topCam;
                    LocalPlayer.transform.localPosition = new Vector3(0.2f,0,0);
                    LocalPlayer.tag = "oldTraveling";
                }


            }

            //CmdTargetAnim(PlayerID);

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

            SpherePose = GameObject.FindGameObjectWithTag("SphereTarget");

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
        Transform[] PoseChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in PoseChildren)
        {
            if (child.CompareTag("LeftHandPose"))
            {
                LeftHandPose = child.gameObject;
            }
            if (child.CompareTag("StaticLeftHandPose"))
            {
                StaticLeftHandPose = child.gameObject;
            }
            if (child.CompareTag("LeftForeArmPose"))
            {
                LeftForeArmPose = child.gameObject;
            }
            if (child.CompareTag("PocketPose"))
            {
                PocketPose = child.gameObject;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //UpdateTargetAnimLeft(LeftHandPose.transform.position, LeftHandPose.transform.rotation ,PlayerID);

        if (!isLocalPlayer)
            return;
        if (GetComponent<GameInfos>().selfColor == 0)
            return;

        //CmdUpdateTargetAnim(SpherePose.transform.position, SpherePose.transform.rotation, PlayerID);


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


        /*
        if (Input.GetMouseButtonUp(1))// && InGame && !dead)
        {
            if (GotBall)
            {
                GameObject[] PreBullets = GameObject.FindGameObjectsWithTag("PreBullet");
                foreach(GameObject child in PreBullets)
                {
                    Destroy(child);
                }

                _ball.Value = 0;
                GotBall = false;
                Vector3 position = cam.transform.position;
                Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
                BallFire(PlayerID, position, forward, lob, (int)BallTypes.Bullet);
            }
        }

        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))// && InGame && !dead)
        {
            if (GotVellet)
            {
                GameObject[] PreBullets = GameObject.FindGameObjectsWithTag("PreBullet");
                foreach (GameObject child in PreBullets)
                {
                    Destroy(child);
                }

                _ball.Value = 0;
                //GotBall = false;
                GotVellet = false;
                Vector3 position = cam.transform.position;
                Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
                BallFire(PlayerID, position, forward, lob, (int)BallTypes.Vellet);
            }
        }*/


        /*if (Input.GetKeyDown(KeyCode.F))
        {
            if (GotTwollet)
            {
                //GotBall = false;
                GotVellet = true;
                Vector3 position = cam.transform.position;
                Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
                BallFire(PlayerID, position, forward, lob, (int)BallTypes.Twollet);
            }
        }*/


        /*if (Input.GetKeyDown(KeyCode.G))
        {
            if (GotRainBall)
            {
                //GotBall = false;
                GotRainBall = false;
                Vector3 position = cam.transform.position;
                Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
                BallFire(PlayerID, position, forward, lob, (int)BallTypes.RainBall);
            }


        }*/


        /*bool reloadReady = GetComponent<SpellManager>().reloadReady;
        if (Input.GetMouseButtonDown(0) && _munition.Value > 0 && !dead && reloadReady && InGame && !Input.GetMouseButton(1))
        {
            Vector3 position = cam.transform.position;
            Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
            CmdFireVFX(PlayerID, position);
            CmdFire(position, forward);
            _munition.Value--;
        }*/

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


    public static GameObject FindParentWithTag(GameObject childObject, string tag) // Traverse up the hierarchy to find first parent with specific tag
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
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

        if (isServer)
        {


            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1000);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                // do whathever you need here to determine if an object is a coin
                // Here I assume that all the coins will be tagged as coin
                if (hitColliders[i].tag == "Bullet")
                {
                    Transform coin = hitColliders[i].transform;
                    float distance = Vector3.Distance(hitColliders[i].transform.position, transform.position);
                    if (distance > 2 && hitColliders[i].GetComponent<Identifier>().Id == PlayerID)
                    {
                        coin.position = Vector3.MoveTowards(coin.position, transform.position, 2 * Time.deltaTime);

                    }
                }
            }
        }
    }



    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !GetComponent<AnimationStateControler>().isLaunchingIdle)
        {
            //var GI = GetComponent<GameInfos>().enabled = false; //toggle this script to re-invoke it // Bug Je ne sais pas ce que cette ligne faisait là
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

    /*void BallPreFire(Vector3 position, Vector3 forward, bool lob)
    {
        //int layerMask = 1 << 11;
        int grnd = 1 << LayerMask.NameToLayer("Ground");
        int plyr = 1 << LayerMask.NameToLayer("Player");

        int SecondLayer = 1 << LayerMask.NameToLayer("SecondLayer");
        int mask3 = SecondLayer;
        RaycastHit hit3;

        Vector3 SecondPose = Vector3.zero;
        if (Physics.Raycast(position, forward, out hit3, Mathf.Infinity, mask3))
            SecondPose = hit3.point;
        else
            return;

        int mask = grnd | plyr;
        RaycastHit hit;
        Vector3 dir;
        if (Physics.Raycast(SecondPose, forward, out hit, Mathf.Infinity, mask))
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

        if (!lob)
        {
            float v = (13000 * Time.fixedDeltaTime / 5);
            VisualizeSegment(dir * v);
        }
        else
        {
            float v = (2000 * Time.fixedDeltaTime / 5);
            VisualizeSegment((dir + new Vector3(0, 1.1f, 0).normalized) * v);
        }

    }*/


    public void BallFire(int nb, Vector3 position, Vector3 forward, bool lobb, int BallType)
    {
        //int layerMask = 1 << 11;
        int grnd = 1 << LayerMask.NameToLayer("Ground");
        int plyr = 1 << LayerMask.NameToLayer("Player");

        int SecondLayer = 1 << LayerMask.NameToLayer("SecondLayer");
        int mask3 = SecondLayer;
        RaycastHit hit3;

        Vector3 SecondPose = Vector3.zero;
        if (Physics.Raycast(position, forward, out hit3, Mathf.Infinity, mask3))
            SecondPose = hit3.point;
        else
            return;

        int mask = grnd | plyr;
        RaycastHit hit;
        Vector3 dir;
        if (Physics.Raycast(SecondPose, forward, out hit, Mathf.Infinity, mask))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log(hit.point);
            dir = hit.point - bulletSpawn.transform.position;
            dir = dir.normalized;
            //bullet.GetComponent<Rigidbody>().AddForce(dir * 15000);
        }
        else
        {
            Debug.Log("WRONNG");
            dir = Vector3.zero;
            //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;
        }

        CmdBallFire(nb, dir, lobb, BallType);
    }


    #region Unity BallKill
    [Command] //Appelé par le client mais lu par le serveur
    void CmdBallFire(int nb, Vector3 dir, bool lobb, int ballType)
    {
        GameObject bullet = null;

        if (ballType == (int)BallTypes.Bullet)
        {
            bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

            if (bullet == null)
            {
                return;
            }

            if (!lobb)
                bullet.GetComponent<Rigidbody>().AddForce(dir * (13000));//p + (_ballPower.Value) * 200));
            else
                bullet.GetComponent<Rigidbody>().AddForce((dir + new Vector3(0, 1.1f, 0).normalized) * (2000));//p + (_ballPower.Value) * 200));

            bullet.GetComponent<Bullet>().player = nb;
            bullet.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
            bullet.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
            bullet.GetComponent<Bullet>().BulletType = 0;


            NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        }

        else if (ballType == (int)BallTypes.Vellet)
        {
            bullet = (GameObject)Instantiate(
            velletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

            bullet.GetComponent<Rigidbody>().AddForce(dir * (13000));
            bullet.GetComponent<Vellet>().player = nb;
            bullet.GetComponent<Vellet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
            bullet.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;


            NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        }


        else if (ballType == (int)BallTypes.RainBall)
        {
            bullet = (GameObject)Instantiate(
            RainBalltPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

            bullet.GetComponent<Rigidbody>().AddForce((dir + new Vector3(0, 1.1f, 0).normalized) * (2000));//p + (_ballPower.Value) * 200));
            //bullet.GetComponent<Rigidbody>().AddForce(dir * (13000));
            bullet.GetComponent<RainBall>().player = nb;
            bullet.GetComponent<RainBall>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
            bullet.GetComponent<RainBall>().BallEffect = (int)BallEffects.Kill;


            NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        }

        else if (ballType == (int)BallTypes.ShotBall)
        {
            bullet = (GameObject)Instantiate(
            ShotBalltPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);


            bullet.GetComponent<Rigidbody>().AddForce(dir * (13000));

            bullet.GetComponent<ShotBall>().player = nb;
            bullet.GetComponent<ShotBall>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
            bullet.GetComponent<ShotBall>().BallEffect = (int)BallEffects.Kill;
            bullet.GetComponent<ShotBall>().InitialDir = dir;

            NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        }

        else if (ballType == (int)BallTypes.ExplosiveBall)
        {
            bullet = (GameObject)Instantiate(
            ExplosiveBalltPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);


            bullet.GetComponent<Rigidbody>().AddForce(dir * (13000));

            bullet.GetComponent<ExplosiveBall>().player = nb;
            bullet.GetComponent<ExplosiveBall>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
            bullet.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;

            NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        }

        else if (ballType == (int)BallTypes.Twollet)
        {
            bullet = (GameObject)Instantiate(
            twolletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

            if (bullet == null)
            {
                return;
            }

            if (!lobb)
                bullet.GetComponent<Rigidbody>().AddForce(dir * (13000));//p + (_ballPower.Value) * 200));
            else
                bullet.GetComponent<Rigidbody>().AddForce((dir + new Vector3(0, 1.1f, 0).normalized) * (2000));//p + (_ballPower.Value) * 200));

            bullet.GetComponent<Twollet>().player = nb;
            bullet.GetComponent<Twollet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;

            NetworkServer.Spawn(bullet); //Spawn sur le serveur et les clients

        }

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

    #region Unity TargetAnim
    [Command] //Appelé par le client mais lu par le serveur
    void CmdTargetAnim(int playerId)
    {
        var TargetAnim = (GameObject)Instantiate(
        TargetAnimPrefab,
        Vector3.zero,
        Quaternion.identity);

        var TargetAnimLeft = (GameObject)Instantiate(
        TargetAnimPrefabLeft,
        Vector3.zero,
        Quaternion.identity);


        NetworkServer.Spawn(TargetAnim); //Spawn sur le serveur et les clients
        NetworkServer.Spawn(TargetAnimLeft); //Spawn sur le serveur et les clients

        //TargetAnim.transform.parent = transform;
        TargetAnim.GetComponent<TargetAnimator>().PlayerId = playerId;
        TargetAnimLeft.GetComponent<TargetAnimator>().PlayerId = playerId;
        ClientTargetAnimToParent(TargetAnimLeft, playerId);
        ClientTargetAnimToParent(TargetAnim, playerId);
    }

    [ClientRpc]
    void ClientTargetAnimToParent(GameObject TargetAnim, int id)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == id)
            {
                TargetAnim.transform.parent = child.transform;
            }
        }
    }


    void UpdateTargetAnimLeft(Vector3 leftHandPose, Quaternion leftHandRot, int playerId)
    {

        GameObject[] targetLeft = GameObject.FindGameObjectsWithTag("TargetAnimatorLeft");

        foreach (GameObject child in targetLeft)
        {
            var TA = child.GetComponent<TargetAnimator>();
            if (TA.PlayerId == playerId)
            {
                TA.transform.position = leftHandPose;
                TA.transform.rotation = leftHandRot;

            }
        }
    }

    [Command]
    void CmdUpdateTargetAnim(Vector3 spherePos, Quaternion sphereRot, int playerId)
    {
        GameObject[] target = GameObject.FindGameObjectsWithTag("TargetAnimator");

        foreach (GameObject child in target)
        {
            var TA = child.GetComponent<TargetAnimator>();
            if (TA.PlayerId == playerId)
            {
                TA.transform.position = spherePos;
                TA.transform.rotation = sphereRot;
            }
        }
    }

    #endregion

    /*#region Unity Shoot
    [Command]
    void CmdFire(Vector3 position, Vector3 forward)
    {
        //int layerMask = 1 << 11;
        int grnd = 1 << LayerMask.NameToLayer("StartWall");
        int plyr = 1 << LayerMask.NameToLayer("Player");
        int mask = grnd | plyr;

        int ground = 1 << LayerMask.NameToLayer("Ground");
        int mask2 = ground;

        int SecondLayer = 1 << LayerMask.NameToLayer("SecondLayer");
        int mask3 = SecondLayer;

        RaycastHit hit;
        RaycastHit hit2;
        RaycastHit hit3;

        Vector3 SecondPose = Vector3.zero;
        if (Physics.Raycast(position, forward, out hit3, Mathf.Infinity, mask3))
        {
            SecondPose = hit3.point;
        }

        else
            return;

        if (Physics.Raycast(SecondPose, forward, out hit, Mathf.Infinity, mask))
        {
            //Vector3 dir = hit.point - bullet.transform.position;
            //dir = dir.normalized;
            //bullet.GetComponent<Rigidbody>().AddForce(dir * 10000);
            if (hit.transform.gameObject.GetComponent<FullControl>() != null)
            {
                int playerTouched = hit.transform.gameObject.GetComponent<FullControl>().PlayerID;
                int shooter = PlayerID;
                if (GetComponent<ZoneLimitations>().teamBlue != hit.transform.gameObject.GetComponent<ZoneLimitations>().teamBlue) // S'ils ne sont pas de la même équipe
                    ClientFire(shooter, playerTouched);
            }
        }

        else if (Physics.Raycast(SecondPose, forward, out hit2, Mathf.Infinity, mask2))
        {

            if (hit2.transform.tag != "SphereTarget")
            {
                Vector3 touchPoint = hit2.point;
                Quaternion lookRotation = Quaternion.LookRotation(-hit2.normal);

                ClientFireImpact(touchPoint, lookRotation);//, hit2.transform);
            }
        }
    }

    [ClientRpc]
    void ClientFireImpact(Vector3 touchPoint, Quaternion lookRotation)//, Transform objTouched)
    {

        GameObject impact = Instantiate(BulletImpact, touchPoint + new Vector3(0.02f, 0.02f, 0.02f), lookRotation);
        //impact.transform.parent = objTouched;
        Destroy(impact, 0.1f);
        
    }

    [ClientRpc]
    void ClientFire(int shooter, int playerTouched)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                if (child.GetComponent<FullControl>().PlayerID == shooter)
                {
                    GetComponent<Health>().DisplayDamageDealed(20, playerTouched);
                }
                if (child.GetComponent<FullControl>().PlayerID == playerTouched)
                {
                    int damage = 20;
                    bool kill = child.GetComponent<Health>().TakeDamage(damage);

                    if (kill) //Si y a kill le joueur redescend
                    {


                        if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill) //Si permier kill, on donne la balle et on bloque  
                            child.GetComponent<Health>().KillManager(shooter, playerTouched, true, false);
                        else
                            child.GetComponent<Health>().KillManager(shooter, playerTouched, false, false);

                        if (!child.GetComponent<FullControl>().dead)
                            child.GetComponent<ZoneLimitations>().UpState();
                        //child.GetComponent<ZoneLimitations>().UpdateZone();

                    }
                }

            }

        }
    }
    #endregion
    */


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
        /*if (teamBlue)
            gameObject.layer = LayerMask.NameToLayer("BluePlayer");
        if (!teamBlue)
            gameObject.layer = LayerMask.NameToLayer("RedPlayer"); */
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

                //child.GetComponent<FullControl>().controller.enabled = false;
                //ZL.InitState();
                //if (child.GetComponent<FullControl>().isBlue)
                //
                //child.transform.position = GameObject.FindGameObjectWithTag("BlueFieldSpawner").transform.position;
                //else
                //    ZL.UpdateZone();

                //child.transform.position = GameObject.FindGameObjectWithTag("RedFieldSpawner").transform.position;

                //child.GetComponent<FullControl>().controller.enabled = true;
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
                child.GetComponent<GameInfos>().blueButton.interactable = false;
                child.GetComponent<GameInfos>().redButton.interactable = false;
                child.GetComponent<GameInfos>().spectatButton.interactable = false;

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

    /*[Command]
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
    }*/

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

    public void GameEnd()
    {

        int blueAlive = 0;
        int redAlive = 0;
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            var FC = child.GetComponent<FullControl>();
            var ZL = child.GetComponent<ZoneLimitations>();

            if (ZL.teamBlue && !FC.dead)
            {
                blueAlive++;
            }
            if (!ZL.teamBlue && !FC.dead)
            {
                redAlive++;
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
                        if (blueAlive == 0)
                        {
                            if (child.GetComponent<ZoneLimitations>().teamBlue)
                                child2.GetComponent<endGame>().displayEndGame(false);
                            else
                                child2.GetComponent<endGame>().displayEndGame(true);

                        }

                        else if (redAlive == 0)
                        {
                            if (child.GetComponent<ZoneLimitations>().teamBlue)
                                child2.GetComponent<endGame>().displayEndGame(true);
                            else
                                child2.GetComponent<endGame>().displayEndGame(false);
                        }
                    }
                }
            }
        }
    }

    public void InitPlayerBody()
    {
        SelfBody = Instantiate(BodyPrefab, gameObject.transform);
        SelfBody.transform.parent = gameObject.transform;
        GetComponent<AnimationStateControler>().player = SelfBody;
        GetComponent<AnimationStateControler>().animator = GetComponent<AnimationStateControler>().player.GetComponent<Animator>(); ;

        bulletSpawn = SelfBody.GetComponent<BulletSpawn>().BulletPose.transform;
        //body.GetComponent<GetRigInfo>().PlayerID = playerNumber;

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

    public void DeleteBalls()
    {
        if (isServer)
        {
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            foreach (GameObject child in bullets)
            {
                NetworkServer.Destroy(child);
            }
        }

    }




}