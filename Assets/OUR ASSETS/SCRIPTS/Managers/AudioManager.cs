using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource cardSource;

    public List<SoundSO> musicSounds;
    public List<SoundSO> sfxSounds;
    public List<SoundSO> cardSounds;


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

        PlayMusic("UI_MainMenu");
    }

    public void Start()
    {

    }

    public void PlayMusic(string name)
    {
        SoundSO soundSO = musicSounds.Find(item => item.soundName==name);

        if (soundSO == null)
        {
            Debug.Log("Music not found");
        }
        else
        {
            musicSource.clip = soundSO.soundClip;
            musicSource.Play();
        }
    }

    public void PlaySfx(string name)
    {
        SoundSO soundSO = sfxSounds.Find(item => item.soundName == name);

        if (soundSO == null)
        {
            Debug.Log("SFX Audio not found");
        }
        else
        {
            sfxSource.PlayOneShot(soundSO.soundClip);
   
        }
    }

    public void PlayCardSound(string name)
    {
        SoundSO soundSO = cardSounds.Find(item => item.soundName == name);

        if (soundSO == null)
        {
            Debug.Log("Card Audio not found");
        }
        else
        {
            cardSource.PlayOneShot(soundSO.soundClip);

        }
    }

    public void ToggleSFX()
    {

    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
