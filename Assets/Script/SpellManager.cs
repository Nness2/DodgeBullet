using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DodgeBullet;

public class SpellManager : NetworkBehaviour
{
    [SerializeField] private IntVariable _munition;
    [SerializeField] private IntVariable _upZone;
    [SerializeField] private IntVariable _downZone;


    [SerializeField] private StringVariable _wallCd;
    [SerializeField] private StringVariable _backWallCd;
    [SerializeField] private StringVariable _CatchCd;
    [SerializeField] private StringVariable _UpZoneCd;
    [SerializeField] private StringVariable _DownZoneCd;


    public GameObject wallPrefab;
    public GameObject backWallPrefab;
    public GameObject CatchPrefab;


    private float WallCdTime;
    private float BackwallCdTime;
    private float CatchCdTime;
    private float UpZoneTime;
    private float DownZoneTime;

    private float WallCdTimeLeft;
    private float BackwallCdTimeLeft;
    private float CatchCdTimeLeft;
    private float UpZoneTimeLeft;
    private float DownZoneTimeLeft;

    private bool wallReady;
    private bool bcWallReady;
    private bool catchWallReady;
    public bool reloadReady;
    public bool UpZoneReady;
    public bool DownZoneReady;
    int elapsedFrames = 0;


    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        WallCdTime = 10;
        BackwallCdTime = 10;
        CatchCdTime = 5;
        UpZoneTime = 5;
        DownZoneTime = 2;
        wallReady = true;
        bcWallReady = true;
        catchWallReady = true;
        reloadReady = true;
        UpZoneReady = true;
        DownZoneReady = true;

    }

    // Update is called once per frame
    void Update()
    {

        //if (Reload)
        //    reloadArm();

        if (!isLocalPlayer)
            return;

        //WALLSPELL
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (wallReady)
            {
                wallReady = false;
                Vector3 rot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
                Vector3 playerPos = transform.position;
                Vector3 playerDirection = transform.forward;
                Quaternion playerRotation = transform.rotation;
                float spawnDistance = 2;
                Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
                spawnPos = new Vector3(spawnPos.x, 1.3f, spawnPos.z);

                CmdWall(spawnPos, Quaternion.Euler(rot));
                StartCoroutine(WallWait());
            }
        }

        //BACKWALLSPELL
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (bcWallReady)
            {
                bcWallReady = false;

                int player = GetComponent<FullControl>().PlayerID;

                CmdBackWall(player);
                StartCoroutine(BcWallWait());
            }
        }

        //CATCHWALLSPELL
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (catchWallReady)
            {
                catchWallReady = false;

                int player = GetComponent<FullControl>().PlayerID;
                CmdCatchWall(player);
                StartCoroutine(BcCatchWallWait());
            }
        }


        //UpZoneSPELL
        if (Input.GetKeyDown(KeyCode.Q) && _upZone.Value > 0 && GetComponent<ZoneLimitations>().state > 1 && GetComponent<FullControl>().InGame)
        {
            if (UpZoneReady)
            {
                UpZoneReady = false;

                GetComponent<ZoneLimitations>().CmdDownState();
                StartCoroutine(UpZoneWait());
                _upZone.Value--;
            }
        }

        //DownZoneSPELL
        if (Input.GetKeyDown(KeyCode.E) && _downZone.Value > 0 && GetComponent<FullControl>().InGame)
        {
            if (DownZoneReady)
            {
                DownZoneReady = false;

                GetComponent<ZoneLimitations>().UpState();
                StartCoroutine(DownZoneWait());
                _downZone.Value--;
            }
        }
        /*if (Input.GetMouseButtonDown(0))
        {
            //_munition.Value = 0;
            reloadReady = false;
            CmdReload(gameObject);

        }

        if (Input.GetMouseButtonUp(0))
        {
            //_munition.Value = 0;
            //reloadReady = false;
            CmdReload(gameObject);

        }*/
    }

    [Command] //Appelé par le client mais lu par le serveur
    void CmdReload(GameObject player)
    {
        ClientReload(player);
    }



    [ClientRpc]
    void ClientReload(GameObject player)
    {

        var FCScript = player.GetComponent<FullControl>();

        Vector3 LeftHandPose = FCScript.LeftHandPose.transform.position;
        Vector3 StaticLeftHandPose = FCScript.StaticLeftHandPose.transform.position;
        Quaternion StaticLeftHandRot = FCScript.StaticLeftHandPose.transform.rotation;
        Vector3 LeftArmPose = FCScript.LeftForeArmPose.transform.position;
        Vector3 PocketPose = FCScript.PocketPose.transform.position;
        Quaternion LeftArmRot = FCScript.LeftForeArmPose.transform.rotation;
        Quaternion PocketRot = FCScript.PocketPose.transform.rotation;

        StartCoroutine(MoveToPosition(player, LeftHandPose, LeftArmPose, PocketPose, StaticLeftHandPose, 0.2f, 0.4f, 0.6f, 0.8f, LeftArmRot, PocketRot, StaticLeftHandRot));
    }

    private IEnumerator MoveToPosition(GameObject Obj, Vector3 LeftHandPose, Vector3 Pose1, Vector3 Pose2, Vector3 Pose3,float time, float time2, float time3, float time4, Quaternion Quat1, Quaternion Quat2, Quaternion Quat3)
    {

        var FCScript = Obj.GetComponent<FullControl>();
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            FCScript.LeftHandPose.transform.position = Vector3.Lerp(LeftHandPose, Pose1, (elapsedTime / time)) - (Pose3 - FCScript.StaticLeftHandPose.transform.position);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (elapsedTime > time && elapsedTime < time2)
        {
            FCScript.LeftHandPose.transform.position = Vector3.Lerp(Pose1, Pose2, (elapsedTime / time2)) - (Pose3 - FCScript.StaticLeftHandPose.transform.position);
            //FCScript.LeftHandPose.transform.rotation = Quaternion.Lerp(Quat1, Quat2, (elapsedTime / time2)) * Quaternion.Inverse((Quat3 * Quaternion.Inverse(FCScript.StaticLeftHandPose.transform.rotation)));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        /*while (elapsedTime > time2 && elapsedTime < time3)
        {
            FCScript.LeftHandPose.transform.position = Vector3.Lerp(Pose2, Pose1, (elapsedTime / time3)) - (Pose3 - FCScript.StaticLeftHandPose.transform.position);
            //FCScript.LeftHandPose.transform.rotation = Quaternion.Lerp(Quat2, Quat1, (elapsedTime / time3)) * Quaternion.Inverse((Quat3 * Quaternion.Inverse(FCScript.StaticLeftHandPose.transform.rotation)));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        while (elapsedTime > time3 && elapsedTime < time4)
        {
            FCScript.LeftHandPose.transform.position = Vector3.Lerp(Pose1, LeftHandPose, (elapsedTime / time4)) - (Pose3 - FCScript.StaticLeftHandPose.transform.position);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }

        if (isLocalPlayer)
        {
            reloadReady = true;
            _munition.Value = 30;
        }*/

    }

    /*void reloadArm()
    {
        int interpolationFramesCount = 300; // Number of frames to completely interpolate between the 2 positions
        int elapsedFrames = 0;
        var FCScript = GetComponent<FullControl>();
        Vector3 LeftHandPose = FCScript.LeftHandPose.transform.position;
        Vector3 LeftArmPose = FCScript.LeftForeArmPose.transform.position;
        Vector3 PocketPose = FCScript.PocketPose.transform.position;
        float interpolationRation = (float)elapsedFrames / interpolationFramesCount;

        Vector3 newPos = Vector3.Lerp(LeftHandPose, LeftArmPose, 100);
        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
        Debug.Log(newPos);
        Debug.Log(LeftHandPose);
        Debug.Log(LeftArmPose);


        FCScript.LeftHandPose.transform.position = newPos;
        Debug.Log("ok");
        if (LeftHandPose == LeftArmPose)
        {
            Reload = false;
            Debug.Log("End");
        }
    }*/


    [Command] //Appelé par le client mais lu par le serveur
    void CmdWall(Vector3 position, Quaternion rotation)
    {
        ClientWall(position, rotation);
    }



    [ClientRpc]
    void ClientWall(Vector3 position, Quaternion rotation)
    {
        //Transform camInfo = CamInfo;
        
        var bullet = (GameObject)Instantiate(
            wallPrefab,
            position,
            rotation);
        

        Destroy(bullet, 5.0f);
    }

    IEnumerator WallWait()
    {

        for (WallCdTimeLeft = WallCdTime; WallCdTimeLeft > 0; WallCdTimeLeft -= Time.deltaTime)
        {
            int value = (int)WallCdTimeLeft + 1;
            _wallCd.Value = value.ToString();
            yield return null;
        }
        _wallCd.Value = "";
        wallReady = true;

    }

    
    [Command] //Appelé par le client mais lu par le serveur
    void CmdBackWall(int player)
    {
        ClientBackWall(player);
    }

    [ClientRpc]
    void ClientBackWall(int player)
    {
        //Transform camInfo = CamInfo;
        /*var bkWall = (GameObject)Instantiate(
            backWallPrefab,
            Vector3.zero,
            new Quaternion(0,0,0,0));*/

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == player)// && ZLScript.state > 0)
            {

                Vector3 rot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
                Vector3 playerPos = transform.position;
                Vector3 playerDirection = transform.forward;
                Quaternion playerRotation = transform.rotation;
                float spawnDistance = -0.6f;
                Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
                spawnPos = new Vector3(spawnPos.x, spawnPos.y+0.8f, spawnPos.z);

                var bkWall = (GameObject)Instantiate(
                    backWallPrefab,
                    spawnPos,
                    Quaternion.Euler(rot));
                //child.transform.position += new Vector3(0, 0, -0.5f);
                //child.transform.rotation);
                //bkWall.transform.eulerAngles += new Vector3(0, 90, 0);

                bkWall.transform.parent = child.transform;



                Destroy(bkWall, 5.0f);
            }
        }
    }

    IEnumerator BcWallWait()
    {
        for (BackwallCdTimeLeft = BackwallCdTime; BackwallCdTimeLeft > 0; BackwallCdTimeLeft -= Time.deltaTime)
        {
            int value = (int)BackwallCdTimeLeft + 1;
            _backWallCd.Value = value.ToString();
            yield return null;
        }
        _backWallCd.Value = "";
        bcWallReady = true;

    }

    [Command] //Appelé par le client mais lu par le serveur
    void CmdCatchWall(int player)
    {
        ClientCatchWall(player);
    }

    [ClientRpc]
    void ClientCatchWall(int player)
    {
        //Transform camInfo = CamInfo;
        /*var bkWall = (GameObject)Instantiate(
            backWallPrefab,
            Vector3.zero,
            new Quaternion(0,0,0,0));*/

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == player)// && ZLScript.state > 0)
            {

                Vector3 rot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
                Vector3 playerPos = transform.position;
                Vector3 playerDirection = transform.forward;
                Quaternion playerRotation = transform.rotation;
                float spawnDistance = 0f;
                Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
                spawnPos = new Vector3(spawnPos.x, spawnPos.y + 0.75f, spawnPos.z);

                var bkWall = (GameObject)Instantiate(
                    CatchPrefab,
                    spawnPos,
                    Quaternion.identity);
                bkWall.GetComponent<Identifier>().Id = player;
                //child.transform.position += new Vector3(0, 0, -0.5f);
                //child.transform.rotation);
                //bkWall.transform.eulerAngles += new Vector3(0, 90, 0);

                bkWall.transform.parent = child.transform;


                if (!GetComponent<FullControl>().isLocal)
                {
                    bkWall.AddComponent<SphereCollider>();
                }
                Destroy(bkWall, 0.2f);
            }
        }
    }

    IEnumerator BcCatchWallWait()
    {

        for (CatchCdTimeLeft = CatchCdTime; CatchCdTimeLeft > 0; CatchCdTimeLeft -= Time.deltaTime)
        {
            int value = (int)CatchCdTimeLeft+1;
            _CatchCd.Value = value.ToString();
            yield return null;
        }
        _CatchCd.Value = "";
        catchWallReady = true;

    }

    IEnumerator UpZoneWait()
    {

        for (UpZoneTimeLeft = UpZoneTime; UpZoneTimeLeft > 0; UpZoneTimeLeft -= Time.deltaTime)
        {
            int value = (int)UpZoneTimeLeft + 1;
            _UpZoneCd.Value = value.ToString();
            yield return null;
        }
        _UpZoneCd.Value = "";
        UpZoneReady = true;

    }

    IEnumerator DownZoneWait()
    {

        for (DownZoneTimeLeft = DownZoneTime; DownZoneTimeLeft > 0; DownZoneTimeLeft -= Time.deltaTime)
        {
            int value = (int)DownZoneTimeLeft + 1;
            _DownZoneCd.Value = value.ToString();
            yield return null;
        }
        _DownZoneCd.Value = "";
        DownZoneReady = true;

    }

}
