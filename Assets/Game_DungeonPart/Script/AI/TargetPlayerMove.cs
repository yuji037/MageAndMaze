using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayerMove : AImove {

    BattleParticipant targetChara;
    [SerializeField] Vector3 targetPos = new Vector3(-1, 0, 0);
    //thisChara.type　でタイプを取得できる
    int preExistRoomNum = 0;

    // 巡回モード（チュートリアルなどでOFF）
    public bool PatrolMode = true;
    public int NonPatrolSearchRange = 4;


    public void FindLightMonster()
    {
        Player player = parent.GetComponentInChildren<Player>();
        targetChara = null;
        targetChara = ( player.abnoState.transparentTurn <= 0 ) ? player : null;
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
        
        //光源モンスターかプレイヤーのどちらかをターゲットする
        FindLightMonster();

        // ターゲットを発見できるかどうかの判定
        bool targetFind = false;

        if ( targetChara )
        {
            if ( searchWholeFloor ) targetFind = true;
            else if ( targetChara.existRoomNum >= 0 && targetChara.existRoomNum < mapMn.max_room )
            {
                // ターゲットが部屋に居る
                // ターゲットと同部屋にいれば発見
                targetFind = ( targetChara.existRoomNum == thisChara.existRoomNum );
                if ( !targetFind && thisChara.existRoomNum >= mapMn.max_room )
                {
                    // ターゲットとの距離が近いと発見
                    int d_x = Mathf.Abs((int)( targetChara.sPos.x - thisChara.pos.x ));
                    int d_z = Mathf.Abs((int)( targetChara.sPos.z - thisChara.pos.z ));
                    targetFind = ( d_x <= searchTargetRange && d_z <= searchTargetRange );
                    if ( !PatrolMode ) targetFind = ( d_x <= NonPatrolSearchRange && d_z <= NonPatrolSearchRange );
                }
                // 攻撃されたら発見
                if ( !targetFind ) targetFind = ( thisChara.perpetrator );
            }
            else
            {
                // ターゲットが部屋以外の場所に居る
                // ターゲットとの距離が近いと発見
                int d_x = Mathf.Abs((int)( targetChara.sPos.x - thisChara.pos.x ));
                int d_z = Mathf.Abs((int)( targetChara.sPos.z - thisChara.pos.z ));
                targetFind = ( d_x <= searchTargetRange && d_z <= searchTargetRange );
                if ( !PatrolMode ) targetFind = ( d_x <= NonPatrolSearchRange && d_z <= NonPatrolSearchRange );
                // 攻撃されたら発見
                if ( !targetFind ) targetFind = ( thisChara.perpetrator );
            }
        }

        // 行動を決める処理
        if (!targetFind) {
            // ターゲット未発見 ＝ 巡回モード
            // 目標地点がある かつ　目標地点に着いた
            if ( targetPos.x != -1 && thisChara.pos == targetPos)                                             
            {
                targetPos.x = -1;   // ←NULLとして扱う
            }
            // 目標地点NULL かつ thisChara が部屋に居て、巡回モードONの場合
            if ( targetPos.x == -1 && thisChara.existRoomNum >= 0 && thisChara.existRoomNum < mapMn.max_room
                && PatrolMode)
            {
                // 一番遠い出入り口を目標地点に登録
                Vector3[] gatewayPos = mapMn.room_info[thisChara.existRoomNum].gatewayPos;
                float maxDis = 0;
                int maxNum = 0;
                float minDis = 100;
                int minNum = 0;
                for ( int i = 0; i < gatewayPos.Length; i++ )
                {
                    if ( gatewayPos[i].x == -1 ) continue;
                    float d = ( gatewayPos[i] - thisChara.pos ).magnitude;
                    if ( d > maxDis )
                    {
                        maxNum = i;
                        maxDis = d;
                    }
                    if ( d < minDis )
                    {
                        minNum = i;
                        minDis = d;
                    }
                }
                targetPos = gatewayPos[maxNum];
                // 元々この部屋に居てtargetPosがnullになったということは
                // 目的があってここに来た（プレイヤーなどがいた）のでそのまま近い出入り口を登録
                if ( thisChara.existRoomNum == preExistRoomNum )
                {
                    targetPos = gatewayPos[minNum];
                }
            }
            // それ以外で目標地点NULLの場合
            // （つまり部屋以外にいて巡回モードONか、
            // または巡回モードOFF）
            else if ( targetPos.x == -1 )
            {
                if ( !PatrolMode )
                {
                    thisChara.moveVec = Vector3.zero;
                    return true;
                }

                Vector3 dir = thisChara.charaDir;               // まっすぐ進む
                if ( mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir) )
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                if ( mapMn.IsBreakableObstacle(thisChara.pos + dir) )   // 壁があるので壊そうと通常攻撃
                {
                    thisChara.charaDir = dir;
                    return false;
                }

                dir = Quaternion.Euler(0, 90f * ( Random.Range(0, 2) * 2 - 1 ), 0) * dir;    // 左か右に進む
                if ( mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir) )
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                if ( mapMn.IsBreakableObstacle(thisChara.pos + dir) )   // 壁があるので壊そうと通常攻撃
                {
                    thisChara.charaDir = dir;
                    return false;
                }

                dir = Quaternion.Euler(0, -180f, 0) * dir;  // ↑の逆方向（右か左）に進む
                if ( mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir) )
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                if ( mapMn.IsBreakableObstacle(thisChara.pos + dir) )   // 壁があるので壊そうと通常攻撃
                {
                    thisChara.charaDir = dir;
                    return false;
                }

                dir = Quaternion.Euler(0, -90f, 0) * dir;       // 後ろに進む
                if ( mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir) )
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                if ( mapMn.IsBreakableObstacle(thisChara.pos + dir) )   // 壁があるので壊そうと通常攻撃
                {
                    thisChara.charaDir = dir;
                    return false;
                }

                thisChara.moveVec = Vector3.zero;
                return true;
            }
            if (targetPos.x != -1)                                                // 目標地点がある
            {
                GetVecToTarget(targetPos);
                if (thisChara.moveVec == Vector3.zero)
                {
                    // 目標地点があるのに移動できない状態の場合
                    targetPos.x = -1; 
                }
                return true;
            }
        }
        // ターゲット発見状態なら
        Debug.Log("targetFind = " + targetFind);
        if ( !targetChara ) return true;
        targetPos = targetChara.sPos;
        preExistRoomNum = thisChara.existRoomNum;
        Vector3 dis = targetPos - thisChara.sPos;
        if (dis.magnitude < 1.5f) // ルート２以下＝隣接している
        {
            
            if ( mapMn.DiagonalCheck(thisChara.pos, thisChara.pos + dis) )
            {
                // 通常攻撃が可能なので false
                thisChara.target = targetChara;
                thisChara.charaDir = dis;
                return false;
            }
        }
        // 隣接してないならプレイヤーへのベクトルを設定する
        bool move = GetVecToTarget(targetPos);
        // 進行方向に壊せるオブジェクトがあれば通常攻撃
        if(mapMn.IsBreakableObstacle(thisChara.pos + thisChara.moveVec) )
        {
            thisChara.charaDir = thisChara.moveVec;
            return false;
        }
        return move;
    }
    
   private bool GetVecToTarget(Vector3 targetPos)
    {
        Vector3 dis = targetPos - transform.position;
        if (dis == Vector3.zero)
            return false;
        return AStarN(targetPos);
    }

}
