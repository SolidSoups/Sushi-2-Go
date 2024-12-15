using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_DebugController : MonoBehaviour
{
  public static UI_DebugController Instance;
  
  [Header("References")]
  [SerializeField] private TextMeshProUGUI textMesh;

  private void Awake()
  {
    Instance      = this;
    textMesh.text = "";
  }

  private void Start()
  {
    StartCoroutine(DisplayMessages());
  }

  private Queue<string> _debugQueue = new Queue<string>(); 
  public void AddToQueue(string message)
  {
    _debugQueue.Enqueue(message);
    if (_debugQueue.Count > 5)
    {
      _debugQueue.Dequeue();
    }
  }

  private IEnumerator DisplayMessages()
  {
    while (true)
    {
      if (_debugQueue.Count != 0)
      {
        string message = _debugQueue.Dequeue();
        textMesh.text = message;
        yield return new WaitForSeconds(1);
        textMesh.text = "";
      }
      else
        yield return null;
    }
  } 
}
