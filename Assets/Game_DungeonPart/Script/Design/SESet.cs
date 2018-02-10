using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SESet : MonoBehaviour
{
    public enum Type
    {
        STONE_BREAK,
        WIND_CUT,
        STAIRS_DOWN,
    }

    [SerializeField]
    AudioClip[] sounds;

    AudioSource audioSource;

    public void PlaySE(Type seType)
    {
        if ( !audioSource ) audioSource = GetComponent<AudioSource>();
        audioSource.clip = sounds[(int)seType];
        audioSource.Play();
    }
}
