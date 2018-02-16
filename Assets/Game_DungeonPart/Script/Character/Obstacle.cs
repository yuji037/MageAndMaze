using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : BattleParticipant
{

    public enum Type
    {
        ROCK,
        ICEBLOCK,
        BOMB
    }

    public Type type;
    protected ObstacleManager obsMn;
    protected List<ActionData> thisTurnAction = new List<ActionData>();
    [SerializeField] protected List<GameObject> skills;
    protected List<GameObject> prepareSkills = new List<GameObject>();
    [SerializeField]
    List<GameObject> myBodys = new List<GameObject>();

    public override void Init()
    {
        base.Init();
        obsMn = parent.GetComponentInChildren<ObstacleManager>();
        Body[] bodys = GetComponentsInChildren<Body>();
        foreach (Body body in bodys )
        {
            myBodys.Add(body.gameObject);
        }
        sPos = pos;
    }

    public override void CharaUpdate()
    {
        base.CharaUpdate();

        if ( thisTurnAction.Count <= 0 ) return;
        if ( !thisTurnAction[0].allowed ) return;

        switch ( thisTurnAction[0].actType )
        {
            case ActionType.ATTACK:
                if ( actStarted ) return;
                Debug.Log("爆発スキルを起動");
                var skill = prepareSkills[0].GetComponent<NPCSkill>();
                skill.actionStart = true;
                actStarted = true;
                break;
        }
    }

    public override void ActEnd()
    {
        if ( prepareSkills[0] )
        {
            prepareSkills.RemoveAt(0);
        }
        thisTurnAction[0].finish = true;
        thisTurnAction.RemoveAt(0);

        actStarted = false;

        DeathCheck();
    }

    protected override void DeathCheck()
    {
        if ( HP <= 0 && isAlive )
        {
            mapMn.SetCharaExistInfo(sPos);
            foreach ( GameObject deadObj in deadObjPrefab )
            {
                var obj = Instantiate(deadObj);
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                Destroy(obj, 5.0f);
            }
            foreach ( GameObject aliveBody in myBodys )
            {
                Destroy(aliveBody);
            }
            isAlive = false;
        }
    }
}
