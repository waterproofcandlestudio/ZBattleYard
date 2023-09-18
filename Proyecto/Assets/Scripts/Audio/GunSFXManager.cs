using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSFXManager : MonoBehaviour
{
    static AudioSource sfxSource;

    void Awake() => GetReferences();

    public static void PlayGunSound(AudioClip clip) => sfxSource.PlayOneShot(clip);

    void GetReferences()
    {
        sfxSource = GetComponent<AudioSource>();
    }
}
