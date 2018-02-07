using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShot : StraightShot {

    //public override void HitAndParamChange()
    //{
    //    if (range.Count == 0 )
    //    {
    //        base.HitAndParamChange();
    //        return;
    //    }
    //    foreach ( Vector2 ran in range )
    //    {
    //        Vector3 judgePos = targetExistPos - new Vector3(ran.x, 0, ran.y);
    //        int chara = mapMn.GetCharaExist((int)judgePos.x, (int)judgePos.z);
    //        if ( chara != -1 )
    //        {
    //            BattleParticipant target = eneMn.GetEnemy(chara);
    //            if ( !target ) target = obsMn.GetObstacle(chara);

    //            if ( target ) target.DamageParameter((int)calcPower, ParamType.HP, element, player);
    //        }
    //    }
    //}
}
