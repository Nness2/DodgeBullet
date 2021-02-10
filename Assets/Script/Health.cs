using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Cinemachine;
using DodgeBullet;


public class Health : NetworkBehaviour
{

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;

    public RectTransform healthBar;
    float RecInitX;
    [SerializeField] private IntVariable _ball;
    [SerializeField] private StringVariable _health;

    public RectTransform DamageDealed;
    public Text DamagePrefab;
    private IEnumerator coroutine;

    public GameObject SelfCanvas;
    private Text DamageDealedDisplay;

    void Start()
    {
        RecInitX = healthBar.rect.position.x;
        _health.Value = "100 / 100";


}

//take Damage
public bool TakeDamage(int amount) //return true si il y a kill
    {
        //if (!isLocalPlayer)
        //return false;


        bool isDead = false;
        var ZL = GetComponent<ZoneLimitations>();
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        _health.Value = currentHealth.ToString() + " / 100";
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        //healthBar.position = new Vector2(RecInitX - (maxHealth - currentHealth), healthBar.position.y);

        if (currentHealth <= 0)
        {

            //ZL.state++;
            if (ZL.state > 3)//2) //state est up après, il faut anticiper de 1, attention à l'utilisation du stateDown et du state--
            {
                var FC = GetComponent<FullControl>();
                //FC.UpdateDeadCam();
                //FC.CmdDisplayPlayer(FC.selfNumber, false);
                GetComponent<ZoneLimitations>().CmdInitState();
                FC.dead = true;
                FC.CmdDeadPlayer(FC.PlayerID);
                FC.InGame = false;

                return true;
            }
            InitHealth();

            //ZL.UpdateZone();
            //Debug.Log("Dead");

            isDead = true;
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        //healthBar.position = new Vector2(RecInitX - (maxHealth - currentHealth) , healthBar.position.y);
        if (isDead)
            return true;

        return false;
    }

    public void InitHealth()
    {
        currentHealth = maxHealth;
        _health.Value = currentHealth.ToString() + " / 100";
        healthBar.sizeDelta = new Vector2(maxHealth, healthBar.sizeDelta.y);
        //healthBar.position = new Vector2(RecInitX, healthBar.position.y);
    }

    [Command]
    void DestroyPlayer(GameObject player)
    {
        Destroy(player);
    }

    /*public void UpZone() 
    {

        //if (!isLocalPlayer)
        //    return;
        Debug.Log("moins");
        var ZL = GetComponent<ZoneLimitations>();

        currentHealth = 100;
        ZL.state--;
        ZL.UpdateZone();
    }*/


    public void KillManager(int killer, int killed, bool firstKill, bool Twollet)  // à appeler quand il y a une mecanique de kills
    {
        if (!isLocalPlayer)
            return;

        CmdKillNotification(killer, killed, firstKill, Twollet);
    }

    //On peut modifier la valeur d'un local en modiffient son player depuis le serveur.
    [Command] //Appelé par le client mais lu par le serveur
    void CmdKillNotification(int killer, int killed, bool firstKill, bool Twollet)
    {
        //propagateInfos(killer, killed);
        //Debug.Log("Killer = " + killer + " - Killed = " + killed);
        if (firstKill)
            ClientFirstKill(killer);
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == killed)
            {
                child.GetComponent<Stats>().selfDeath++;
                child.GetComponent<GameInfos>().callKda = true;
            }

            if (child.GetComponent<FullControl>().PlayerID == killer)
            {
                child.GetComponent<Stats>().selfKill++;
                child.GetComponent<GameInfos>().callKda = true;

                if (!isServer)
                    return;
                child.GetComponent<ZoneLimitations>().DownState();
                if (Twollet)
                {
                    child.GetComponent<ZoneLimitations>().DownState();
                }
                //child.GetComponent<ZoneLimitations>().UpdateZone();

            }
            //Debug.Log(child.GetComponent<FullControl>().selfNumber);
        }
    }

    [ClientRpc]
    void ClientFirstKill(int killer)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill = true;

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == killer)
            {
                //child.GetComponent<FullControl>().GotBall = true;
                if (child.GetComponent<FullControl>().isLocal)
                    child.GetComponent<Health>()._ball.Value = 1;
            }
        }
    }

    public void DisplayDamageDealed(int amount, int playerTouched)
    {
        if (playerTouched != -1)
        {
            GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

            foreach (GameObject child in characters)
            {
                if (child.GetComponent<FullControl>().PlayerID == playerTouched)
                {
                    //Debug.Log(amount + " " + playerTouched);

                    DamageDealedDisplay = Instantiate(
                            DamagePrefab,
                            new Vector3(-200, 0, 0),
                            Quaternion.identity) as Text;
                    DamageDealedDisplay.GetComponent<CanvasDistance>().LocalPlayer = gameObject;
                    DamageDealedDisplay.transform.SetParent(child.GetComponent<Health>().SelfCanvas.transform, false);
                    //DamageDealedDisplay.fontSize = 10;
                    //DamageDealedDisplay.fontSize = 10;

                    DamageDealedDisplay.text = amount.ToString();
                    DamageDealedDisplay.color = Color.yellow;


                    StartCoroutine(DownFade(DamageDealedDisplay));

                    //coroutine = ResetDamageDisplayed(child);
                    //StartCoroutine(coroutine);
                }
            }
        }
    }

    private IEnumerator DownFade(Text obj)
    {
        float elapsedTime = 0;
        obj.text = 20.ToString();
        SelfCanvas.transform.position = new Vector3(0, 1, 0);
        Vector3 InitPose = obj.GetComponent<RectTransform>().position;
        Vector3 EndPose = new Vector3(0, 1, 0);
        float time = 0.2f;
        obj.GetComponent<RectTransform>().position += new Vector3(0, 0, 0);
        while (elapsedTime < time)
        {
             obj.GetComponent<RectTransform>().position = Vector3.Lerp(InitPose + new Vector3(0, 0, 0), InitPose + new Vector3(0, -2f, 0), (elapsedTime / time)*Time.deltaTime) - (InitPose - obj.GetComponent<RectTransform>().position);
            
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        //Destroy(obj.gameObject);
    }
}