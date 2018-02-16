using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSceneManager : MonoBehaviour {
    
    [SerializeField] SceneTransitionManager sceneTransitionManager;


	// Use this for initialization
	void Start () {
        StartCoroutine(sceneTransitionManager.FadeIn());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    Coroutine totitle = null;

    public void ToTitleScene()
    {
        if( totitle == null) totitle = StartCoroutine(ToTitleCoroutine());
    }

    IEnumerator ToTitleCoroutine()
    {
        yield return StartCoroutine(sceneTransitionManager.FadeOut());
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        totitle = null;
    }

}
