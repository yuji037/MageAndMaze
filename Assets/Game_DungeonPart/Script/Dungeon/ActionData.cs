using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionData {

    public bool allowed = false;
    public bool finish = false;
    public BattleParticipant actChara;
    public ActionType actType;
    public int skillNum;
    public int sequence;
    public Vector3 pos;

    public float actionRate = 0;
    // 倍速移動時はspeedRate = 2
    public float speedRate = 1;
    public TurnManager turnMn;

    public ActionData(BattleParticipant _chara, ActionType _type, int _skillNum, int _seq, Vector3 _pos)
    {
        actChara = _chara;
        actType = _type;
        skillNum = _skillNum;
        sequence = _seq;
        pos = _pos;
    }

    public void ActionUpdate()
    {
        //if ( finish ) return;

        //if ( actType == ActionType.MOVE )
        //{
        //    actionRate += Time.deltaTime * turnMn.moveSpeed * speedRate;
        //    if (actionRate >= 1 )
        //    {
        //        // 移動終了
        //        finish = true;
        //        actChara.transform.position = pos;
        //        actChara.pos = pos;
        //    }
        //    else
        //    {
        //        // 移動の途中
        //        Vector3 distance = ( pos - actChara.pos ) * actionRate;
        //        actChara.transform.position = actChara.pos + distance;
        //    }
        //}
        //if( actType != ActionType.MOVE )
        //{
        //    finish = true;
        //}
    }
    
}
