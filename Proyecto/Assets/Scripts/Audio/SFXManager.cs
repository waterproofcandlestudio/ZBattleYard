using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    static AudioSource sfxSource;
    AudioLowPassFilter lowPassFilter;

    [SerializeField] static AudioMixerGroup audioMixerGroup;
    [SerializeField][Range(0, 1)] static float pitchChanger = 0.5f;

    void Awake() => GetReferences();

    public static void PlaySound_AudioMixer(AudioClip clip)
    {
        bool value = audioMixerGroup.audioMixer.GetFloat("sfxVolume", out float volume);
        volume = Mathf.Pow(10f, volume / 20); //converted from Log10
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
    public static void PlaySound_AudioMixer(AudioClip clip, Vector3 position)
    {
        bool value = audioMixerGroup.audioMixer.GetFloat("sfxVolume", out float volume);
        volume = Mathf.Pow(10f, volume / 20); //converted from Log10
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
    public static void PlaySound_AudioMixer(AudioClip clip, Vector3 position, float volumeMultiplier)
    {
        bool value = audioMixerGroup.audioMixer.GetFloat("sfxVolume", out float volume);
        volume = Mathf.Pow(10f, volume / 20); //converted from Log10
        AudioSource.PlayClipAtPoint(clip, position, volume * volumeMultiplier);
    }
    //public static void PlaySound(AudioClip clip)
    //{
    //    sfxSource.PlayOneShot(clip);
    //}
    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public static IEnumerator PlaySound_CoroutineWait(AudioClip clip, float time)
    {
        yield return new WaitForSeconds(time);
        sfxSource.PlayOneShot(clip);
    }
    static void CalculateSoundPitch()
    {
        sfxSource.pitch = Random.Range(1 - pitchChanger, 1 + pitchChanger);
    }

    void GetReferences()
    {
        sfxSource = GetComponent<AudioSource>();
        audioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }
}
