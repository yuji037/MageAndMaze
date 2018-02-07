using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMagic : SkillBase {

    [SerializeField]
    int _rangeZ = 10;

    [SerializeField]
    protected int _particleMax = 10;

    [SerializeField] GameObject hitEff;

    void Update()
    {
        if (shouldActionStart)
        {
            StartCoroutine(Coroutine());
            shouldActionStart = false;
        }
    }

    public virtual IEnumerator Coroutine()
    {
        HitAndParamChange();
        yield return null;
    }

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
                    target.perpetrator = player;
                    target.CauseElementEffect(element);
                }
            }
        }
    }

    public override List<Vector3> GetRange()
    {
        List<Vector3> rangeList = new List<Vector3>();
        int _existRoomNum = player.existRoomNum;
        int[,] dung_room_info2D = mapMn.dung_room_info2D;
        if ( _existRoomNum < mapMn.max_room )     // 自分が部屋に居る
        {
            for ( var z = 0; z < MapManager.DUNGEON_HEIGHT; z++ )
            {
                for ( var x = 0; x < MapManager.DUNGEON_WIDTH; x++ )
                {
                    if ( dung_room_info2D[z, x] == _existRoomNum )
                    {
                        Vector3 _rangePos = new Vector3(x, 0, z);
                        rangeList.Add(_rangePos);
                    }
                }
            }
        }
        else    // 自分が道か、もともと壁だった場所(-1)に居る
        {
            for ( int ran = 1; ran < _rangeZ; ran++ )
            {
                Vector3 targetP = player.pos + player.charaDir * ran;
                int z = (int)targetP.z;
                int x = (int)targetP.x;
                if ( dung_room_info2D[z, x] >= mapMn.max_room ) // 壁か部屋にぶつかるまで続く
                {
                    Vector3 _rangePos = new Vector3(x, 0, z);
                    rangeList.Add(_rangePos);
                }
                else if ( mapMn.dung_2D[z, x] < 0 ) break;  // 壁があったら範囲はそこまで
            }
        }
        
        foreach(Vector3 _ranPos in rangeList)
        {
            range.Add(new Vector2(_ranPos.x, _ranPos.z));
        }

        return rangeList;
    }

    public virtual void HitAndParamChange()
    {

        foreach(Vector2 ran in range)
        {
            Vector3 judgePos = new Vector3(ran.x, 0, ran.y);
            int chara = mapMn.chara_exist2D[(int)judgePos.z, (int)judgePos.x];
            if ( chara != -1 )
            {
                BattleParticipant target = eneMn.GetEnemy(chara);
                if ( !target ) target = obsMn.GetObstacle(chara);
                if ( target )
                {
                    target.DamageParameter((int)calcPower, ParamType.HP, element, player);
                    if ( hitEff ) Instantiate(hitEff, judgePos, transform.rotation, transform);
                }
            }
        }

        player.ActEnd();
        Destroy(gameObject, 3.0f);
    }
}
