using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Track")]
public class SO_AudioTrack : ScriptableObject
{
    public AudioClip trackAudioClip;
    public string trackName;
    public string trackAuthor;
}
