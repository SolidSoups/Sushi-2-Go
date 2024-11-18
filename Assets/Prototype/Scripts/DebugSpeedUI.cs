using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugSpeedUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Text text;
     
    void Start()
    {
       text = GetComponent<Text>(); 
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"Speed (debug): {DifficultyController.Instance.WorldSpeed.ToString("F2")}";
    }
}
