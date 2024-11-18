using UnityEngine.Audio;
using UnityEngine;
using System;

[System.Serializable]
public enum SoundType
{
    MUSIC,
    VFX,
    BACKGROUND
}

[System.Serializable]
public class SoundsScript 
{
    public string name;
    public SoundType type;
    public AudioClip clips;

    [Range(0f, 1f)] public float volume = 0.3f;
    [Range(.1f, 3f)] public float Pitch = 1f;

    public bool loopSound;

    [HideInInspector]
    public AudioSource source;
}
