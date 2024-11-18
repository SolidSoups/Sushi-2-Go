using System;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 90f;

    public Camera mainCamera;
    public Camera funCamera;

    public bool funMode = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            funMode = !funMode;
            mainCamera.enabled = !funMode;
            funCamera.enabled = funMode;
        }
        
        transform.position = target.position;
        Rotate();
        
        
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
