using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AudioManager : MonoBehaviour
{
    static AudioSource uiSource;

    //[SerializeField] AudioClip exitMenu_Sound;
    //[SerializeField] AudioClip enterMenu_Sound;
    [SerializeField] AudioClip buttonHover_Sound;
    [SerializeField] AudioClip buttonPress_Sound;
    [SerializeField] AudioClip enterMenu_Sound;

    void Awake() => GetReferences();


    public static void PlaySound(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void Play_EnterMenu_Sound()
    {
        uiSource.PlayOneShot(enterMenu_Sound);
    }
    public void Play_ButtonHover_Sound()
    {
        uiSource.PlayOneShot(buttonHover_Sound);
    }
    public void Play_ButtonPress_Sound()
    {
        uiSource.PlayOneShot(buttonPress_Sound);
    }
    public static void PlayButtonSound(AudioClip clip) => uiSource.PlayOneShot(clip);
    //public void PlayButton1Sound() => uiSource.PlayOneShot(button1);

    void GetReferences()
    {
        uiSource = GetComponent<AudioSource>();
    }
}
