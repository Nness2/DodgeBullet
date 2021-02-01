using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UpdateTargetOffset : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Player;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateAnimationOffset(int anim)
    {
        float value = 50;

        if (anim == 0)
        {
            value = 50;
        }
        else if (anim == 1)
        {
            value = 72;
        }
        else if (anim == 2)
        {
            value = 97;
        }
        else if (anim == 3)
        {
            value = 68;
        }
        else if (anim == 4)
        {
            value = 80;
        }
        else if (anim == 5)
        {
            value = 61;
        }
        else if (anim == 6)
        {
            value = 85;
        }
        else if (anim == 7)
        {
            value = 98.5f;
        }
        else if (anim == 8)
        {
            value = 40;
        }
        Player.GetComponent<MultiAimConstraint>().data.offset = new Vector3(0, 0, value);
    }
}
