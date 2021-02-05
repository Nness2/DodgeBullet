using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDistance : MonoBehaviour
{
    public GameObject LocalPlayer;
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scaleValues = GetComponent<RectTransform>().transform.localScale;
        float distance = Vector3.Distance(gameObject.transform.position, LocalPlayer.transform.position);
        GetComponent<RectTransform>().transform.localScale = new Vector3(scaleValues.x * distance, scaleValues.y * distance, scaleValues.z * distance);
    }
}
