using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAxis : MonoBehaviour
{
    [SerializeField] GameObject dashedLine;
    
    LineRenderer _lineRend; 
    GameObject _dashLineClone;
    SpriteRenderer _dashedLineRend;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        _lineRend = GetComponent<LineRenderer>();
        float angle = Vector3.SignedAngle(Vector3.up, GetDirection(), Vector3.back);
        _dashLineClone = Instantiate(dashedLine, (_lineRend.GetPosition(0) + _lineRend.GetPosition(1)) / 2,
            Quaternion.Euler(0, 0, 90 - angle));
        _dashLineClone.transform.parent = transform;
        _dashedLineRend = _dashLineClone.GetComponent<SpriteRenderer>();
    }

    public void SetVisible(bool visible)
    {
        _dashedLineRend.enabled = visible;
    }
    public Vector3 GetPosition()
    {
        return _lineRend.GetPosition(0);
    }
    
    public Vector3 GetDirection()
    {
        return ((_lineRend.GetPosition(1) - _lineRend.GetPosition(0)));
    }
}
