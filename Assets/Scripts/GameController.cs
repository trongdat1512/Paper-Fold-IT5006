using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] List<Part> partList;
    [SerializeField] List<FoldGroup> foldGroups;
    [SerializeField] List<Part> winCondition;

    Stack<FoldGroup> _historyStack;
    bool _coroutineAllow = true;

    // Start is called before the first frame update
    void Start()
    {
        _historyStack = new Stack<FoldGroup>();
    }

    private void Update()
    {
        if (_historyStack.Count == foldGroups.Count)
        {
            if (CheckWin())
            {
                //Win
            }
            else
            {
                if (_coroutineAllow)
                {
                    StartCoroutine(UnFoldAll());
                }
            }
        }
    }

    public List<Part> GetPartList()
    {
        return partList;
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
    
    IEnumerator UnFoldAll()
    {
        _coroutineAllow = false;
        yield return StartCoroutine(UnFoldTop());
        if (_historyStack.Count > 0)
        {
            yield return StartCoroutine(UnFoldAll());
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

    bool CheckWin()
    {
        if (winCondition.Count > 0 && winCondition.Count % 2 == 0)
        {
            int i = 0;
            bool status = false;
            while (i < winCondition.Count)
            {
                status = winCondition[i].GetSortingOrder() > winCondition[i+1].GetSortingOrder();
                i += 2;
            }
            return status;
        }
        return true;
    }
}
