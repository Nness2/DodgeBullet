using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Bullet : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int player;
    public bool touchedGround;


    private void Start()
    {
        touchedGround = false;

    }
    void OnCollisionEnter(Collision collision)
    {
        
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();

        if (touchedGround)
            return;
       
        if (health != null)
        {
            if (hit.GetComponent<FullControl>().isLocal)
            {
                var ZLScript = hit.GetComponent<ZoneLimitations>();
                bool kill = health.TakeDamage(70);

                if (kill) //Si y a kill le joueur redescend
                {
                    //UpCounter
                    //var FC = GetComponent<FullControl>().killNbr++;
                    //GameObject.FindGameObjectWithTag("killManager").GetComponent<Text>().text = GetComponent<FullControl>().killNbr.ToString();

                    ZLScript.UpState();
                    //ZLScript.UpdateZone();

                    GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

                    foreach (GameObject child in characters)
                    {
                        if (child.GetComponent<FullControl>().PlayerID == player)// && ZLScript.state > 0)
                        {
                            int killer = child.GetComponent<FullControl>().PlayerID;
                            int killed = hit.GetComponent<FullControl>().PlayerID;
                            health.KillManager(killer, killed, false);
                            //Destroy(gameObject);
                        }
                    }
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
        PickUp();
    }

    void PickUp()
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
                        int player = child.GetComponent<FullControl>().PlayerID;
                        child.GetComponent<FullControl>().CmdPickUp(player);
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

}


/*
 *         var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 2.0f));
        Vector3 dir = (point - cam.transform.position).normalized;
        Vector3 force = Vector3.Project(dir, point);
        bullet.GetComponent<Rigidbody>().AddForce(force);
*/
