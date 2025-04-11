using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// This is a Audio Clips Script Written by Patrick Emmons for the game "Pin Brawl" in 2024
/// </summary>
public class AudioClips : MonoBehaviour
{
    AudioSource source;
    [SerializeField] AudioMixer musicMix;
    [SerializeField] AudioMixer sfxMix;
    [SerializeField]private AudioClip hitSound;
    [SerializeField]private List<AudioClip> whiffSounds;
    [SerializeField]private AudioClip startSound;
    [SerializeField]private AudioClip shieldHit;
    [SerializeField]private AudioClip dashSound;
    [SerializeField]private AudioClip jumpSound;
    [SerializeField]private AudioClip deathSound;
    [SerializeField]private AudioClip click;

    public const string MUSIC_KEY = "MusicVol";
    public const string SFX_KEY = "SFXVol";




    private void Awake()
    {
        LoadVolume();    
    }

    public void Start()
    {
        source = this.GetComponent<AudioSource>();
    }

    public AudioClip HitSound
    {
        get { return hitSound; }
    }  


    //Sounds for getting hit
    public void PlayDamageSound()
    {
        ResetPitch();
        source.pitch = Random.Range(0.8f, 1.2f);
        Debug.Log(source.pitch);
        source.PlayOneShot(hitSound);
        
    }

    public void PlayShieldHitSound()
    {
        ResetPitch();
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(shieldHit);
        
    }

    public void PlaySwoosh()
    {
        ResetPitch();
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(whiffSounds[Random.Range(0, 3)]);
    }

    public void PlayDashSound()
    {
        ResetPitch();
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(dashSound);
    }
    

    public void PlayJumpSound()
    {
        ResetPitch();
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(jumpSound);
    }

    //Join sounds
    public void PlayStartSound()
    {
        ResetPitch();
        source.PlayOneShot(startSound);
    }

    //Menu sound
    public void ClickSound()
    {
        ResetPitch();
        source.PlayOneShot(click);
    }

    public void PlayDeathSound()
    {
        ResetPitch();
        source.volume = .75f;
        source.PlayOneShot(deathSound);
    }

    //Sets the pitch back to 1 on the audio source.
    private void ResetPitch()
    {
        source.volume = 1f;
        source.pitch = 1.0f;
    }

    void LoadVolume() //Volume saved in VolumeControl.cs
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        musicMix.SetFloat(VolumeControl.MUSIC_MIX, Mathf.Log10(musicVolume) * 20);
        sfxMix.SetFloat(VolumeControl.SFX_MIX, Mathf.Log10(sfxVolume) * 20);
        
    }


}
