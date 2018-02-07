using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour {

    [SerializeField] Canvas sceneTransitionCanvas;
    public Image fadeInImage;
    [SerializeField] float fadeSpeed = 1;

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
        for ( float t = 0; ; t += Time.deltaTime * fadeSpeed )
        {
            fadeInImage.color = new Color(color.r, color.g, color.b, t);
            if ( t >= 1 ) yield break;
            yield return null;
        }
    }
}
