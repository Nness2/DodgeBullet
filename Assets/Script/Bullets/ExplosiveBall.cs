using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBall : Bullet
{
    // Start is called before the first frame update
    private bool _splited;

    void Start()
    {
        _splited = false;
        BulletType = 5;
        GetComponent<Identifier>().Id = player;
    }

    // Update is called once per frame
    void Update()
    {
        PickUp();
        if (!_splited)
        {
            if (touchedGround)
            {
                SplitExplosion();
                _splited = true;
            }
        }
    }

    private void SplitExplosion()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {

            if (child.GetComponent<FullControl>().isLocal && child.GetComponent<FullControl>().PlayerID == player)
            {
                child.GetComponent<BulletManager>().CmdBallExplosion(gameObject.transform, plyTouched);
            }
        }
    }
}
