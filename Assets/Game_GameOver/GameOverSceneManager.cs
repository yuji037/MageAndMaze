using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneManager : MonoBehaviour {

    [SerializeField]
    SceneTransitionManager sceneTransitionMn;

    // Use this for initialization
    void Start()
    {
        if ( sceneTransitionMn ) StartCoroutine(sceneTransitionMn.FadeIn());
    }
	
}