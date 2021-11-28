using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Button hapticToggle, musicToggle, soundToggle;
    [SerializeField] Sprite hapticEnable, hapticDisable, musicEnable, musicDisable, soundEnable, soundDisable;

    bool _isHapticEnable, _isMusicEnable, _isSoundEnable;
    // Start is called before the first frame update
    void Start()
    {
        _isHapticEnable = true;
        _isMusicEnable = true;
        _isSoundEnable = true;
        hapticToggle.image.sprite = _isHapticEnable ? hapticEnable : hapticDisable;
        musicToggle.image.sprite = _isMusicEnable ? musicEnable : musicDisable;
        soundToggle.image.sprite = _isSoundEnable ? soundEnable : soundDisable;
    }

    public void HapticToggle()
    {
        _isHapticEnable = !_isHapticEnable;
        hapticToggle.image.sprite = _isHapticEnable ? hapticEnable : hapticDisable;
    }
    
    public void MusicToggle()
    {
        _isMusicEnable = !_isMusicEnable;
        musicToggle.image.sprite = _isMusicEnable ? musicEnable : musicDisable;
    }
    
    public void SoundToggle()
    {
        _isSoundEnable = !_isSoundEnable;
        soundToggle.image.sprite = _isSoundEnable ? soundEnable : soundDisable;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
