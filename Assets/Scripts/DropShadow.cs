using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    Vector3 _offset = new Vector3(.5f, -.5f);
    GameObject _shadow;
    
    [SerializeField] private Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        _shadow = new GameObject("DropShadow");
        _shadow.transform.parent = transform;
        _shadow.transform.localPosition = _offset;
        _shadow.transform.localScale = Vector3.one;
        _shadow.transform.rotation = Quaternion.identity;
        SpriteRenderer sRend = _shadow.AddComponent<SpriteRenderer>();
        sRend.sprite = GetComponent<SpriteRenderer>().sprite;
        sRend.material = material;
        sRend.sortingLayerName = "Shadow";
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _shadow.transform.localPosition = _offset;
        //_shadow.transform.localRotation = Quaternion.identity;
    }
}
