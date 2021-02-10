using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DodgeBullet;

public class BallPowerManager : MonoBehaviour
{
    public const int maxPower = 100;
    public int currentPower = maxPower;

    public RectTransform PowerBar;
    float RecInitY;
    [SerializeField] private IntVariable _ballPower;
    private float floatPower;

    void Start()
    {
        RecInitY = PowerBar.position.y;
        floatPower = 100;
    }

    // Update is called once per frame
    void Update()
    {
        var FCS = GetComponent<FullControl>();
        if (Input.GetMouseButton(1))// && FCS.GotBall)// && InGame && !dead)
        {
            floatPower -= 100 * Time.deltaTime;
            _ballPower.Value = Mathf.RoundToInt(floatPower);
            if (_ballPower.Value <= 0)
            {
                floatPower = 100;
                PowerBar.position = new Vector3(PowerBar.position.x, RecInitY, PowerBar.position.z);

            }
            PowerBar.sizeDelta = new Vector2(_ballPower.Value, PowerBar.sizeDelta.y);
            PowerBar.position = new Vector3(PowerBar.position.x, RecInitY - (maxPower - _ballPower.Value) * 0.75f, PowerBar.position.z);
        }

    }
}
