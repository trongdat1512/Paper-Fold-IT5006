using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject _lv;
    GameController _gameController;
    Transform _lvTrans;
    [SerializeField] Text levelLabel;
    [SerializeField] int level;
    bool _coroutineAllow = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Resources.Load("Prefabs/Level/Lv"+ level) != null)
        {
            _lv = Instantiate(Resources.Load("Prefabs/Level/Lv"+ level)) as GameObject;
            _gameController = _lv.GetComponentInChildren<GameController>();
            _lvTrans = _lv.transform;
        }
        else
        {
            Debug.Log("Out of levels!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        levelLabel.text = "Level " + level;
        if (_gameController.IsPassed())
        {
            if (_coroutineAllow)
            {
                _coroutineAllow = false;
                NextLevelAnim1();
            }
        }
    }

    void NextLevelAnim1()
    {
        _lvTrans.LeanMoveY(.3f, .2f).setOnComplete(NextLevelAnim2);
    }

    void NextLevelAnim2()
    {
        _lvTrans.LeanMoveY(-10, .5f);
        _lvTrans.LeanRotateZ(180, .5f).setOnComplete(NextLevel);
    }
    
    void NextLevel()
    {
        Destroy(_lvTrans.gameObject);
        level++;
        if (level > 2) level = 1;
        if (Resources.Load("Prefabs/Level/Lv" + level) != null)
        {
            _lv = Instantiate(Resources.Load("Prefabs/Level/Lv"+ level)) as GameObject;
            _gameController = _lv.GetComponentInChildren<GameController>();
            _lvTrans = _lv.transform;
            _coroutineAllow = true;
        }
        else
        {
            Debug.Log("Out of levels!");
        }
    }
}
