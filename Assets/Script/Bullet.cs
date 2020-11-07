using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if(health != null)
        {
            health.TakeDamage(10);
        }
        Destroy(gameObject);
    }
}


/*
 *         var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 2.0f));
        Vector3 dir = (point - cam.transform.position).normalized;
        Vector3 force = Vector3.Project(dir, point);
        bullet.GetComponent<Rigidbody>().AddForce(force);
*/