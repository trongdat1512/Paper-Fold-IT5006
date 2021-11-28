using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoldGroup : MonoBehaviour
{
    public List<Part> foldParts;
    [SerializeField] RotationAxis rAxis;
    //status -1: Not _isFolding; 0: _isFolding; 1: Folded & Un_isFolding; >1: Folded & Not un_isFolding
    int _status;
    [SerializeField] List<FoldGroup> prevFoldGroups, nextFoldGroups;

    private bool _isFolding;

    private void Start()
    {
        _status = prevFoldGroups.Count > 0 ? -1 : 0;
    }

    private void Update()
    {
        rAxis.SetVisible(_status == 0);
    }

    public int GetStatus()
    {
        return _status;
    }

    public bool IsFolding()
    {
        return _isFolding;
    }

    public IEnumerator Fold()
    {
        foreach (var foldPart in foldParts)
        {
            if (!foldPart.IsCoroutineAllow())
            {
                _isFolding = true;
                yield break;
            }
        }
        if (!_isFolding)
        {
            foreach (var foldPart in foldParts)
            {
                if (foldPart.IsCoroutineAllow())
                {
                    StartCoroutine(foldPart.Fold(rAxis));
                }
            }

            bool isHalfFolded = false;
            while (!isHalfFolded)
            {
                isHalfFolded = true;
                foreach (var foldPart in foldParts) {
                    if (!foldPart.IsHalfFolded()) isHalfFolded = false;
                }
                yield return null;
            }
            ChangeSortingOrder();
            
            bool isPartFolding = true;
            while (isPartFolding) {
                isPartFolding = false;
                foreach (var foldPart in foldParts) {
                    if (!foldPart.IsCoroutineAllow()) isPartFolding = true;
                }
                yield return null;
            }
            
            foreach (var foldGroup in prevFoldGroups)
            {
                foldGroup._status++;
            }
            
            foreach (var foldGroup in nextFoldGroups)
            {
                foldGroup._status = 0;
            }
            
            _status = 1;
            _isFolding = false;
            yield return null;
        }
    }

    public IEnumerator UnFold()
    {
        foreach (var foldPart in foldParts)
        {
            if (!foldPart.IsCoroutineAllow())
            {
                _isFolding = true;
                yield break;
            }
        }
        if (!_isFolding)
        {
            foreach (var foldPart in foldParts)
            {
                if (foldPart.IsCoroutineAllow())
                {
                    StartCoroutine(foldPart.Fold(rAxis));
                }
            }
            
            bool isHalfFolded = false;
            while (!isHalfFolded)
            {
                isHalfFolded = true;
                foreach (var foldPart in foldParts) {
                    if (!foldPart.IsHalfFolded()) isHalfFolded = false;
                }
                yield return null;
            }
            ChangeSortingOrder();
            
            bool isPartFolding = true;
            while (isPartFolding) {
                isPartFolding = false;
                foreach (var foldPart in foldParts) {
                    if (!foldPart.IsCoroutineAllow()) isPartFolding = true;
                }
                yield return null;
            }
            
            foreach (var foldGroup in prevFoldGroups)
            {
                foldGroup._status--;
            }
            
            foreach (var foldGroup in nextFoldGroups)
            {
                foldGroup._status = -1;
            }
            
            _status = 0;
            _isFolding = false;
            yield return null;
        }
    }

    // Reverse sorting order of fold part group & change it after fold/unfold
    void ChangeSortingOrder()
    {
        List<int> sortingOrderList = new List<int>();
        List<int> distinctSortingOrderList = new List<int>();
        
        foreach (var foldPart in foldParts)
        {
            sortingOrderList.Add(foldPart.GetSortingOrder());
        }
        sortingOrderList.Sort();
        distinctSortingOrderList = sortingOrderList.Distinct().ToList();
        foreach (var foldPart in foldParts)
        {
            int index = distinctSortingOrderList.Count - 1 -
                        distinctSortingOrderList.IndexOf(foldPart.GetSortingOrder());
            int changeVal = distinctSortingOrderList.Max() - distinctSortingOrderList.Min() + 1;
            foldPart.SetSortingOrder(distinctSortingOrderList[index] + (_status == 0 ? changeVal : -changeVal));
        }
    }
}
