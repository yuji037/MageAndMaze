using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAct : SpecialActAI {

    private int range = 0;//矢の射程（マス）

    public override void Init()
    {
        base.Init();

        range = 10;
        probability = 50;
    }

    public override bool ShouldSpecialAct()
    {
        //特殊行動を行うかの判定
        int rnd = Random.Range(0, 100);
        if (rnd >= probability) return false;

        if (!FindTarget()) return false;
        
        int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
        int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
        if (d_x > range) return false;//xの範囲を超えている場合は除外
        if (d_z > range) return false;//zの範囲を超えている場合は除外
        if (d_x != 0) if (d_z != 0) if (d_x != d_z) return false;//斜めで一直線上にいない場合は除外

        //アーチャーとターゲットの間に壁や他のキャラがいないことを確認する
        int count = (d_x > d_z) ? d_x : d_z;
        d_x = (int)(targetChara.sPos.x - thisChara.pos.x);
        d_z = (int)(targetChara.sPos.z - thisChara.pos.z);
        int inc_x = 0;
        int inc_z = 0;
        if (d_x != 0) inc_x = (d_x > 0) ? 1 : -1;
        if (d_z != 0) inc_z = (d_z > 0) ? 1 : -1;
        d_x = (int)thisChara.pos.x;
        d_z = (int)thisChara.pos.z;

        for (int i = 0; i < count - 1; ++i)
        {
            d_x += inc_x;
            d_z += inc_z;
            if (mapMn.dung_2D[d_z, d_x] < 0) return false;
            if (mapMn.chara_exist2D[d_z, d_x] > -1) return false;
        }

        return true;
    }
}
