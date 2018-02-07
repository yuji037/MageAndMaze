using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShot9 : StraightShot
{
    public override void OnDecided()
    {
        base.SetTarget(Vector3.zero);
        base.OnDecided();
        foreach ( Vector2 ran in range )
        {
            Vector3 judgePos = targetExistPos + new Vector3(ran.x, 0, ran.y);

            // 敵を感電させる

            int ID = mapMn.chara_exist2D[(int)judgePos.z, (int)judgePos.x];
            var chara = eneMn.GetEnemy(ID);
            if ( chara ) chara.CauseAbnormalState(AbnormalStateType.PARALIZE, abnoEffectPower);
        }
    }
}
