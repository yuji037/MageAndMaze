using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock9 : StraightShot
{
    public override void OnDecided()
    {
        base.SetTarget(Vector3.zero);
        base.OnDecided();
        foreach ( Vector2 ran in range )
        {
            Vector3 judgePos = targetExistPos + new Vector3(ran.x, 0, ran.y);

            // 氷ブロックを置くか、敵を凍らせる
            // 氷ブロックの場合
            if ( mapMn.CanPutCharacter(judgePos) )
                mapMn.chara_exist2D[(int)judgePos.z, (int)judgePos.x] = 200;

            // 敵の場合
            int ID = mapMn.chara_exist2D[(int)judgePos.z, (int)judgePos.x];
            var chara = eneMn.GetEnemy(ID);
            if ( chara ) chara.CauseAbnormalState(AbnormalStateType.FREEZE, abnoEffectPower);
        }
    }

    public override void HitAndParamChange()
    {
        foreach ( Vector2 ran in range )
        {
            //// ダメージ
            //Vector3 judgePos = targetExistPos - new Vector3(ran.x, 0, ran.y);
            //int chara = mapMn.GetCharaExist((int)judgePos.x, (int)judgePos.z);
            //if ( chara != -1 )
            //{
            //    BattleParticipant target = eneMn.GetEnemy(chara);
            //    if ( target )
            //    {
            //        target.DamageParameter((int)calcPower);
            //        target.UpdateAbnoEffect();
            //    }
            //}

            // 氷ブロック設置
            var obs = obsMn.AddObstacle(Obstacle.Type.ICEBLOCK, targetExistPos + new Vector3(ran.x, 0, ran.y));
            if ( obs )
            {
                obs.HP = (int)rowPower;
                obs.MaxHP = obs.HP;
            }
        }

        //battleTarget = null;
        base.HitAndParamChange();
    }
}
