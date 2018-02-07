using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRandomMove : AImove {

    private int faceProbability;//プレイヤーの方向に向かう確率 0 - 100 [%]
    BattleParticipant targetChara;
    [SerializeField] Vector3 targetPos = new Vector3(-1, 0, 0);
    //thisChara.type　でタイプを取得できる

    protected override void Start()
    {
        base.Start();
        faceProbability = 50;
    }

    void Update()
    {
        
    }

    public void FindLightMonster()
    {
        targetChara = parent.GetComponentInChildren<Player>();
        foreach (Enemy ene in eneMn.enemys)
        {
            if (ene.type == EnemyType.LIGHT)
            {
                // 光るモンスターが近いならそっちをターゲットする
                int d_x = Mathf.Abs((int)(ene.pos.x - thisChara.pos.x));
                int d_z = Mathf.Abs((int)(ene.pos.z - thisChara.pos.z));
                if (d_x < searchTargetRange)
                {
                    if (d_z < searchTargetRange)
                    {
                        targetChara = ene;
                        break;
                    }
                }
                // 光るモンスターが同じ部屋にいるならそっちをターゲットする
                if (0 <= thisChara.existRoomNum && thisChara.existRoomNum <= mapMn.max_room)
                {
                    if (ene.existRoomNum == thisChara.existRoomNum)
                    {
                        targetChara = ene;
                        break;
                    }
                }
            }
        }
    }

    public override bool GetMoveVec()
    {
         //プレイヤーのみを標的にする
        targetChara = parent.GetComponentInChildren<Player>();
        targetPos = targetChara.sPos;

        int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
        int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
        if (d_x < 2 && d_z < 2) // 隣接している場合、通常攻撃をする
        {
            thisChara.charaDir = targetPos - thisChara.pos;
            thisChara.target = targetChara;
            return false;
        }

        int rnd = Random.Range(0, 100);
        if (rnd < faceProbability)
            return GetVecToTarget(targetPos); //ターゲットに向かう

        //ランダムに動く
        int[] dir_x = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dir_z = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int rnd1 = Random.Range(0, 8);
        for (int i = 0; i < 8; ++i)
        {
            int now = rnd1 + i;
            if (now > 7) now -= 8;
            int nowX = (int)thisChara.pos.x + dir_x[now];
            int nowZ = (int)thisChara.pos.z + dir_z[now];
            if (nowX < 0) continue;
            if (nowX > MapManager.DUNGEON_WIDTH - 1) continue;
            if (nowZ < 0) continue;
            if (nowZ > MapManager.DUNGEON_HEIGHT - 1) continue;
            if (mapMn.chara_exist2D[nowZ, nowX] > -1) continue;
            thisChara.moveVec = new Vector3(dir_x[now], 0, dir_z[now]);
            break;
        }
        
        return true;
    }

    private bool GetVecToTarget(Vector3 targetPos)
    {
        Vector3 dis = targetPos - transform.position;
        if (dis == Vector3.zero)
            return false;
        return AStarG(targetPos);
    }

}
