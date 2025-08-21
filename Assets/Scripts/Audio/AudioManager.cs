using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    #region Audio Manager
    [Header("References")]
    public static AudioManager iAudioManager; // Access the AudioManager with AudioManager.iAudioManager...
    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SFXSlider;

    [Header("Volume")]
    private float masterVolume = 1f; // Current Master Volume.
    private float musicVolume = 0.5f; // Starting Music Volume.
    private float SFXVolume = 1f; // Current SFX Volume.
    private float whooshVolume = 1f; // Current Whoosh Volume.

    [Header("Audio Source")]
    public AudioSource SFXAudioSource; // Sound Effect Audiosource.
    public AudioSource MusicAudioSource; // Music Audiosource.
    public AudioSource WhooshAudioSource; // Whoosh Audiosource.

    [Header("AudioClip")]
    public AudioClip Whoosh; // Whooshing Sound for holder going back and forth.

    [Header("AudioClip Array")]
    public AudioClip[] CloudPop; // Popping sound of transition loading clouds.
    public AudioClip[] Chop; // Slice Sounds for slicing food button.
    public AudioClip[] ScorePoint; // Squelch Sounds for sliced food slapping things as it falls.
    public AudioClip[] Squelch; // Squelch Sounds for sliced food slapping things as it falls.
    public AudioClip[] DeathWhistle; // Death Whistle as things fall to oblivion.


    [Header("Sound Array Index")]
    private int previousPlayedSquaelchIndex = -1; // What was the previous Cloud Pop Index?
    private int previousPlayedCloudPopIndex = -1; // What was the previous Cloud Pop Index?
    private int previousPlayedSliceIndex = -1; // What was the previous Cloud Pop Index?
    private int previousPlayedDeathWhistleIndex = -1; // What was the previous Cloud Pop Index?
    private int previousPlayedScorePointIndex = -1; // What was the previous Cloud Pop Index?


    [Header("Sound Audio Preference")]
    private float squelchVolume = 0.5f; // Volume of squelch.

    private float scorePointVolume = 0.5f; // Volume of Score Point.
    private float overallScorePointVolume
    {
        get
        {
            return SFXVolume * scorePointVolume;
        }
    }
    private float deathWhistleVolume
    {
        get
        {
            return SFXVolume * Random.Range(0.4f, 0.9f);
        }
    }

    [Header("Music")]
    Coroutine musicCoroutine; // Music Coroutine.
    public AudioClip[] MusicPlaylist; // Place all music clips in order here.
    private int musicIndex = -1; // Must start at -1. Controls position in the music playlist.

    [Header("Load Setting Strings")]
    private readonly string loadSetting_MuteMusic = "Mute_Music";
    private readonly string loadSetting_MuteSFX = "Mute_SFX";
    private readonly string loadSetting_MasterVolume = "Master_Volume";
    private readonly string loadSetting_SFXVolume = "SFX_Volume";
    private readonly string loadSetting_MusicVolume = "Music_Volume";
    #endregion

    #region Setup
    private void Awake()
    {
        SetupVariables();
    }

    // Sets up script variables.
    private void SetupVariables()
    {
        musicIndex = 0;
        iAudioManager = this;
        if (MusicPlaylist.Length > 0)
        {
            PlayMusic();
        }

    }

    private void Start()
    {
        LoadSettings();
    }

    // Loads all presets required for game audio.
    // Also saves if the game has been played before or not.
    // Must be played on start.
    private void LoadSettings()
    {
        if (GameManager.playedBefore)
        {
            Debug.Log("Game Played Before");
            MusicAudioSource.mute = PlayerPrefs.GetInt(loadSetting_MuteMusic) == 1;
            SFXAudioSource.mute = PlayerPrefs.GetInt(loadSetting_MuteSFX) == 1;
            WhooshAudioSource.mute = SFXAudioSource.mute;

            masterVolume = PlayerPrefs.GetFloat(loadSetting_MasterVolume);
            SFXVolume = PlayerPrefs.GetFloat(loadSetting_SFXVolume);
            musicVolume = PlayerPrefs.GetFloat(loadSetting_MusicVolume);
        }

        LoadVolumeSliders();
    }

    #endregion

    #region Music Controls
    private void PlayMusic()
    {
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }

        musicCoroutine = StartCoroutine(PlayCurrentMusicIndexInPlaylist());
    }

    // Play Music with this Coroutine, make sure to use PlayMusic() to run this
    private IEnumerator PlayCurrentMusicIndexInPlaylist()
    {
        MusicAudioSource.clip = MusicPlaylist[musicIndex];
        MusicAudioSource.Play();
        // Wait for song to end.
        yield return new WaitForSecondsRealtime(MusicPlaylist[musicIndex].length);
        NextSong();
    }

    public void NextSong()
    {
        musicIndex++;
        if (musicIndex >= MusicPlaylist.Length) musicIndex = 0;
        if (musicIndex < 0) musicIndex = MusicPlaylist.Length - 1;
        PlayMusic();
    }

    public void PreviousSong()
    {
        musicIndex -= 2;
        PlayMusic();
    }

    public void MuteMusic()
    {
        MusicAudioSource.mute = !MusicAudioSource.mute;
        PlayerPrefs.SetInt(loadSetting_MuteMusic, MusicAudioSource.mute ? 1 : 0);
        if (MusicAudioSource.mute)
            NextSong();
    }
    #endregion


    #region Play Sounds 

    // Play transition cloud pop once.
    public void PlayCloudPop()
    {
        PlayAudioClipIndex(CloudPop, ref previousPlayedCloudPopIndex);
    }

    // Play during slice.
    public void PlayChop()
    {
        PlayAudioClipIndex(Chop, ref previousPlayedSliceIndex);
    }

    public void PlayScorePoint()
    {
        PlayAudioClipIndex(ScorePoint, ref previousPlayedScorePointIndex, overallScorePointVolume);
    }

    // Play after slice.
    public void PlaySquelch()
    {
        PlayAudioClipIndex(Squelch, ref previousPlayedSquaelchIndex, squelchVolume);
    }
    // Play after slice.
    public void PlaySpawnBurger()
    {
        PlayAudioClipIndex(Squelch, ref previousPlayedSquaelchIndex);
    }

    // Play after Falling into death block and having markedfordeath script applied.
    public void PlayDeathWhistle()
    {
        PlayAudioClipIndex(DeathWhistle, ref previousPlayedDeathWhistleIndex, deathWhistleVolume);
    }

    // Play an audio clip index of an audio clip array, pick one at random without picking the previous index.
    private void PlayAudioClipIndex(AudioClip[] audioClips, ref int previousSoundIndex)
    {
        if (audioClips.Length == 0)
            return;

        int newIndex = Random.Range(0, audioClips.Length);
        if (audioClips.Length > 1)
            if (newIndex == previousSoundIndex)
                newIndex = Random.Range(0, audioClips.Length);

        if (audioClips[newIndex] != null)
            PlayOneShot(audioClips[newIndex]);
        previousSoundIndex = newIndex;
    }

    // For extra volume control.
    private void PlayAudioClipIndex(AudioClip[] audioClips, ref int previousSoundIndex, float tempVol)
    {
        if (audioClips.Length == 0)
            return;

        int newIndex = Random.Range(0, audioClips.Length);
        if (audioClips.Length > 1)
            if (newIndex == previousSoundIndex)
                newIndex = Random.Range(0, audioClips.Length);

        if (audioClips[newIndex] != null)
            PlayOneShot(audioClips[newIndex], tempVol);
        previousSoundIndex = newIndex;
    }


    #endregion

    #region SFX Controls
    public void PlayOneShot(AudioClip clip)
    {
        SFXAudioSource.PlayOneShot(clip);
    }
    public void PlayOneShot(AudioClip clip, float newVolume)
    {
        newVolume = Mathf.Clamp(newVolume, 0, SFXAudioSource.volume);
        SFXAudioSource.PlayOneShot(clip, newVolume);
    }

    public void MuteSFX()
    {
        SFXAudioSource.mute = !SFXAudioSource.mute;
        WhooshAudioSource.mute = SFXAudioSource.mute;
        PlayerPrefs.SetInt(loadSetting_MuteSFX, MusicAudioSource.mute ? 1 : 0);
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
        if (volume > musicVolume && musicVolume == 0)
            MuteMusic();
        musicVolume = volume;
        if (musicVolume <= 0)
            MuteMusic();

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
        MusicAudioSource.volume = musicVolume * masterVolume;
        SFXAudioSource.volume = SFXVolume * masterVolume;

        PlayerPrefs.SetFloat(loadSetting_MasterVolume, masterVolume);
        PlayerPrefs.SetFloat(loadSetting_MusicVolume, musicVolume);
        PlayerPrefs.SetFloat(loadSetting_SFXVolume, SFXVolume);
    }

    private void LoadVolumeSliders()
    {
        UpdateAllVolume();
        MasterSlider.value = masterVolume;
        MusicSlider.value = musicVolume;
        SFXSlider.value = SFXVolume;
    }
    #endregion

}
