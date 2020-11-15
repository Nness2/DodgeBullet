using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpellManager : NetworkBehaviour
{

    public GameObject wallPrefab;

    private float wallCd;
    private bool wallReady;

    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {
        wallCd = 5;
        wallReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(wallReady);
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
}
