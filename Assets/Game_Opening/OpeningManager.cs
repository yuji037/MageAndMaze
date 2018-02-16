using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningManager : MonoBehaviour {

    [SerializeField] SceneTransitionManager sceneTransitionMn;
    [SerializeField] OpeningDialogManager openingDialogMn;

	// Use this for initialization
	void Start () {

        StartCoroutine(sceneTransitionMn.FadeIn());
        openingDialogMn.EventStart("opening_text");
	}
	
    public IEnumerator ToDungeonScene()
    {
        yield return StartCoroutine(sceneTransitionMn.FadeOut());
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon1");
    }
}
