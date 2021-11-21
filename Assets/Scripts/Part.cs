using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    [SerializeField] Sprite front, back;
    GameObject _mask;
    SpriteRenderer _rend;
    PolygonCollider2D _cld;
    GameController _controller;
    Transform _myTransform;
    
    bool _coroutineAllow, _isFaceUp, _halfFolded;
    
    // Start is called before the first frame update
    void Start()
    {
        _myTransform = GetComponent<Transform>();
        _rend = GetComponent<SpriteRenderer>();
        _cld = GetComponent<PolygonCollider2D>();
        _controller = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        _mask = new GameObject("Mask");
        _mask.transform.parent = _myTransform;
        _mask.AddComponent<SpriteMask>();
        _mask.GetComponent<SpriteMask>().sprite = _rend.sprite;
        _mask.transform.localPosition = Vector3.zero;
        _mask.transform.localScale = Vector3.one;
        _mask.transform.localRotation = Quaternion.identity;
        _rend.sprite = back;
        _coroutineAllow = true;
        _isFaceUp = false;
        _halfFolded = false;
    }

    public Sprite GetSprite()
    {
        return front;
    }
    
    public void SetSprite(Sprite sprite)
    {
        front = sprite;
    }

    void OnMouseDown()
    {
        _controller.HandlePress(this);
    }

    public bool IsCoroutineAllow()
    {
        return _coroutineAllow;
    }

    public bool IsHalfFolded()
    {
        return _halfFolded;
    }
    
    public int GetSortingOrder()
    {
        return _rend.sortingOrder;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        _rend.sortingOrder = sortingOrder;
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
                _halfFolded = true;
                if (_isFaceUp)
                {
                    _rend.sprite = back;
                }
                else
                {
                    _rend.sprite = front;
                }
            }

            yield return new WaitForSeconds(0.005f);
        }
        _myTransform.position = new Vector3(_myTransform.position.x, _myTransform.position.y, 0);
        //_myTransform.position = new Vector3(_myTransform.position.x, _myTransform.position.y, -_rend.sortingOrder);
        _coroutineAllow = true;
        _halfFolded = false;
        _isFaceUp = !_isFaceUp;
    }
}
