using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    private SpriteRenderer _rend;
    private PolygonCollider2D _cld;
    
    public Sprite front, back;

    private bool _coroutineAllow, _isFaceUp, _isPressed;

    private Transform _myTransform;
    // Start is called before the first frame update
    void Start()
    {
        _myTransform = GetComponent<Transform>();
        _rend = GetComponent<SpriteRenderer>();
        _cld = GetComponent<PolygonCollider2D>();
        _rend.sprite = back;
        _coroutineAllow = true;
        _isFaceUp = false;
        _isPressed = false;
    }

    private void Update()
    {
        // Debug log
        if (Input.GetKeyDown("space"))
        {
            /*Debug.Log("from: " + rAxis.GetDirection());
            Debug.Log("to: " + (_mMyTransform.position - rAxis.GetPosition()));
            Debug.Log("angle: " + Vector3.SignedAngle(rAxis.GetDirection(), _mMyTransform.position - rAxis.GetPosition(), Vector3.forward));*/
        }
    }

    void OnMouseDown()
    {
        _isPressed = true;
        Debug.Log(gameObject.name);
    }

    void OnMouseUp()
    {
        _isPressed = false;
    }

    public bool IsPressed()
    {
        return _isPressed;
    }

    public bool IsCoroutineAllow()
    {
        return _coroutineAllow;
    }
    public IEnumerator Fold(RotationAxis rotationAxis)
    {
        //_myTransform.position = new Vector3(_myTransform.position.x, _myTransform.position.y, 0);
        float angle = Vector3.SignedAngle(rotationAxis.GetDirection(), _cld.bounds.center - rotationAxis.GetPosition(),
            Vector3.back);
        float signed = angle / Math.Abs(angle);
        _coroutineAllow = false;
        
        for (int i = 1; i <= 18; i++)
        {
            _myTransform.RotateAround(rotationAxis.GetPosition(), rotationAxis.GetDirection(), 10*signed);
            if (i == 9)
            {
                if (_isFaceUp)
                {
                    _rend.sortingOrder--;
                    _rend.sprite = back;
                }
                else
                {
                    _rend.sortingOrder++;
                    _rend.sprite = front;
                }
            }

            yield return new WaitForSeconds(0.01f);
        }
        _myTransform.position = new Vector3(_myTransform.position.x, _myTransform.position.y, 0);
        //_myTransform.position = new Vector3(_myTransform.position.x, _myTransform.position.y, -_rend.sortingOrder);
        _coroutineAllow = true;
        _isFaceUp = !_isFaceUp;
    }
}
