using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<Part> partList;
    public List<RotationAxis> rAxisList;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (partList[0].IsPressed())
        {
            if (partList[0].IsCoroutineAllow())
            {
                StartCoroutine(partList[0].Fold(rAxisList[0]));
            }
        }
    }
}
