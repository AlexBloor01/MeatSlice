using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Manager")]

    #region Variables
    public static AudioManager iAudioManager; //Access the AudioManager with AudioManager.iAudioManager...
    private float masterVolume = 1f; //Current Master Volume.
    private float musicVolume = 0.7f; //Current Music Volume.
    private float SFXVolume = 1f; //Current SFX Volume.
    private float whooshVolume = 1f; //Current Whoosh Volume.

    public AudioSource SFXAudioSource; //Sound Effect Audiosource.
    public AudioSource WhooshAudioSource; //Whoosh Audiosource.
    public AudioClip[] slice; //Slice Sounds for slicing food button.
    public AudioClip[] squelch; //Squelch Sounds for sliced food slapping things as it falls.
    public AudioClip whoosh; //Whooshing Sound for holder going back and forth.


    Coroutine musicCoroutine; //Music Coroutine.
    public AudioSource musicAudioSource; //Music Audiosource.
    public AudioClip[] musicPlaylist; //Place all music clips in order here.
    private int musicIndex = -1; //Must start at -1. Controls position in the music playlist.
    #endregion

    #region Setup
    private void Awake()
    {
        SetupVariables();
    }

    //Sets up script variables.
    private void SetupVariables()
    {
        musicIndex = 0;
        iAudioManager = this;
        if (musicPlaylist.Length > 0)
        {
            PlayMusic();
        }

    }

    private void Start()
    {
        LoadSettings();
    }

    //Loads all presets required for game audio.
    //Also saves if the game has been played before or not.
    //Must be played on start.
    private void LoadSettings()
    {
        if (GameManager.playedBefore)
        {
            Debug.Log("Played Before");
            musicAudioSource.mute = PlayerPrefs.GetInt("Mute_Music") == 1;
            SFXAudioSource.mute = PlayerPrefs.GetInt("Mute_SFX") == 1;
            WhooshAudioSource.mute = SFXAudioSource.mute;

            masterVolume = PlayerPrefs.GetFloat("Master_Volume");
            SFXVolume = PlayerPrefs.GetFloat("SFX_Volume");
            musicVolume = PlayerPrefs.GetFloat("Music_Volume");
        }

        UpdateAllVolume();
    }

    #endregion

    //Play Music with this Coroutine, make sure to use PlayMusic() to run this so 
    private IEnumerator PlayPlaylist()
    {
        while (true)
        {
            musicIndex++;
            if (musicIndex >= musicPlaylist.Length) musicIndex = 0;
            if (musicIndex < 0) musicIndex = musicPlaylist.Length - 1;
            musicAudioSource.clip = musicPlaylist[musicIndex];
            musicAudioSource.Play();
            //Wait for song to end.
            yield return new WaitForSecondsRealtime(musicPlaylist[musicIndex].length);
        }
    }

    private void PlayMusic()
    {
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }

        musicCoroutine = StartCoroutine(PlayPlaylist());
    }

    #region Music Controls

    public void NextSong()
    {
        PlayMusic();
    }

    public void PreviousSong()
    {
        musicIndex -= 2;
        PlayMusic();
    }

    public void MuteMusic()
    {
        musicAudioSource.mute = !musicAudioSource.mute;
        PlayerPrefs.SetInt("Mute_Music", musicAudioSource.mute ? 1 : 0);
    }
    #endregion

    #region SFX Controls
    public void PlayOneShot(AudioClip clip)
    {
        SFXAudioSource.PlayOneShot(clip);
    }

    public void MuteSFX()
    {
        SFXAudioSource.mute = !SFXAudioSource.mute;
        WhooshAudioSource.mute = SFXAudioSource.mute;
        PlayerPrefs.SetInt("Mute_SFX", musicAudioSource.mute ? 1 : 0);
    }
    #endregion

    #region Volume Controls

    public void SFXVolumeControl(float volume)
    {
        SFXVolume = volume;
        UpdateAllVolume();
    }
    public void MusicVolumeControl(float volume)
    {
        musicVolume = volume;
        UpdateAllVolume();
    }
    public void MasterVolumeControl(float volume)
    {
        masterVolume = volume;
        UpdateAllVolume();
    }

    public void WhooshVolumeControl(float volume)
    {
        whooshVolume = volume;
        WhooshAudioSource.volume = whooshVolume * SFXVolume * masterVolume;
    }

    private void UpdateAllVolume()
    {
        musicAudioSource.volume = musicVolume * masterVolume;
        SFXAudioSource.volume = SFXVolume * masterVolume;

        PlayerPrefs.SetFloat("Master_Volume", masterVolume);
        PlayerPrefs.SetFloat("SFX_Volume", SFXVolume);
        PlayerPrefs.SetFloat("Music_Volume", musicVolume);
    }
    #endregion

}
