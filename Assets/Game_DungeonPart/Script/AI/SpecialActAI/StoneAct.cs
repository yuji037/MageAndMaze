using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAct : SpecialActAI
{
    private int range = 0;//石の射程（マス）

    public override void Init()
    {
        base.Init();

        range = 3;
        probability = 50;
    }

    public override bool ShouldSpecialAct()
    {
        //特殊行動を行うかの判定
        int rnd = Random.Range(0, 100);
        if ( rnd >= probability ) return false;

        if (!FindTarget()) return false;

        int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
        int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
        if (d_x > range) return false;//xの範囲を超えている場合は除外
        if (d_z > range) return false;//zの範囲を超えている場合は除外

        return true;
    }
}
