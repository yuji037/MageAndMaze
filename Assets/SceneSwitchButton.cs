using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSwitchButton : MonoBehaviour {

    GameObject parent;
    SceneTransitionManager sceneTransitionManager;

    [SerializeField]
    GameObject menu;
    [SerializeField]
    Button continueButton;
    [SerializeField]
    Image[] fadeInImgs;
    [SerializeField]
    Text fadeOutText;
    float timeFade = 0;
    bool menuOn = false;
    

    // Use this for initialization
    void Start () {
        // 中断フラグONなら中断データからコンティニュー（再開）できる
        int isInterrupt = SaveData.GetInt("IsInterrupt", 0);
        continueButton.gameObject.SetActive((isInterrupt == 1) ? true : false);
        parent = GameObject.Find("GameObjectParent");
        sceneTransitionManager = parent.GetComponentInChildren<SceneTransitionManager>();
        sceneTransitionManager.fadeInImage.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update () {
        if (menuOn)
        {
            timeFade += Time.deltaTime;
            foreach(Image img in fadeInImgs)
            {
                Color color = img.color;
                img.color = new Color(color.r, color.g, color.b, timeFade);
            }
            Color colr = fadeOutText.color;
            fadeOutText.color = new Color(colr.r, colr.g, colr.b, ( 1 - timeFade ));
        }
	}
    public void ToMenu()
    {
        menuOn = true;
        menu.SetActive(true);
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }
    
    // 洞窟１の最初から
    public void ToDungeon1Scene()
    {
        StartCoroutine(ToDungeon1SceneCoroutine(false));
    }

    public IEnumerator ToDungeon1SceneCoroutine(bool isContinue)
    {
        yield return StartCoroutine(sceneTransitionManager.FadeOut());

        if ( !isContinue )
        {
            // 中断フラグOFF（最初から）
            SaveData.Clear();
            SaveData.Save();
        }
        SceneManager.LoadScene("Dungeon1");
    }

    //public void ToDungeon2Scene()
    //{
    //    SceneManager.LoadScene("Dungeon1");
    //}
    // 中断データから
    public void ToContinue()
    {
        StartCoroutine(ToDungeon1SceneCoroutine(true));
    }
    public void ToVillageScene()
    {
        SceneManager.LoadScene("Village1");
    }

    
}
