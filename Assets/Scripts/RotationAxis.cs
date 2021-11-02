using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAxis : MonoBehaviour
{
    private LineRenderer _mLineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _mLineRenderer = GetComponent<LineRenderer>();
    }

    public Vector3 GetPosition()
    {
        return _mLineRenderer.GetPosition(0);
    }
    
    public Vector3 GetDirection()
    {
        return ((_mLineRenderer.GetPosition(1) - _mLineRenderer.GetPosition(0)));
    }
}
