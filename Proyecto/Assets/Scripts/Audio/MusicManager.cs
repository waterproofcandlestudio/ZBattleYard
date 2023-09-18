using System;
using System.Collections;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    AudioSource musicAudioSource;
    AudioLowPassFilter lowPassFilter;

    [SerializeField] AudioClip playingClip;

    [SerializeField] AudioClip[] music; // Music that i put in inspector that must be in the randomized playlist ingame
    [SerializeField] AudioClip[] playList;  // Ingame randomize playlist created by my with code (manages a list of non repeated songs till it ends like a normal playlist)
    //[SerializeField] SO_AudioTrack[] musicTracks;
    //[SerializeField] SO_AudioTrack[] playListTracks;
    [SerializeField] [Range(0, 1)] float fadeValue = 0.5f;
    [SerializeField] [Range(0f, 1f)] float menuMusicVolume = 0.5f;


    int playingSong;    // Playing Song general index
    float songLenght;
    bool songPaused;


    void Awake() => GetReferences();
    void Start()
    {
        InitializeVariables();
        if (music.Length != 0)  // If list has songs
        {
            CreateMusicList();  // First the playlist is created
            PlayNewSong(); // Start the playlist by the first song and when it ends, it continues with the next one
        }
    }
    void Update()
    {
        if (songPaused)
            return;

        if (musicAudioSource.isPlaying == false)
        {
            if (playingSong >= playList.Length)    // If actual song was the last of the list, create new list and start playing songs again
            {
                CreateMusicList();
                PlayNewSong();
            }
            else
                SkipForward();
        }
    }
    public void ChangeMusicVolume(float volume) => musicAudioSource.volume = volume;
    //public void LowerMenuVolumePercentage()
    //{
    //    //musicAudioSource.volume *= menuMusicVolume;
    //    musicAudioSource.volume = ((PlayerPrefs.GetFloat("musicVolume") + 80) * 0.01f) * menuMusicVolume;
    //}
    //public void ResetVolumePercentage() => musicAudioSource.volume = 1;

    public void LowPassFilter_SetActive(bool b) => lowPassFilter.enabled = b;

    public void SkipForward()
    {
        if (playingSong < playList.Length - 1)
        {
            playingSong++;
            PlayNewSong();
        }
        else
        {
            CreateMusicList();
            PlayNewSong();
        }
    }
    public void SkipBackwards()
    {
        if (playingSong >= 1)
        {
            playingSong--;
            PlayNewSong();
        }
        else
        {
            CreateMusicList();
            PlayNewSong();
        }
    }
    void UpdateTrack(int index)
    {
        musicAudioSource.clip = playList[index];
    }

    public void PlayAudio()
    {
        if (musicAudioSource.isPlaying)
            return;
        musicAudioSource.Play();
        songPaused = false;
    }
    public void PauseAudio()
    {
        musicAudioSource.Pause();
        songPaused = true;
    }
    public void StopAudio() => StartCoroutine(FadeOut());
    public float GetMenuMusicVolume() => menuMusicVolume;

    IEnumerator FadeOut()
    {
        float startVolume = musicAudioSource.volume;

        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= startVolume * Time.deltaTime / fadeValue;
            yield return null;
        }

        musicAudioSource.Stop();
        musicAudioSource.volume = startVolume;
        UpdateTrack(playingSong);
        SkipForward();
    }
    IEnumerator FadeIn()
    {
        float startVolume = musicAudioSource.volume;
        UpdateTrack(playingSong);
        musicAudioSource.volume = 0;
        musicAudioSource.Play();

        while (musicAudioSource.volume < startVolume)
        {
            musicAudioSource.volume += startVolume * Time.deltaTime / fadeValue;
            yield return null;
        }

        musicAudioSource.volume = startVolume;
        playingClip = playList[playingSong];
        StopAudio();
    }

    void CreateMusicList()  // Creates randomized music list with music in "AudioClip[] music" list
    {
        playingSong = 0;
        Array.Clear(playList, 0, playList.Length);  // Clear content if it is re-creating itself again after getting to final song of the list

        int songToInclude = Random.Range(0, music.Length);

        for (int i = 0; i < music.Length; i++)  // Go over music songs array
        {
            while (SongInPlaylist(music[songToInclude]))
                songToInclude = Random.Range(0, music.Length);

            playList[i] = music[songToInclude];
        }        
    }

    bool SongInPlaylist(AudioClip clip) // Go over playlist songs array
    {
        for (int i = 0; i < playList.Length; i++)
            if (playList[i] == clip)
                return true;

        return false;
    }

    void PlayNewSong()
    {
        AudioClip song = playList[playingSong];
        musicAudioSource.clip = song;
        musicAudioSource.Play();
        songLenght = song.length;
    }

    void GetReferences()
    {
        musicAudioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    void InitializeVariables()
    {
        Array.Resize(ref playList, music.Length);   // Resize playlist amount of songs to the amount of songs i included to the game
    }
}
