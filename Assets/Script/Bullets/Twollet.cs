using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twollet : Bullet
{
    // Start is called before the first frame update
    void Start()
    {
        touchedGround = false;
        BulletType = 1;

    }

    // Update is called once per frame
    void Update()
    {
        PickUp();
    }
}
