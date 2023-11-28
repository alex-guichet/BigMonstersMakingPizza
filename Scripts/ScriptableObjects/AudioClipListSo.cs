using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AudioClipListSO", fileName = "AudioClipListSO")]
public class AudioClipListSO : ScriptableObject
{
    public AudioClip[] chopSounds;
    public AudioClip[] deliveryFails;
    public AudioClip[] deliverySuccesses;
    public AudioClip[] footSteps;
    public AudioClip[] objectDrops;
    public AudioClip[] objectPickUps;
    public AudioClip panSizzle;
    public AudioClip[] trash;
    public AudioClip[] warnings;
    public AudioClip[] bell;
    public AudioClip[] orderBell;
}
