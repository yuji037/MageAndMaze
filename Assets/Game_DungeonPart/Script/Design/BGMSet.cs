using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSet : MonoBehaviour {

    [SerializeField]
    AudioClip[] bgm;

    [SerializeField]
    int[] dungeonTypeBgmNums;

    public void SetBGM(int dungType)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        int bgmNum = dungeonTypeBgmNums[dungType];
        audioSource.clip = bgm[bgmNum];
        audioSource.Play();
    }
}
