using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RainBall : Bullet
{
    // Start is called before the first frame update

    private bool _splited;


    void Start()
    {
        //BallEffect = (int)BallEffects.Slow;

        _splited = false;
        BulletType = 3;
        GetComponent<Identifier>().Id = player;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !_splited)
        {
            _splited = true;
            SplitRain();
        }

        PickUp();
    }

    private void SplitRain()
    {
        GameObject[] characters1 = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child1 in characters1)
        {

            if (child1.GetComponent<FullControl>().isLocal && child1.GetComponent<FullControl>().PlayerID == player)
            {
                child1.GetComponent<BulletManager>().SplitRain(gameObject.transform.position, gameObject.transform, gameObject, plyTouched);

            }
        }

    }

}
