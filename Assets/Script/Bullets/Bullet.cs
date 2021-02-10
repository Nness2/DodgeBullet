using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

[System.Serializable]
public class Bullet : NetworkBehaviour
{
    public enum BallEffects : int { Kill, Heal, Stun, Slow, TwoKill };


    // Start is called before the first frame update
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int player;
    [SyncVar(hook = nameof(OnChangeTeam))]
    public bool teamBlue;
    public bool touchedGround;
    public int BulletType;
    public GameObject LocalPlayer;
    public int BallEffect;

    private void Start()
    {
        //BallEffect = (int)BallEffects.Heal;
        BulletType = 0;
    }
    void OnCollisionEnter(Collision collision)
    {

        var hit = collision.gameObject;


        if (hit.tag == "CatchWall")
        {
            var playerObj = hit.transform.parent;
            //Debug.Log(player.gameObject.GetComponent<FullControl>().PlayerID);
            GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().isLocal)
                {
                    if (hit.GetComponent<Identifier>().Id != player)
                    {
                        child.GetComponent<BulletManager>().CmdPickUp(gameObject, playerObj.gameObject.GetComponent<FullControl>().PlayerID, BulletType);
                    }
                }
            }
            return;
            //Destroy(gameObject);
        }


        var FCScript = hit.GetComponent<FullControl>();
        if (FCScript != null)
        {
            if (hit.GetComponent<FullControl>().PlayerID == player)
                return;
            if (touchedGround)
                return;

            GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().isLocal && hit.GetComponent<FullControl>().PlayerID == child.GetComponent<FullControl>().PlayerID)
                {
                    Debug.Log("TOuché");
                    child.GetComponent<BulletManager>().CmdBallEffect(hit.GetComponent<FullControl>().PlayerID, BallEffect, hit.GetComponent<ZoneLimitations>().teamBlue, teamBlue);
                }
            }

            return;
        }

        if (hit.tag == "GroundField")
        {
            touchedGround = true;
            return;
        }



        //Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (gameObject.tag == "SplittedBall")
            return;

        PickUp();
    }

    public void PickUp()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {

                float distance = Vector3.Distance(child.transform.position, transform.position);
                if (touchedGround)
                {

                    if (distance <= 2)
                    {

                        int playerId = child.GetComponent<FullControl>().PlayerID;
                        child.GetComponent<BulletManager>().CmdPickUp(gameObject, playerId, BulletType);
                        //Destroy(gameObject);
                    }
                }
            }
        }
    }





    void OnChangePlayer(int oldValue, int newValue)
    {
        player = newValue;
    }

    void OnChangeTeam(bool oldValue, bool newValue)
    {
        teamBlue = newValue;
    }





}


/*
 *      var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 2.0f));
        Vector3 dir = (point - cam.transform.position).normalized;
        Vector3 force = Vector3.Project(dir, point);
        bullet.GetComponent<Rigidbody>().AddForce(force);
*/
