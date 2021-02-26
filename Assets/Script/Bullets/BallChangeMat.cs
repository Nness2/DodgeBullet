using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DodgeBullet;

public class BallChangeMat : MonoBehaviour
{
    enum BallTypes : int { Bullet, Vellet, Twollet, RainBall, ShotBall, ExplosiveBall };
    // Start is called before the first frame update
    [SerializeField] private IntVariable _Handball;
    [SerializeField] private IntVariable _Pocketball;


    public Material BlueMat;
    public Material RedMat;
    public Material GreeneMat;
    public Material VelletMat;
    public Material GoldMat;
    public Material NoMat;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_Handball.Value == (int)BallTypes.Bullet)
            gameObject.GetComponent<MeshRenderer>().material = GreeneMat;
        if (_Handball.Value == (int)BallTypes.Vellet)
            gameObject.GetComponent<MeshRenderer>().material = VelletMat;
        if (_Handball.Value == (int)BallTypes.RainBall)
            gameObject.GetComponent<MeshRenderer>().material = BlueMat;
        if (_Handball.Value == (int)BallTypes.ShotBall)
            gameObject.GetComponent<MeshRenderer>().material = RedMat;
        if (_Handball.Value == (int)BallTypes.ExplosiveBall)
            gameObject.GetComponent<MeshRenderer>().material = GoldMat;
        if (_Handball.Value == -1)
            gameObject.GetComponent<MeshRenderer>().material = NoMat;
    }


}
