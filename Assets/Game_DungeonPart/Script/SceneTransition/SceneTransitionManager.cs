using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour {

    [SerializeField] Canvas sceneTransitionCanvas;
    public Image fadeInImage;
    [SerializeField] float fadeSpeed = 1;

    [SerializeField] AudioSource bgm;

    private void Awake()
    {
        sceneTransitionCanvas.enabled = true;
    }

    public IEnumerator FadeIn()
    {
        // フェードイン
        var color = fadeInImage.color;
        for ( float t = 0; ; t += Time.deltaTime * fadeSpeed )
        {
            fadeInImage.color = new Color(color.r, color.g, color.b, 1 - t);
            if ( t >= 1 ) yield break;
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        // フェードアウト
        var color = fadeInImage.color;
        float volume = 0;
        if ( bgm ) volume = bgm.volume;
        for ( float t = 0; ; t += Time.deltaTime * fadeSpeed )
        {
            fadeInImage.color = new Color(color.r, color.g, color.b, t);
            if ( bgm ) bgm.volume = volume * ( 1 - t );
            if ( t >= 1 ) yield break;
            yield return null;
        }
    }

    public IEnumerator BGMFadeOut()
    {
        float volume = 0;
        if ( bgm ) volume = bgm.volume;
        for ( float t = 0; ; t += Time.deltaTime * fadeSpeed )
        {
            if ( bgm ) bgm.volume = volume * ( 1 - t );
            if ( t >= 1 ) yield break;
            yield return null;
        }
    }
}
