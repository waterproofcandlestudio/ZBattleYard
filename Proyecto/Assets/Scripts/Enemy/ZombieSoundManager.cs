using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSoundManager : MonoBehaviour
{
    AudioSource zombieAudioSource;
    ZombieStats stats;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] walkingClips;
    [SerializeField] AudioClip[] attackClips;
    [Header("Parameters")]
    [SerializeField] float waitTime = 3;

    float timer;

    void Awake()
    {
        zombieAudioSource = GetComponent<AudioSource>();
        stats = GetComponent<ZombieStats>();
    }

    void Update()
    {
        if (stats.IsDead())
        {
            zombieAudioSource.Stop();
            return;
        }

        WalkEffects_Timer();
        if (timer <= 0)
        {
            AudioClip clip = walkingClips[Random.Range(0, walkingClips.Length - 1)];
            zombieAudioSource.PlayOneShot(clip);
            timer = waitTime;
        }
    }
    public void PlayAttackEffect()
    {
        AudioClip clip = walkingClips[Random.Range(0, attackClips.Length - 1)];
        zombieAudioSource.Stop();
        zombieAudioSource.PlayOneShot(clip);
    }

    void WalkEffects_Timer()
    {
        timer -= Time.deltaTime;
    }
}
