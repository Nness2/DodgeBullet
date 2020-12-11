using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpellManager : NetworkBehaviour
{

    public GameObject wallPrefab;
    public GameObject backWallPrefab;


    private float wallCd;
    private bool wallReady;
    private bool bcWallReady;


    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {
        wallCd = 5;
        wallReady = true;
        bcWallReady = true;

    }

    // Update is called once per frame
    void Update()
    {
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
                int player = GetComponent<FullControl>().selfNumber;

                CmdBackWall(spawnPos, Quaternion.Euler(rot), player);
            }
        }
    }

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
            if (child.GetComponent<FullControl>().selfNumber == player)// && ZLScript.state > 0)
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
}
