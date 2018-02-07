using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : StraightShot
{
    public override void OnDecided()
    {
        base.OnDecided();
        if ( mapMn.CanPutCharacter(targetExistPos) )
            mapMn.chara_exist2D[(int)targetExistPos.z, (int)targetExistPos.x] = 200;
    }

    public override void HitAndParamChange()
    {
        // 氷ブロック設置
        var obs = obsMn.AddObstacle(Obstacle.Type.ICEBLOCK, targetExistPos);
        if ( obs )
        {
            obs.HP = (int)rowPower;
            obs.MaxHP = obs.HP;
        }

        base.HitAndParamChange();
    }
}
