using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionAnimation : MonoBehaviour {

    Player player;
    int attackType = -1;

    [SerializeField]
    public GameObject[] acts;

    [SerializeField]
    public Dictionary<int, GameObject> skills;

    // Use this for initialization
    void Start () {
        player = GetComponent<Player>();
        Init();
	}

    public void SetAttackNumber(int num)
    {
        attackType = num;
        if ( attackType != -1 )
        {
            int _useMp = skills[attackType].GetComponent<SkillBase>().useMp;
            if ( !player.MpUseSkill(_useMp) ) attackType = 0;      // MP足りなかったら物理攻撃を自動選択

            player.skillNum = attackType;
            var _act = Instantiate(skills[attackType], transform);
            var _skillBase = _act.GetComponent<SkillBase>();
            _skillBase.shouldActionStart = true;
            _skillBase.OnSelected();
            _skillBase.OnDecided();
            attackType = -1;
        }
    }

    public void Init()
    {
        skills = new Dictionary<int, GameObject>();
        Dictionary<int, SkillData> skillData = player.GetComponent<PlayerSkillTree>().Skills;
        List<int> ids = new List<int>();
        foreach (int key in skillData.Keys)
        {
            ids.Add(key);
        }
        foreach (int k in ids)
        {
            skills[k] = Resources.Load<GameObject>("SkillPrefab/" + k) as GameObject;
        }
    }
}
