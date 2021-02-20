using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffects : MonoBehaviour
{
    private IEnumerator coroutine;

    public void BallEffectKill(bool ally) {
        if (!ally)
        {

            GetComponent<ZoneLimitations>().UpState();
        }
    }

    public void BallEffectHeal(bool ally) {
        if (ally)
        {
            GetComponent<ZoneLimitations>().DownState();
        }
    }

    public void BallEffectStun(bool ally)
    {
        if (!ally)
        {
            GetComponent<AnimationStateControler>().penalty = 0;
            coroutine = WaitAndPrint(2.0f);
            StartCoroutine(coroutine);
        }
    }

    public void BallEffectSlow(bool ally)
    {
        if (!ally)
        {
            GetComponent<AnimationStateControler>().penalty = 0.5f;
            coroutine = WaitAndPrint(2.0f);
            StartCoroutine(coroutine);
        }
    }


    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GetComponent<AnimationStateControler>().penalty = 1;

    }
}
