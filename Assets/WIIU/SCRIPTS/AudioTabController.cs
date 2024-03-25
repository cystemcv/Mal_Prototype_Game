using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioTabController : MonoBehaviour
{
    public static AudioTabController Instance;

    public SliderManager musicSlider;
    public SliderManager sfxSlider;

    private void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(musicSlider.mainSlider.value);
    }

    public void SfxVolume()
    {
        AudioManager.Instance.SFXVolume(sfxSlider.mainSlider.value);
    }

}
