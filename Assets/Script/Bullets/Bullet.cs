using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

[System.Serializable]
public class Bullet : NetworkBehaviour
{
    public enum BallEffects : int { Kill, Heal, Stun, Slow, TwoKill };
    enum BallTypes : int { Bullet, Vellet, Twollet, RainBall, ShotBall, ExplosiveBall };


    // Start is called before the first frame update
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int player;
    [SyncVar(hook = nameof(OnChangeTeam))]
    public bool teamBlue;
    public bool touchedGround;
    [SyncVar(hook = nameof(OnChangeBulletType))]
    public int BulletType;
    public GameObject LocalPlayer;
    public int BallEffect;

    public float pullRadius = 20;
    public float pullForce = 10;

    [SyncVar(hook = nameof(OnChangeInitialDir))]
    public Vector3 InitialDir;

    public Material BlueMat;
    public Material RedMat;
    public Material GreeneMat;
    public Material VelletMat;
    public Material GoldMat;

    [SyncVar(hook = nameof(OnChangeplyTouched))]
    public int plyTouched;


    private void Start()
    {
        //BallEffect = (int)BallEffects.Heal;
        GetComponent<Identifier>().Id = player;
        ChangeBallMat();
        /*GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal && child.GetComponent<FullControl>().PlayerID != player)
            {
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<SphereCollider>());
            }
        }*/
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
            if ((touchedGround && gameObject.tag == "Bullet"))
                return;

            GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().isLocal && hit.GetComponent<FullControl>().PlayerID == child.GetComponent<FullControl>().PlayerID)
                {
                    child.GetComponent<BulletManager>().CmdBallEffect(hit.GetComponent<FullControl>().PlayerID, BallEffect, hit.GetComponent<ZoneLimitations>().teamBlue, teamBlue);
                    child.GetComponent<Health>().KillManager(player, hit.GetComponent<FullControl>().PlayerID);
                    plyTouched = -1;
                }
            }

            return;
        }

        if (hit.tag == "GroundField" && gameObject.tag == "Bullet")
        {
            touchedGround = true;
            plyTouched = -1;
            return;
        }



        //Destroy(gameObject);
    }


    private void FixedUpdate()
    {
        //RayCastCollider();
        if (gameObject.tag == "SplittedBall")
        {
            return;
        }

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

    void OnChangeplyTouched(int oldValue, int newValue)
    {
        plyTouched = newValue;
    }

void OnChangeBulletType(int oldValue, int newValue)
    {
        BulletType = newValue;
    }

    void OnChangeInitialDir(Vector3 oldValue, Vector3 newValue)
    {
        InitialDir = newValue;
    }

    public void ChangeBallMat()
    {

        if (BulletType == (int)BallTypes.Bullet)
        {
            GetComponent<MeshRenderer>().material = GreeneMat;
        }
        else if (BulletType == (int)BallTypes.Vellet)
        {
            GetComponent<MeshRenderer>().material = VelletMat;
        }
        else if (BulletType == (int)BallTypes.RainBall)
        {
            GetComponent<MeshRenderer>().material = BlueMat;
        }
        else if (BulletType == (int)BallTypes.ShotBall)
        {
            gameObject.GetComponent<MeshRenderer>().material = RedMat;

        }
        else if (BulletType == (int)BallTypes.ExplosiveBall)
        {
            GetComponent<MeshRenderer>().material = GoldMat;
        }
    }

    public void RayCastCollider()
    {
        List<Vector3> Raydir = new List<Vector3>();

        Raydir.Add(new Vector3(-1, -1, -1));
        Raydir.Add(new Vector3(1, -1, -1));
        Raydir.Add(new Vector3(-1, 1, -1));
        Raydir.Add(new Vector3(-1, -1, 1));
        Raydir.Add(new Vector3(1, 1, -1));
        Raydir.Add(new Vector3(-1, 1, 1));
        Raydir.Add(new Vector3(1, -1, 1));
        Raydir.Add(new Vector3(1, 1, 1));

        Raydir.Add(new Vector3(1, -1, 0));
        Raydir.Add(new Vector3(-1, 1, 0));
        Raydir.Add(new Vector3(-1, -1, 0));
        Raydir.Add(new Vector3(1, 1, 0));

        Raydir.Add(new Vector3(0, 1, -1));
        Raydir.Add(new Vector3(0, -1, 1));
        Raydir.Add(new Vector3(0, 1, 1));
        Raydir.Add(new Vector3(0, -1, -1));

        Raydir.Add(new Vector3(-1, 0, -1));
        Raydir.Add(new Vector3(1, 0, 1));
        Raydir.Add(new Vector3(-1, 0, 1));
        Raydir.Add(new Vector3(1, 0, -1));

        Raydir.Add(new Vector3(1, 0, 0));
        Raydir.Add(new Vector3(-1, 0, 0));

        Raydir.Add(new Vector3(0, 1, 0));
        Raydir.Add(new Vector3(0, -1, 0));

        Raydir.Add(new Vector3(0, 0, 1));
        Raydir.Add(new Vector3(0, 0, -1));

        int layerMask = 1 << 11;
        foreach (Vector3 Rdir in Raydir)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 15f, layerMask))
            {
                Debug.Log("HIT");
            }
            //else
            //    Debug.DrawRay(transform.position, transform.TransformDirection(Rdir) * 0.15f, Color.yellow);
            
        }
    }

}


/*
 *      var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 2.0f));
        Vector3 dir = (point - cam.transform.position).normalized;
        Vector3 force = Vector3.Project(dir, point);
        bullet.GetComponent<Rigidbody>().AddForce(force);
*/
