using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBall : Bullet
{
    // Start is called before the first frame update

    void Start()
    {
        //BallEffect = (int)BallEffects.Kill;

        BulletType = 4;
        ShotBallSplit();
        GetComponent<Identifier>().Id = player;

        /*GameObject[] characters1 = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child1 in characters1)
        {

            if (child1.GetComponent<FullControl>().PlayerID != 9 && child1.GetComponent<FullControl>().isLocal)
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {

        PickUp();
    }

    private void ShotBallSplit()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {

            if (child.GetComponent<FullControl>().isLocal && child.GetComponent<FullControl>().PlayerID == player)
            {
                child.GetComponent<BulletManager>().CmdShotBallSplit(gameObject.transform.position, gameObject.transform, InitialDir);
            }
        }

    }

}
