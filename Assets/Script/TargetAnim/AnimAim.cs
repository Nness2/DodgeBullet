using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class AnimAim : NetworkBehaviour
{

    //public string TargetName;
    public GameObject Target;
    public int PlayerId;
    // Start is called before the first frame update
    void Start()
    {
        PlayerId = gameObject.transform.parent.parent.parent.parent.GetComponent<FullControl>().PlayerID;
        //Target = GameObject.FindGameObjectWithTag(TargetName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
        {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("TargetAnimator");

            foreach (GameObject child in characters)
            {
                if (child.GetComponent<TargetAnimator>().PlayerId == PlayerId)
                {
                    Target = child;
                }
            }
        }
        else
            transform.position = Target.transform.position; 
        //Vector3 SpherePose = GameObject.FindGameObjectWithTag("SphereTarget").transform.position;
        //gameObject.transform.position = Target.transform.position;

    }


}
