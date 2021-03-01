using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DodgeBullet;
using UnityEngine.UI;

public class BulletManager : NetworkBehaviour
{
    enum BallTypes : int { Bullet, Vellet, Twollet, RainBall, ShotBall, ExplosiveBall};
    enum BallEffects : int { Kill, Heal, Stun, Slow, TwoKill};


    public GameObject bulletPrefab;

    public LineRenderer lineVisual;
    public int ligneSegment = 10;

    public bool _lob;

    [SerializeField] private IntVariable _Handball;
    [SerializeField] private IntVariable _Pocketball;


    public Image HandImage;
    public Image PocketImage;

    public Sprite NoBallSprite;
    public Sprite BlueBallSprite;
    public Sprite GreenBallSprite;
    public Sprite RedBallSprite;
    public Sprite VelletBallSprite;
    public Sprite ExplosiveBallSprite;

    public Material BlueMat;
    public Material RedMat;
    public Material GreeneMat;
    public Material VelletMat;
    public Material GoldMat;
    public Material NoMat;

    public GameObject HandBallObj;
    

    // Start is called before the first frame update
    void Start()
    {

        InitStartBall();

        lineVisual.positionCount = ligneSegment;
        lineVisual.enabled = false;

        _lob = false;
        Transform[] MatChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in MatChildren)
        {
            if (child.CompareTag("HandBall"))
            {
                HandBallObj = child.gameObject;
            }
        }
        ChangeHandMat();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
            return;

        if (_Handball.Value == -1 && _Pocketball.Value != -1)
        {
            _Handball.Value = _Pocketball.Value;
            _Pocketball.Value = -1;
        }


        var FCScritp = GetComponent<FullControl>();
        bool isGround = FCScritp.isGrounded;

        if (Input.GetMouseButton(0) && FCScritp.InGame)// && InGame && !dead)
        {

            if (_Handball.Value != -1)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    _lob = !_lob;
                }
                lineVisual.enabled = true;

                var FCScript = GetComponent<FullControl>();

                Vector3 position = FCScript.cam.transform.position;
                Vector3 forward = FCScript.cam.transform.TransformDirection(Vector3.forward);
                BallPreFire(position, forward);

            }
        }
        
        else
        {
            lineVisual.enabled = false;
            GameObject[] PreBullets = GameObject.FindGameObjectsWithTag("PreBullet");
            foreach (GameObject child in PreBullets)
            {
                Destroy(child);
            }
        }

        if (Input.GetMouseButtonUp(0) && FCScritp.InGame && isGround)// && InGame && !dead)
        {
            GameObject[] PreBullets = GameObject.FindGameObjectsWithTag("PreBullet");
            foreach (GameObject child in PreBullets)
            {
                Destroy(child);
            }
            var FCScript = GetComponent<FullControl>();
            Vector3 position = FCScript.cam.transform.position;
            Vector3 forward = FCScript.cam.transform.TransformDirection(Vector3.forward);
            GetComponent<FullControl>().BallFire(FCScript.PlayerID, position, forward, _lob, _Handball.Value);
            _Handball.Value = -1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))// && !Input.GetMouseButton(1))// && InGame && !dead)
        {
            int tmp = _Handball.Value;
            _Handball.Value = _Pocketball.Value;
            _Pocketball.Value = tmp;

        }
        //Debug.Log(HandBall);
        //Debug.Log(PocketBall);
    }

    public void InitStartBall()
    {
        _Handball.Value = 4;
        _Pocketball.Value = 5;
    }



    public void ChangeHandSprite()
    {
        if (_Handball.Value == -1)
            HandImage.GetComponent<Image>().sprite = NoBallSprite;

        else if (_Handball.Value == (int)BallTypes.Bullet)
            HandImage.GetComponent<Image>().sprite = GreenBallSprite;

        else if (_Handball.Value == (int)BallTypes.Vellet)
            HandImage.GetComponent<Image>().sprite = VelletBallSprite;

        else if (_Handball.Value == (int)BallTypes.RainBall)
            HandImage.GetComponent<Image>().sprite = BlueBallSprite;

        else if (_Handball.Value == (int)BallTypes.ShotBall)
            HandImage.GetComponent<Image>().sprite = RedBallSprite;

        else if (_Handball.Value == (int)BallTypes.ExplosiveBall)
            HandImage.GetComponent<Image>().sprite = ExplosiveBallSprite;
    }


 

    public void ChangePocketSprite()
    {
        if (_Pocketball.Value == -1)
            PocketImage.GetComponent<Image>().sprite = NoBallSprite;

        else if(_Pocketball.Value == (int)BallTypes.Vellet)
            PocketImage.GetComponent<Image>().sprite = VelletBallSprite;

        else if (_Pocketball.Value == (int)BallTypes.RainBall)
            PocketImage.GetComponent<Image>().sprite = BlueBallSprite;

        else if (_Pocketball.Value == (int)BallTypes.ShotBall)
            PocketImage.GetComponent<Image>().sprite = RedBallSprite;

        else if (_Pocketball.Value == (int)BallTypes.Bullet)
           PocketImage.GetComponent<Image>().sprite = GreenBallSprite;

        else if (_Pocketball.Value == (int)BallTypes.ExplosiveBall)
            PocketImage.GetComponent<Image>().sprite = ExplosiveBallSprite;
        
    }

    public void ChangeHandMat()
    {
        CmdChangeHandMat(GetComponent<FullControl>().PlayerID, _Handball.Value);
    }

    [Command]
    public void CmdChangeHandMat(int player, int ballType)
    {
        ClientChangeHandMat(player, ballType);
    }

    [ClientRpc]
    public void ClientChangeHandMat(int player, int ballType)
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == player)
            {
                if (ballType == -1)
                {
                    if (child.GetComponent<BulletManager>().HandBallObj != null)
                        child.GetComponent<BulletManager>().HandBallObj.GetComponent<MeshRenderer>().material = NoMat;
                }
                if (ballType == (int)BallTypes.Bullet)
                {
                    if (child.GetComponent<BulletManager>().HandBallObj != null)
                        child.GetComponent<BulletManager>().HandBallObj.GetComponent<MeshRenderer>().material = GreeneMat;
                }
                if (ballType == (int)BallTypes.Vellet)
                {
                    if (child.GetComponent<BulletManager>().HandBallObj != null)
                        child.GetComponent<BulletManager>().HandBallObj.GetComponent<MeshRenderer>().material = VelletMat;
                }
                if (ballType == (int)BallTypes.RainBall)
                {
                    if (child.GetComponent<BulletManager>().HandBallObj != null)
                        child.GetComponent<BulletManager>().HandBallObj.GetComponent<MeshRenderer>().material = BlueMat;
                }
                if (ballType == (int)BallTypes.ShotBall)
                {
                    if (child.GetComponent<BulletManager>().HandBallObj != null)
                        child.GetComponent<BulletManager>().HandBallObj.GetComponent<MeshRenderer>().material = RedMat;

                }
                if (ballType == (int)BallTypes.ExplosiveBall)
                {
                    if (child.GetComponent<BulletManager>().HandBallObj != null)
                        child.GetComponent<BulletManager>().HandBallObj.GetComponent<MeshRenderer>().material = GoldMat;
                }
            }
        }
    }

    [Command]
    public void SpliBallSync(Vector3 pose, Transform vel)
    {
        var bulletRight = Instantiate(bulletPrefab, pose, transform.rotation);
        var bulletLeft = Instantiate(bulletPrefab, pose, Quaternion.identity);

        Vector3 dirLeft = vel.TransformDirection(Vector3.left);
        Vector3 dirRight = vel.TransformDirection(Vector3.right);


        bulletRight.GetComponent<Rigidbody>().AddForce(dirLeft * 7000);// * (1000));
        bulletRight.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletRight.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletRight.GetComponent<Bullet>().BulletType = 1;
        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;
        bulletRight.GetComponent<MeshRenderer>().material = VelletMat;


        bulletLeft.GetComponent<Rigidbody>().AddForce(dirRight * 7000);// * (2000));
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletLeft.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletLeft.GetComponent<Bullet>().BulletType = 1;
        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;
        bulletLeft.GetComponent<MeshRenderer>().material = VelletMat;


        NetworkServer.Spawn(bulletRight);
        NetworkServer.Spawn(bulletLeft);

        Destroy(bulletRight, 1);
        Destroy(bulletLeft, 1);

    }

    [Command]
    public void SplitRain(Vector3 pose, Transform vel, GameObject RB)
    {
        var bulletRight = Instantiate(bulletPrefab, pose , transform.rotation);
        var bulletLeft = Instantiate(bulletPrefab, pose , transform.rotation);
        var bulletTop = Instantiate(bulletPrefab, pose , transform.rotation);
        var bulletBot = Instantiate(bulletPrefab, pose , transform.rotation);

        Vector3 dirLeft = vel.TransformDirection(new Vector3(-0.05f, 0, 0.25f));
        Vector3 dirRight = vel.TransformDirection(new Vector3(0, 0.05f, -0.25f));
        Vector3 dirTop = vel.TransformDirection(new Vector3(0.05f, 0, 0.25f));
        Vector3 dirBot = vel.TransformDirection(new Vector3(0, -0.05f, -0.25f));

        bulletRight.GetComponent<Rigidbody>().velocity = RB.GetComponent<Rigidbody>().velocity;
        bulletRight.GetComponent<Rigidbody>().AddForce(dirRight * 5000);
        bulletRight.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletRight.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletRight.GetComponent<Bullet>().BulletType = 3;
        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;
        bulletRight.GetComponent<MeshRenderer>().material = BlueMat;


        bulletLeft.GetComponent<Rigidbody>().velocity = RB.GetComponent<Rigidbody>().velocity;
        bulletLeft.GetComponent<Rigidbody>().AddForce(dirLeft * 5000);
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletLeft.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletLeft.GetComponent<Bullet>().BulletType = 3;
        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;
        bulletLeft.GetComponent<MeshRenderer>().material = BlueMat;


        bulletTop.GetComponent<Rigidbody>().velocity = RB.GetComponent<Rigidbody>().velocity;
        bulletTop.GetComponent<Rigidbody>().AddForce(dirTop * 5000);
        bulletTop.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletTop.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletTop.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletTop.GetComponent<Bullet>().BulletType = 3;
        bulletTop.tag = "SplittedBall";
        bulletTop.layer = 21;
        bulletTop.GetComponent<MeshRenderer>().material = BlueMat;


        bulletBot.GetComponent<Rigidbody>().velocity = RB.GetComponent<Rigidbody>().velocity;
        bulletBot.GetComponent<Rigidbody>().AddForce(dirBot * 5000);
        bulletBot.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletBot.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletBot.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletBot.GetComponent<Bullet>().BulletType = 3;
        bulletBot.tag = "SplittedBall";
        bulletBot.layer = 21;
        bulletBot.GetComponent<MeshRenderer>().material = BlueMat;


        NetworkServer.Spawn(bulletRight);
        NetworkServer.Spawn(bulletLeft);
        NetworkServer.Spawn(bulletTop);
        NetworkServer.Spawn(bulletBot);
        Destroy(bulletRight, 2);
        Destroy(bulletLeft, 2);
        Destroy(bulletTop, 2);
        Destroy(bulletBot, 2);
    }



    [Command]
    public void CmdShotBallSplit(Vector3 pose, Transform vel, Vector3 InitDir)
    {
        var bulletRight = Instantiate(bulletPrefab, pose, transform.rotation);
        var bulletLeft = Instantiate(bulletPrefab, pose, transform.rotation);

        //Vector3 dirLeft = vel.TransformDirection(Vector3.left);
        //Vector3 dirRight = vel.TransformDirection(Vector3.right);

        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;
        bulletRight.GetComponent<Rigidbody>().AddForce((InitDir + new Vector3(0.07f, 0f, 0f)) * 13000);// * (1000));
        bulletRight.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletRight.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletRight.GetComponent<Bullet>().BulletType = 4;
        bulletRight.GetComponent<MeshRenderer>().material = RedMat;


        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;
        bulletLeft.GetComponent<Rigidbody>().AddForce((InitDir + new Vector3(-0.07f, 0f, 0f)) * 13000);// * (2000));
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletLeft.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletLeft.GetComponent<Bullet>().BulletType = 4;
        bulletLeft.GetComponent<MeshRenderer>().material = RedMat;


        NetworkServer.Spawn(bulletRight);
        NetworkServer.Spawn(bulletLeft);

        Destroy(bulletRight, 1);
        Destroy(bulletLeft, 1);
    }


    [Command]
    public void CmdBallExplosion(Transform TBull)
    {
        var bulletRight = Instantiate(bulletPrefab, TBull.position, transform.rotation);
        var bulletLeft = Instantiate(bulletPrefab, TBull.position, transform.rotation);
        var bulletTop = Instantiate(bulletPrefab, TBull.position, transform.rotation);
        var bulletBot = Instantiate(bulletPrefab, TBull.position, transform.rotation);


        Vector3 dirLeft = TBull.TransformDirection(Vector3.left);
        Vector3 dirRight = TBull.TransformDirection(Vector3.right);
        Vector3 dirTop = TBull.TransformDirection(Vector3.forward);
        Vector3 dirBot = TBull.TransformDirection(Vector3.back);


        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;
        bulletRight.GetComponent<Rigidbody>().AddForce((dirLeft + new Vector3(0f, 0.5f, 0f)) * 3000);
        bulletRight.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletRight.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletRight.GetComponent<Bullet>().BulletType = 5;
        bulletRight.GetComponent<MeshRenderer>().material = GoldMat;

        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;
        bulletLeft.GetComponent<Rigidbody>().AddForce((dirRight + new Vector3(0f, 0.5f, 0f)) * 3000);
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletLeft.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletLeft.GetComponent<Bullet>().BulletType = 5;
        bulletLeft.GetComponent<MeshRenderer>().material = GoldMat;

        bulletTop.tag = "SplittedBall";
        bulletTop.layer = 21;
        bulletTop.GetComponent<Rigidbody>().AddForce((dirTop + new Vector3(0f, 0.5f, 0f)) * 3000);
        bulletTop.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletTop.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletTop.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletTop.GetComponent<Bullet>().BulletType = 5;
        bulletTop.GetComponent<MeshRenderer>().material = GoldMat;

        bulletBot.tag = "SplittedBall";
        bulletBot.layer = 21;
        bulletBot.GetComponent<Rigidbody>().AddForce(dirBot + (new Vector3(0f, 0.5f, 0f)) * 3000);
        bulletBot.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletBot.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletBot.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletBot.GetComponent<Bullet>().BulletType = 5;
        bulletBot.GetComponent<MeshRenderer>().material = GoldMat;

        NetworkServer.Spawn(bulletRight);
        NetworkServer.Spawn(bulletLeft);
        NetworkServer.Spawn(bulletTop);
        NetworkServer.Spawn(bulletBot);

        Destroy(bulletRight, 1);
        Destroy(bulletLeft, 1);
        Destroy(bulletTop, 1);
        Destroy(bulletBot, 1);

    }

    #region PickUp
    [Command]
    public void CmdPickUp(GameObject ball, int plyr, int balleType)
    {
        ClientPickUp(plyr, balleType, ball);
        //NetworkServer.Destroy(ball);

    }

    [ClientRpc]
    void ClientPickUp(int plyr, int balleType, GameObject ball)
    {
        if (ball != null)
        {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
            foreach (GameObject child in characters)
            {

                if (child.GetComponent<FullControl>().PlayerID == plyr && child.GetComponent<FullControl>().isLocal)
                {
                    var BMScript = GetComponent<BulletManager>();

                    if (BMScript._Handball.Value == -1)
                    {
                        BMScript._Handball.Value = balleType;
                        NetworkServer.Destroy(ball);
                        CmdDestroyBall(ball);
                        return;
                    }

                    else if (BMScript._Handball.Value != -1 && BMScript._Pocketball.Value == -1)
                    {
                        BMScript._Pocketball.Value = balleType;
                        NetworkServer.Destroy(ball);
                        CmdDestroyBall(ball);
                        return;
                    }

                    else
                    {
                        return;
                    }
                    //child.GetComponent<FullControl>()._ball.Value = 1;
                }
            }
            //GameObject ball = GameObject.FindGameObjectWithTag("Bullet"); // destroy ball here to late time gotball process
        }

    }

    [Command]
    public void CmdDestroyBall(GameObject ball)
    {
        NetworkServer.Destroy(ball);

    }
    #endregion

    void BallPreFire(Vector3 position, Vector3 forward)
    {
        bool lob = _lob;
        //int layerMask = 1 << 11;
        int grnd = 1 << LayerMask.NameToLayer("Ground");
        int plyr = 1 << LayerMask.NameToLayer("Player");

        int SecondLayer = 1 << LayerMask.NameToLayer("SecondLayer");
        int mask3 = SecondLayer;
        RaycastHit hit3;

        Vector3 SecondPose = Vector3.zero;
        if (Physics.Raycast(position, forward, out hit3, Mathf.Infinity, mask3))
            SecondPose = hit3.point;
        else
            return;

        int mask = grnd | plyr;
        RaycastHit hit;
        Vector3 dir;
        if (Physics.Raycast(SecondPose, forward, out hit, Mathf.Infinity, mask))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log(hit.point);
            dir = hit.point - GetComponent<FullControl>().bulletSpawn.position;
            dir = dir.normalized;
            //bullet.GetComponent<Rigidbody>().AddForce(dir * 15000);
        }
        else
        {
            Debug.Log("WRONNG");
            dir = Vector3.zero;
            //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;
        }

         
        if (_Handball.Value == (int)BallTypes.RainBall)
        {
            lob = true;
        }

        else if (_Handball.Value == (int)BallTypes.ShotBall || _Handball.Value == (int)BallTypes.Vellet)
        {
            lob = false;
        }


        if (!lob)
        {
            float v = (13000 * Time.fixedDeltaTime / 5);
            VisualizeSegment(dir * v);
        }
        else
        {
            float v = (2000 * Time.fixedDeltaTime / 5);
            VisualizeSegment((dir + new Vector3(0, 1.1f, 0).normalized) * v);
        }

    }

    Vector3 CalculatePositionInTime(Vector3 vo, float time)
    {
        Vector3 Vxz = vo;
        Vxz.y = 0f;
        Vector3 shootPoint = GetComponent<FullControl>().bulletSpawn.transform.position;
        Vector3 result = shootPoint + vo * time;
        float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + shootPoint.y;

        result.y = sY;

        return result;
    }

    void VisualizeSegment(Vector3 vo)
    {
        for (int i = 0; i < ligneSegment; i++)
        {
            Vector3 pos = CalculatePositionInTime(vo, i / (float)ligneSegment * 3);
            lineVisual.SetPosition(i, pos);
        }
    }



    [Command]
    public void CmdBallEffect(int player, int effect, bool touched, bool shooter)
    {
        ClientBallEffectCall(player, effect, touched, shooter);
    }

    [ClientRpc]
    private void ClientBallEffectCall(int player, int effect, bool touched, bool shooter)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");   

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal && child.GetComponent<FullControl>().PlayerID == player)
            {
                if (effect == (int)BallEffects.Kill)
                {
                    child.GetComponent<BulletEffects>().BallEffectKill(touched == shooter);
                }

                if (effect == (int)BallEffects.Heal)
                {
                    child.GetComponent<BulletEffects>().BallEffectHeal(touched == shooter);

                }

                if (effect == (int)BallEffects.Stun)
                {
                    child.GetComponent<BulletEffects>().BallEffectStun(touched == shooter);

                }

                if (effect == (int)BallEffects.Slow)
                {
                    child.GetComponent<BulletEffects>().BallEffectSlow(touched == shooter);
                }
            }
        }
    }
}
