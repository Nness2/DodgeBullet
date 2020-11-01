using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotShoot : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Fire();
    }

    void Fire()
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;

        Destroy(bullet, 2.0f);
    }
}
