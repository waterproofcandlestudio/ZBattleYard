using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource cameraAudioSource;

    [SerializeField] AudioClip[] audioClips;

    [Header("Modifiers")]
    [SerializeField][Range(0f, 3f)] float pitchChangerMin = 0.5f;
    [SerializeField][Range(0f, 3f)] float pitchChangerMax = 1.8f;

    void Awake() => GetReferences();

    public void RandomizePitch() => cameraAudioSource.pitch = Random.Range(pitchChangerMin, pitchChangerMax);
    public void RandomizePitch(AudioSource audioSource) => audioSource.pitch = Random.Range(pitchChangerMin, pitchChangerMax);
    public void RandomizeSelectedClip()
    {
        int Rand = Random.Range(0, audioClips.Length);
        cameraAudioSource.clip = audioClips[Rand];
        cameraAudioSource.Play();
    }

    void GetReferences()
    {
        cameraAudioSource = Camera.main.GetComponent<AudioSource>();
    }
}
