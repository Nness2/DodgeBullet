using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveName : MonoBehaviour
{
    public GameObject text;
    public string PlayerName;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    
    public void GameEnter()
    {
        PlayerName = text.GetComponent<Text>().text;
        SceneManager.LoadScene("BattleMap");
    }
}
