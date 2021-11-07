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
    [SerializeField] int status;
    [SerializeField] List<FoldGroup> prevFoldGroups, nextFoldGroups;

    private bool _isFolding = false;
    
    public int GetStatus()
    {
        return status;
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
                foldGroup.status++;
            }
            
            foreach (var foldGroup in nextFoldGroups)
            {
                foldGroup.status = 0;
            }
            
            status = 1;
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
                foldGroup.status--;
            }
            
            foreach (var foldGroup in nextFoldGroups)
            {
                foldGroup.status = -1;
            }
            
            status = 0;
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
            foldPart.SetSortingOrder(distinctSortingOrderList[index] + (status == 0 ? changeVal : -changeVal));
        }
    }
}
