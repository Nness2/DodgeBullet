using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DodgeBullet;

public class SpellManager : NetworkBehaviour
{
    [SerializeField] private IntVariable _munition;

    public GameObject wallPrefab;
    public GameObject backWallPrefab;
    public GameObject CatchPrefab;


    private float wallCd;
    private bool wallReady;
    private bool bcWallReady;
    private bool catchWallReady;
    public bool reloadReady;
    int elapsedFrames = 0;


    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        wallCd = 5;
        wallReady = true;
        bcWallReady = true;
        catchWallReady = true;
        reloadReady = true;

    }

    // Update is called once per frame
    void Update()
    {

        //if (Reload)
        //    reloadArm();

        if (!isLocalPlayer)
            return;

        //WALLSPELL
        if (Input.GetKeyDown(KeyCode.E))
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
            }
        }

        //BACKWALLSPELL
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (bcWallReady)
            {
                bcWallReady = false;
                Vector3 rot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
                Vector3 playerPos = transform.position;
                Vector3 playerDirection = transform.forward;
                Quaternion playerRotation = transform.rotation;
                float spawnDistance = -0.8f;
                Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
                spawnPos = new Vector3(spawnPos.x, 1.3f, spawnPos.z);
                int player = GetComponent<FullControl>().PlayerID;

                CmdBackWall(spawnPos, Quaternion.Euler(rot), player);
            }
        }

        //CATCHWALLSPELL
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (catchWallReady)
            {
                catchWallReady = false;
                Vector3 rot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90, transform.eulerAngles.z);
                Vector3 playerPos = transform.position;
                Vector3 playerDirection = transform.forward;
                Quaternion playerRotation = transform.rotation;
                float spawnDistance = 0f;
                Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
                spawnPos = new Vector3(spawnPos.x, 0.7f, spawnPos.z);
                int player = GetComponent<FullControl>().PlayerID;

                CmdCatchWall(spawnPos, player);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && reloadReady && _munition.Value < 30)
        {
            //_munition.Value = 0;
            reloadReady = false;
            CmdReload(gameObject);

        }
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

        while (elapsedTime > time2 && elapsedTime < time3)
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
        }

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


        StartCoroutine(WallWait());


        Destroy(bullet, 5.0f);
    }

    IEnumerator WallWait()
    {
        yield return new WaitForSeconds(wallCd);
        wallReady = true;

    }

    
    [Command] //Appelé par le client mais lu par le serveur
    void CmdBackWall(Vector3 position, Quaternion rotation, int player)
    {
        ClientBackWall(position, rotation, player);
    }

    [ClientRpc]
    void ClientBackWall(Vector3 position, Quaternion rotation, int player)
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
                var bkWall = (GameObject)Instantiate(
                    backWallPrefab,
                    position,
                    rotation);
                //child.transform.position += new Vector3(0, 0, -0.5f);
                //child.transform.rotation);
                //bkWall.transform.eulerAngles += new Vector3(0, 90, 0);

                bkWall.transform.parent = child.transform;

                StartCoroutine(BcWallWait());

                Destroy(bkWall, 5.0f);
            }
        }
    }

    IEnumerator BcWallWait()
    {
        yield return new WaitForSeconds(wallCd);
        bcWallReady = true;

    }

    [Command] //Appelé par le client mais lu par le serveur
    void CmdCatchWall(Vector3 pos, int player)
    {
        ClientCatchWall(pos, player);
    }

    [ClientRpc]
    void ClientCatchWall(Vector3 pos, int player)
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
                var bkWall = (GameObject)Instantiate(
                    CatchPrefab,
                    pos,
                    Quaternion.identity);
                bkWall.GetComponent<Identifier>().Id = player;
                //child.transform.position += new Vector3(0, 0, -0.5f);
                //child.transform.rotation);
                //bkWall.transform.eulerAngles += new Vector3(0, 90, 0);

                bkWall.transform.parent = child.transform;

                StartCoroutine(BcCatchWallWait());

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
        yield return new WaitForSeconds(wallCd);
        catchWallReady = true;

    }
}
