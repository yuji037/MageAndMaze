using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBoss : MonoBehaviour {

    [SerializeField] GameObject shineEffect;
    [SerializeField] GameObject explodeEffect;
    Boss1 thisEnemy;

    GameObject parent;
    SceneTransitionManager sceneTransitionManager;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        sceneTransitionManager = parent.GetComponentInChildren<SceneTransitionManager>();

        thisEnemy = GetComponent<Boss1>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator DeathCoroutine()
    {
        shineEffect.SetActive(true);
        StartCoroutine(sceneTransitionManager.BGMFadeOut());
        yield return new WaitForSeconds(2);

        GetComponentInChildren<AudioSource>().Play();

        yield return new WaitForSeconds(4);

        explodeEffect.SetActive(true);
        shineEffect.SetActive(false);
        thisEnemy.DestroyMyBodys();
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(sceneTransitionManager.FadeOut());
        yield return new WaitForSeconds(1);

        parent.GetComponentInChildren<DungeonPartManager>().SaveGameClear();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");
    }
}
