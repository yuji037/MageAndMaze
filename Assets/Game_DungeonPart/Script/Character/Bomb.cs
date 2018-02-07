using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Obstacle {

    public override void CauseElementEffect(SkillBase.TYPE type)
    {
        Debug.Log("属性効果を受けた");
        ActionData addAction = null;
        switch ( type )
        {
            case SkillBase.TYPE.FLAME:
            case SkillBase.TYPE.LIGHTNING:
                var skillObj = Instantiate(skills[0]);
                prepareSkills.Add(skillObj);
                skillObj.transform.position = pos;
                var skill = skillObj.GetComponent<NPCSkill>();
                skill.thisChara = this;
                skill.Init();
                Debug.Log("爆発を選択");
                addAction = new ActionData(this, ActionType.ATTACK, 0, 0, pos);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
                break;
        }
    }
}
