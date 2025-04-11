using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// This is a Volume Control Script Written by Patrick Emmons for the game "Pin Brawl" in 2024
/// </summary>
public class VolumeControl : MonoBehaviour
{
    [SerializeField] AudioMixer musicMix;
    [SerializeField] AudioMixer sfxMix;
    //[SerializeField] Slider musicSlider;
    //[SerializeField] Slider sfxSlider;

   public const string MUSIC_MIX = "MusicVol";
   public const string SFX_MIX = "SFXVol";

    private void Awake()
    {
        //musicSlider.onValueChanged.AddListener(SetMusicVolume);
        //sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Start()
    {
        //musicSlider.value = PlayerPrefs.GetFloat(AudioClips.MUSIC_KEY, 1f);
        //sfxSlider.value = PlayerPrefs.GetFloat(AudioClips.SFX_KEY, 1f);
    }

    private void OnDisable() //Saves the audio settings
    {
        //PlayerPrefs.SetFloat(AudioClips.MUSIC_KEY, musicSlider.value);
        //PlayerPrefs.SetFloat(AudioClips.SFX_KEY, sfxSlider.value);
        
    }
    void SetMusicVolume(float val)
    {
        musicMix.SetFloat(MUSIC_MIX, Mathf.Log10(val) * 20);
    }

    void SetSFXVolume(float val)
    {
        sfxMix.SetFloat(SFX_MIX, Mathf.Log10(val) * 20);
    }

  
}
