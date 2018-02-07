using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRoom : RoomMagic {

    public override void OnDecided()
    {
        base.OnDecided();

        foreach ( Vector2 ran in range )
        {
            Vector3 judgePos = new Vector3(ran.x, 0, ran.y);
            int chara = mapMn.chara_exist2D[(int)judgePos.z, (int)judgePos.x];
            if ( chara != -1 )
            {
                BattleParticipant target = eneMn.GetEnemy(chara);
                if ( !target ) target = obsMn.GetObstacle(chara);
                if ( target )
                {
                    target.CauseAbnormalState(AbnormalStateType.PARALIZE, abnoEffectPower);
                }
            }
        }
        
    }

    public override IEnumerator Coroutine()
    {
        cameraMn.SetIsBigMagicCamera(true);
        anim.TriggerAnimator("Magic_forward2");
        yield return new WaitForSeconds(0.5f);
        cameraMn.SetIsBigMagicCamera(false);
        GetComponent<AudioSource>().Play();
        // プレイヤーは部屋に居る
        if (player.existRoomNum < mapMn.max_room )
        {
            for (int p = 0; p < _particleMax; p++)
            {
                int num = Random.Range(0, range.Count);
                var eff = Instantiate(effects[0]);
                eff.transform.position = new Vector3(range[num].x, 0, range[num].y);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else // それ以外
        {
            foreach (Vector2 ran in range)
            {
                var eff = Instantiate(effects[0]);
                eff.transform.position = new Vector3(ran.x, 0, ran.y);
                yield return new WaitForSeconds(0.2f);
            }
        }
        HitAndParamChange();
        yield return null;
    }
}
