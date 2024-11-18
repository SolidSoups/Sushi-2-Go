using System;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class AlignBottomWithYPlane : MonoBehaviour
{
    private WorldMover _worldMover;
    private Bounds _myBounds;

    private void OnDrawGizmos()
    {
        AlignToYPlane();
    }

    private void GenerateMyBounds()
    {
        Vector3 center = Vector3.zero;
        foreach (Transform child in transform)
        {
            center += child.gameObject.GetComponent<Renderer>().bounds.center;
        }
        center /= transform.childCount;
        Bounds newBounds = new Bounds(center, Vector3.zero);
        foreach (Transform child in transform)
        {
            newBounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        newBounds.Encapsulate(transform.GetComponent<Renderer>().bounds);
        _myBounds = newBounds;
    }
    
    private void AlignToYPlane()
    {
        GenerateMyBounds();

        float yHeight = 0;
        Vector3 pos = transform.position;
        pos.y = yHeight + _myBounds.extents.y;
        transform.position = pos;
    }  
}
