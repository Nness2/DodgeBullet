using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DodgeBullet;
using UnityEngine.UI;

public class BulletManager : NetworkBehaviour
{
    enum BallTypes : int { Bullet, Vellet, Twollet, RainBall, ShotBall};
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

    // Start is called before the first frame update
    void Start()
    {
        _Handball.Value = 4;
        _Pocketball.Value = 1;

        lineVisual.positionCount = ligneSegment;
        lineVisual.enabled = false;

        _lob = false;
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

        if (Input.GetMouseButton(0))// && InGame && !dead)
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

        if (Input.GetMouseButtonUp(0)) // && GetComponent<FullControl>().InGame)// && InGame && !dead)
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
        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;

        bulletLeft.GetComponent<Rigidbody>().AddForce(dirRight * 7000);// * (2000));
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletLeft.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;
        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;

        NetworkServer.Spawn(bulletRight);
        NetworkServer.Spawn(bulletLeft);

        Destroy(bulletRight, 1);
        Destroy(bulletLeft, 1);

    }

    public void ChangeHandMat()
    {
        if (_Handball.Value == -1)
            HandImage.GetComponent<Image>().sprite = NoBallSprite;
        if (_Handball.Value == (int)BallTypes.Bullet)
            HandImage.GetComponent<Image>().sprite = GreenBallSprite;
        if (_Handball.Value == (int)BallTypes.Vellet)
            HandImage.GetComponent<Image>().sprite = VelletBallSprite;
        if (_Handball.Value == (int)BallTypes.RainBall)
            HandImage.GetComponent<Image>().sprite = BlueBallSprite;
        if (_Handball.Value == (int)BallTypes.ShotBall)
            HandImage.GetComponent<Image>().sprite = RedBallSprite;
    }

    public void ChangePocketMat()
    {
        if (_Pocketball.Value == -1)
            PocketImage.GetComponent<Image>().sprite = NoBallSprite;
        if (_Pocketball.Value == (int)BallTypes.Vellet)
            PocketImage.GetComponent<Image>().sprite = VelletBallSprite;
        if (_Pocketball.Value == (int)BallTypes.RainBall)
            PocketImage.GetComponent<Image>().sprite = BlueBallSprite;
        if (_Pocketball.Value == (int)BallTypes.ShotBall)
            PocketImage.GetComponent<Image>().sprite = RedBallSprite;
        if (_Pocketball.Value == (int)BallTypes.Bullet)
            PocketImage.GetComponent<Image>().sprite = GreenBallSprite;
    }

    [Command]
    public void SplitRain(Vector3 pose, Transform vel)
    {


        var bulletRight = Instantiate(bulletPrefab, pose , transform.rotation);
        var bulletLeft = Instantiate(bulletPrefab, pose , transform.rotation);
        var bulletTop = Instantiate(bulletPrefab, pose , transform.rotation);
        var bulletBot = Instantiate(bulletPrefab, pose , transform.rotation);

        Vector3 dirLeft = vel.TransformDirection(new Vector3(-0.05f, 0, 0.25f));
        Vector3 dirRight = vel.TransformDirection(new Vector3(0, 0.05f, -0.25f));
        Vector3 dirTop = vel.TransformDirection(new Vector3(0.05f, 0, 0.25f));
        Vector3 dirBot = vel.TransformDirection(new Vector3(0, -0.05f, -0.25f));

        bulletRight.GetComponent<Rigidbody>().AddForce(dirLeft * 5000);
        bulletRight.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletRight.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Slow;
        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;

        bulletLeft.GetComponent<Rigidbody>().AddForce(dirRight * 5000);
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletLeft.GetComponent<Bullet>().BallEffect = (int)BallEffects.Slow;
        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;

        bulletTop.GetComponent<Rigidbody>().AddForce(dirTop * 5000);
        bulletTop.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletTop.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletTop.GetComponent<Bullet>().BallEffect = (int)BallEffects.Slow;
        bulletTop.tag = "SplittedBall";
        bulletTop.layer = 21;

        bulletBot.GetComponent<Rigidbody>().AddForce(dirBot * 5000);
        bulletBot.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletBot.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletBot.GetComponent<Bullet>().BallEffect = (int)BallEffects.Slow;
        bulletBot.tag = "SplittedBall";
        bulletBot.layer = 21;

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
    public void ShotBallSplit(Vector3 pose, Transform vel, Vector3 InitDir)
    {
        var bulletRight = Instantiate(bulletPrefab, pose, transform.rotation);
        var bulletLeft = Instantiate(bulletPrefab, pose, transform.rotation);



        Vector3 dirLeft = vel.TransformDirection(new Vector3(0.05f, 0.01f, 0.25f));
        Vector3 dirRight = vel.TransformDirection(new Vector3(-0.05f, 0.01f, 0.25f));
        Vector3 dirFront = vel.TransformDirection(Vector3.forward);


        bulletRight.tag = "SplittedBall";
        bulletRight.layer = 21;
        bulletRight.GetComponent<Rigidbody>().AddForce((InitDir + new Vector3(0.05f, 0f, 0f)) * 13000);// * (1000));
        //bulletRight.GetComponent<Rigidbody>().AddForce(InitDir * 13000);
        bulletRight.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletRight.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;


        bulletLeft.tag = "SplittedBall";
        bulletLeft.layer = 21;
        bulletLeft.GetComponent<Rigidbody>().AddForce((InitDir + new Vector3(-0.05f, 0f, 0f)) * 13000);// * (2000));
        //bulletLeft.GetComponent<Rigidbody>().AddForce(InitDir * 13000);
        bulletLeft.GetComponent<Bullet>().player = GetComponent<FullControl>().PlayerID;
        bulletLeft.GetComponent<Bullet>().teamBlue = GetComponent<ZoneLimitations>().teamBlue;
        bulletRight.GetComponent<Bullet>().BallEffect = (int)BallEffects.Kill;

        NetworkServer.Spawn(bulletRight);
        NetworkServer.Spawn(bulletLeft);

        Destroy(bulletRight, 1);
        Destroy(bulletLeft, 1);

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
