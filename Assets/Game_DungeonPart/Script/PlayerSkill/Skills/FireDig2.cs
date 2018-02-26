using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDig2 : StraightShot
{
    public override void HitAndParamChange()
    {
        foreach ( Vector2 ran in range )
        {
            Vector3 judgePos = targetExistPos + new Vector3(ran.x, 0, ran.y);
            int map = mapMn.GetDungeonInfo((int)judgePos.x, (int)judgePos.z);
            int chara = mapMn.GetCharaExist((int)judgePos.x, (int)judgePos.z);
            if ( map == -1 ) // 壁なら
            {
                mapMn.ChangeMapChip((int)judgePos.z, (int)judgePos.x, -1, 0);
            }
            if ( chara != -1 ) // 障害物
            {
                var obs = obsMn.GetObstacle(chara);
                if ( obs )
                {
                    obs.Kill();
                }
            }
        }
        base.HitAndParamChange();
    }
}
