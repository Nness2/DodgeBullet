using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Vellet : Bullet
{

    private bool _splited;

    // Start is called before the first frame update
    void Start()
    {
        //BallEffect = (int)BallEffects.Stun;

        _splited = false;
        BulletType = 1;
    }

        // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !_splited){
            _splited = true;
            SplitVellet();
        }

        PickUp();
    }

    private void SplitVellet()
    {
        GameObject[] characters1 = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child1 in characters1)
        {

            if (child1.GetComponent<FullControl>().isLocal && child1.GetComponent<FullControl>().PlayerID == player)
            {
                child1.GetComponent<BulletManager>().SpliBallSync(gameObject.transform.position, gameObject.transform);

            }
        }

    }
}
