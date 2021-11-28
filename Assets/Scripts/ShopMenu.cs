using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] Transform box;
    [SerializeField] CanvasGroup background;

    void OnEnable()
    {
        background.alpha = 0;
        background.LeanAlpha(1, .25f);
        
        box.localPosition = new Vector2(-Screen.width, -Screen.height);
        box.LeanMoveLocalX(0, .25f).setEaseOutExpo().delay = .1f;
        box.LeanMoveLocalY(0, .25f).setEaseOutExpo().delay = .1f;
    }

    public void CloseDialog()
    {
        background.LeanAlpha(0, .25f);
        box.LeanMove(new Vector2(-Screen.width, -Screen.height/2), .25f).setEaseOutExpo().setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        gameObject.SetActive(false);
    }
}
