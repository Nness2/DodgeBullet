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
        GameObject[] characters1 = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child1 in characters1)
        {

            if (child1.GetComponent<FullControl>().isLocal && child1.GetComponent<FullControl>().PlayerID == player)
            {
                child1.GetComponent<BulletManager>().ShotBallSplit(gameObject.transform.position, gameObject.transform, InitialDir);
            }
        }

    }

}
