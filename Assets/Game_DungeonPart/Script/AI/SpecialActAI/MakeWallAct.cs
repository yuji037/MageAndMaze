using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWallAct : SpecialActAI
{
    public override void Init()
    {
        base.Init();

        probability = 12;
    }

    public override bool ShouldSpecialAct()
    {
        //特殊行動を行うかの判定
        int rnd = Random.Range(0, 100);
        if ( rnd >= probability ) return false;

        Vector3 judgePos = thisChara.pos + thisChara.charaDir;
        if ( !mapMn.CanPutCharacter(judgePos)) return false;

        return true;
    }
}
