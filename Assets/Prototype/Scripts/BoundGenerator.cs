using System;
using UnityEngine;

public class BoundGenerator : MonoBehaviour
{
    private void Start()
    {
        Bounds bounds = GenerateBounds(this.transform);
        Debug.Log($"Center: {bounds.center}, Size: {bounds.size}");
    }

    public Bounds GenerateBounds(Transform transform)
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
        return newBounds;
    }
}
