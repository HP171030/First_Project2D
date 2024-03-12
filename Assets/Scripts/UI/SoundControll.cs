using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControll : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    private void OnEnable()
    {
        bgmSlider.value = Manager.Sound.BGMVolme;
        sfxSlider.value = Manager.Sound.SFXVolme;
    }
    public void BGMController()
    {
        Manager.Sound.BGMVolme = bgmSlider.value;
        
    }

    public void SFXController()
    {
        Manager.Sound.SFXVolme = sfxSlider.value;
       
    }
}
