using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /*public List<Part> partList;
    public List<RotationAxis> rAxisList;*/
    [SerializeField] List<FoldGroup> foldGroups;

    Stack<FoldGroup> _historyStack;
    bool _coroutineAllow = true;

    // Start is called before the first frame update
    void Start()
    {
        _historyStack = new Stack<FoldGroup>();   
    }

    private void Update()
    {
        if(Input.GetKeyDown("backspace"))
        {
            if (_historyStack.Count != 0 && !_historyStack.Peek().IsFolding())
            {
                if (_coroutineAllow)
                {
                    StartCoroutine(UnFoldTop());
                }
            }
        }
    }

    IEnumerator UnFoldTop()
    {
        _coroutineAllow = false;
        yield return StartCoroutine(_historyStack.Peek().UnFold());
        _historyStack.Pop();
        _coroutineAllow = true;
    }

    IEnumerator UnFoldToPart(Part part)
    {
        _coroutineAllow = false;
        bool finished = _historyStack.Count > 0 ?_historyStack.Peek().foldParts.Contains(part) : true;
        yield return StartCoroutine(UnFoldTop());
        if (_historyStack.Count > 0 && !finished)
        {
            yield return StartCoroutine(UnFoldToPart(part));
        }
        _coroutineAllow = true;
    }
    
    IEnumerator Fold(FoldGroup foldGroup)
    {
        _coroutineAllow = false;
        yield return StartCoroutine(foldGroup.Fold()); 
        _historyStack.Push(foldGroup);
        _coroutineAllow = true;
    }
    public void HandlePress(Part part)
    {
        FoldGroup foldGroup = foldGroups.Find(x => x.foldParts.Contains(part) && x.GetStatus() == 0);
        if (foldGroup)
        {
            if (_coroutineAllow)
            {
                StartCoroutine(Fold(foldGroup));
            }
        }
        // Find in history stack
        else
        {
            if (_historyStack.Count > 0)
            {
                if (_coroutineAllow)
                {
                    StartCoroutine(UnFoldToPart(part));
                }
            }  
        }
    }
}
