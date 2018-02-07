using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialSequence
{
    Default = -1,
    Ryukku,
    SkillLearn,
    FlameShot,
    Learn,
    SkillSet
}

public class TutorialManager : MonoBehaviour {

    [SerializeField] bool tutorialON;

    [SerializeField] GameObject focusImageObj;

    [SerializeField] Vector3 defaultPos;
    [SerializeField] Vector3 defaultScale;

    [SerializeField] TutorialData[] tutorialTable;

    TutorialSequence nowSequence;
    float animatingRate = -1;
    [SerializeField] float animatingSpeed;

    // Use this for initialization
    void Start () {
        nowSequence = TutorialSequence.Default;
        focusImageObj.transform.localPosition = defaultPos;
        focusImageObj.transform.localScale = defaultScale;
    }
	
	// Update is called once per frame
	void Update () {
        // タイトルでの設定によるチュートリアルON/OFF
        if ( !tutorialON ) return;

        if ( animatingRate < 0 ) return;

        if ( animatingRate > 1 )
        {
            // 演出終了
            animatingRate = -1;
            return;
        }
        animatingRate += animatingSpeed;

        Vector3 dis = tutorialTable[(int)nowSequence].pos - defaultPos;
        dis *= animatingRate;
        focusImageObj.transform.localPosition = defaultPos + dis;

        Vector3 difScale = tutorialTable[(int)nowSequence].scale - defaultScale;
        difScale *= animatingRate;
        focusImageObj.transform.localScale = defaultScale + difScale;
    }

    public void Switch(TutorialSequence type)
    {
        if ( tutorialTable[(int)type].done ) return;

        tutorialTable[(int)type].done = true;
        nowSequence = type;
        animatingRate = 0;
    }

    public void DebugSwitchTutorial()
    {
        TutorialSequence type = nowSequence;
        ++type;
        Switch(type);
    }
}
