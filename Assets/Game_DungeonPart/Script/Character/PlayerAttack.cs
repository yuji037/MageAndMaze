using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour {

    GameObject parent;
    Player player;
    PlayerSkillTree playerSkillTree;
    int attackType = -1;
    PlayerActionAnimation actAnim;
    TurnManager turnMn;
    MapManager mapMn;
    TutorialManager tutorialMn;
    [SerializeField] GameObject[] rangePrafab;
    [SerializeField] List<GameObject> rangeRects = new List<GameObject>();
    GameObject selectingAct;

    public float dragTime = 0;
    GameObject popUpDescription;

    private void Awake()
    {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        playerSkillTree = player.GetComponent<PlayerSkillTree>();
        actAnim = GetComponent<PlayerActionAnimation>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();
        popUpDescription = parent.GetComponentInChildren<MagicSelectPopUpDescription>().gameObject;
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(UpdateCoroutine());
	}

    IEnumerator UpdateCoroutine()
    {
        while ( true )
        {
            // 選択中のスキル説明ポップアップ
            bool popUpOn = ( dragTime > 0.5f );
            popUpDescription.SetActive(popUpOn);

            yield return null;
        }
        
    }
	
    public void SetSkillRangeActive(int num)
    {
        
        for (int i = rangeRects.Count - 1; i >= 0; --i)
        {
            Destroy(rangeRects[i]);
            rangeRects.RemoveAt(i);
        }

        if (selectingAct) Destroy(selectingAct);
        selectingAct = Instantiate(actAnim.skills[num]);
        UpdatePopUpDescription(num);
        Debug.Log("選択中のスキル : " + num);
        SkillBase skill = selectingAct.GetComponent<SkillBase>();
        skill.OnSelected();
        List<Vector3> ranges = new List<Vector3>();
        ranges = skill.GetRange();
        foreach(Vector3 pos in ranges)
        {
            int map = mapMn.dung_2D[(int)pos.z, (int)pos.x];
            var rect = Instantiate((map < 0) ? rangePrafab[1] : rangePrafab[0]);
            rect.transform.position = pos;
            rangeRects.Add(rect);
        }
    }

    public void UpdatePopUpDescription(int num)
    {
        SkillData skillData = playerSkillTree.Skills[num];
        string description = skillData.SkillName + "\nMP:" + skillData.useMp + "\n" + skillData.skillDescription;
        popUpDescription.GetComponentInChildren<Text>().text = description;
    }

    public void DestroySkillRange()
    {
        for (int i = rangeRects.Count - 1; i >= 0; --i)
        {
            Destroy(rangeRects[i]);
            rangeRects.RemoveAt(i);
        }
    }
    public void MagicAttack(int num)
    {
        // チュートリアル中の操作制限
        if ( tutorialMn.IsTutorialON )
        {
            if ( ( 2 <= tutorialMn.TutorialNumber
                && tutorialMn.TutorialNumber <= 4 )
                //|| ( 5 <= tutorialMn.TutorialNumber
                //&& tutorialMn.TutorialNumber <= 6 )
                )
            {
                // 一切の攻撃不可
                return;
            }
            if ( ( 5 <= tutorialMn.TutorialNumber
                && tutorialMn.TutorialNumber <= 6 )
                )
            {
                // マジックショット以外不可
                if (num != 1) return;
            }
        }

        if (turnMn.PlayerActionSelected) return;

        attackType = num;
        player.action = ActionType.ATTACK;
        ActionData addAction = new ActionData(player, player.action, attackType, 0, Vector3.zero);
        player.thisTurnAction.Add(addAction);
        turnMn.AddAction(addAction);
        if ( attackType != -1 )
        {
            actAnim.SetAttackNumber(attackType);
            attackType = -1;
        }
        player.PlayerActSelect();
    }
}
