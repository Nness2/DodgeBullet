using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WriteName : MonoBehaviour
{

    private void Start()
    {
        SaveName();
    }

    public void SaveName()
    {
        if (GameObject.FindGameObjectWithTag("name") != null)
        {
            var name = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
            string path = Application.dataPath + "/DataFiles/selfName.txt";

            File.WriteAllText(path, string.Empty);
            TextWriter tw = new StreamWriter(path, true);
            tw.WriteLine(name);
            tw.Close();
        }

    }

}
