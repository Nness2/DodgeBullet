using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class SaveName : MonoBehaviour
{
    public Text textName;
    public InputField InputText;
    public string PlayerName;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        ReadName();
    }

    public void GameEnter()
    {
        string name = textName.GetComponent<Text>().text;
        if (name.Length > 0)
        {
            PlayerName = name;
            SceneManager.LoadScene("BattleMap");
        }

    }
    
    void ReadName()
    {
        string path = "Assets/DataFiles/selfName.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string firstLine = reader.ReadLine();
        if (firstLine == null) firstLine = "";
        Debug.Log(firstLine);
        InputText.text = firstLine;
        reader.Close();
    }
}
    
