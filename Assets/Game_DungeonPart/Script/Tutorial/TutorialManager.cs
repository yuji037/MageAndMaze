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

    GameObject parent;
    DungeonPartManager dMn;
    Player player;
    [SerializeField] GameObject cameraParent;
    [SerializeField] GameObject magicSelectWindow;
    CameraManager cameraMn;
    EventTriggerCameraRotater eventTriggerCameraRotater;
    UISwitch uiSwitch;
    PlayerSkillTree playerSkillTree;
    [SerializeField] GameObject itemDescriotionPanel;

    [SerializeField] bool tutorialON;

    //[SerializeField] GameObject focusImageObj;

    //[SerializeField] Vector3 defaultPos;
    //[SerializeField] Vector3 defaultScale;

    [SerializeField] TutorialData[] tutorialTable;

    //TutorialSequence nowSequence;
    //float animatingRate = -1;
    //[SerializeField] float animatingSpeed;

    [SerializeField] GameObject arrow;

    [System.Serializable]
    class ArrowTransformData
    {
        public Vector3 localPosition;
        public Vector3 eulerAngles;
    }
    [SerializeField]
    ArrowTransformData[] arrowTransformData;

    public int TutorialNumber { get; private set; }

    // Use this for initialization
    public void StartBehaviour () {
        //nowSequence = TutorialSequence.Default;
        //focusImageObj.transform.localPosition = defaultPos;
        //focusImageObj.transform.localScale = defaultScale;

        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();

        // チュートリアルをする時以外、処理の必要なし
        if ( dMn.floor == 1 && SaveData.GetInt("IsTutorialON", 1) == 1 )
        {
            eventTriggerCameraRotater = parent.GetComponentInChildren<EventTriggerCameraRotater>();
            uiSwitch = parent.GetComponentInChildren<UISwitch>();
            playerSkillTree = parent.GetComponentInChildren<PlayerSkillTree>();

            eventTriggerCameraRotater.RotateMoveButtonsAndMiniMap(180);
            TutorialNumber = 0;
            StartCoroutine(TutorialCoroutine());
        }
        else TutorialNumber = 100;
    }
	
	// Update is called once per frame
	void Update () {
        // タイトルでの設定によるチュートリアルON/OFF
        if ( !tutorialON ) return;
        //Debug.Log(Input.touchCount);

        //if ( animatingRate < 0 ) return;
        //if ( animatingRate > 1 )
        //{
        //    // 演出終了
        //    animatingRate = -1;
        //    return;
        //}
        //animatingRate += animatingSpeed;

        //Vector3 dis = tutorialTable[(int)nowSequence].pos - defaultPos;
        //dis *= animatingRate;
        //focusImageObj.transform.localPosition = defaultPos + dis;

        //Vector3 difScale = tutorialTable[(int)nowSequence].scale - defaultScale;
        //difScale *= animatingRate;
        //focusImageObj.transform.localScale = defaultScale + difScale;
    }

    public void Switch(TutorialSequence type)
    {
        //if ( tutorialTable[(int)type].done ) return;

        //tutorialTable[(int)type].done = true;
        //nowSequence = type;
        //animatingRate = 0;
    }

    public void DebugSwitchTutorial()
    {
        //TutorialSequence type = nowSequence;
        //++type;
        //Switch(type);
    }


    IEnumerator TutorialCoroutine()
    {
        player = parent.GetComponentInChildren<Player>();

        arrow.SetActive(true);
        
        // 移動
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while(player.action != ActionType.MOVE )
        {
            yield return null;
        }
        // キャラの方向転換
        SetNextArrow();
        Vector3 charaDir = player.charaDir;
        yield return new WaitForSeconds(0.2f);
        while ( player.charaDir == charaDir )
        {
            yield return null;
        }


        // カメラ回転
        SetNextArrow();
        float eulerY = cameraParent.transform.eulerAngles.y;
        yield return new WaitForSeconds(0.2f);
        while ( Mathf.Abs(cameraParent.transform.eulerAngles.y - eulerY) < 80 )
        {
            yield return null;
        }

        // 魔法選択ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while ( !magicSelectWindow.activeSelf )
        {
            yield return null;
        }

        // マジックショット
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while ( player.skillNum != 1 )
        {
            yield return null;
        }

        //int exp = player.Exp;
        //while (player.Exp == exp)
        //{
        //    yield return null;
        //}
        yield return StartCoroutine(WaitUntilFingerUp());

        // セリフ

        // バッグボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while ( uiSwitch.UIType != DungUIType.INVENTRY )
        {
            yield return null;
        }

        // 「スキル習得」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while ( uiSwitch.UIType != DungUIType.SKILLTREE )
        {
            yield return null;
        }

        // 「フレイムショット」を選択
        SetNextArrow();
        // ↓楳井君の変更待ち
        //yield return new WaitForSeconds(0.2f);
        //while ( uiSwitch.UIType != DungUIType.SKILLTREE )
        //{
        //    yield return null;
        //}
        yield return StartCoroutine(WaitUntilFingerUp());

        // 「習得する」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while ( !playerSkillTree.Skills[101].Syutoku )
        {
            yield return null;
        }

        // スキルセットの2番
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        bool isSetSkill = false;
        while ( !isSetSkill )
        {
            foreach (int skill in playerSkillTree.Skills.Keys )
            {
                if (skill == 101 )
                {
                    isSetSkill = true;
                }
            }
            yield return null;
        }

        // セリフ

        // 「修練＆精製」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while ( uiSwitch.UIType != DungUIType.PRACTICE_AND_ITEMCRAFT )
        {
            yield return null;
        }

        // 「攻撃力アップのオーブ」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!itemDescriotionPanel.activeSelf )
        {
            yield return null;
        }

        SetNextArrow();
        yield return StartCoroutine(WaitUntilFingerUp());

        yield return null;

    }

    IEnumerator WaitUntilTap()
    {
        yield return new WaitForSeconds(0.2f);
        while ( !Input.GetMouseButtonDown(0) )
        {
            yield return null;
        }
    }

    IEnumerator WaitUntilFingerUp()
    {
        yield return new WaitForSeconds(0.2f);
        while( Input.touchCount == 0 && Input.GetMouseButton(0) )
        {
            yield return null;
        }
        while ( Input.touchCount != 0 && !Input.GetMouseButtonUp(0) )
        {
            yield return null;
        }
    }

    void SetNextArrow()
    {
        arrow.transform.localPosition = arrowTransformData[TutorialNumber].localPosition;
        arrow.transform.eulerAngles = arrowTransformData[TutorialNumber].eulerAngles;
        TutorialNumber++;
    }
}
