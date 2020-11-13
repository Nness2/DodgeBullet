using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    // Start is called before the first frame update
    [SyncVar(hook = nameof(OnChangePlayer))]
    public int player;

    private void Start()
    {
        //player = 1;

    }
    void OnCollisionEnter(Collision collision)
    {

        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();

        if (health != null)
        {
            var ZLScript = hit.GetComponent<ZoneLimitations>();
            bool kill = health.TakeDamage(10);

            if (kill) //Si y a kill le joueur redescend
            {
                ZLScript.upState();
                ///upState permet une synchronisation mais probleme de zone chez les rouges, voir si on peut se contenter d'un simple incrémentation
                //ZLScript.state++;
                ZLScript.UpdateZone();
                GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
                
                foreach (GameObject child in characters)
                {
                    //Debug.Log("child = "+ child.GetComponent<FullControl>().selfNumber);
                    //var ZLScript = child.GetComponent<ZoneLimitations>();
                    //Debug.Log("self"+child.GetComponent<FullControl>().selfNumber);
                    //Debug.Log("player"+player);
                    if (child.GetComponent<FullControl>().selfNumber == player)// && ZLScript.state > 0)
                    {
                        //tester d'appeler hook de self ici
                        //var killer = child.GetComponent<Health>();
                        //killer.UpZone();
                        //if(isServer)
                        //child.GetComponent<ZoneLimitations>().CmdDownState();
                        int killer = child.GetComponent<FullControl>().selfNumber;
                        int killed = hit.GetComponent<FullControl>().selfNumber;
                        health.KillManager(killer, killed);
                    }
                }
            }
        }
        Destroy(gameObject);
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
