using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayMove : AImove
{

    BattleParticipant targetChara;
    [SerializeField] Vector3 targetPos = new Vector3(-1, 0, 0);
    private static int countDisappearance = 20;//プレイヤーに発見されてから消えるまでのターン数
    private bool findedPlayer;
    private int countAfterFinded;

    protected override void Start()
    {
        base.Start();
        findedPlayer = false;
        countAfterFinded = 0;
    }

    void Update()
    {

    }

    public override bool GetMoveVec()
    {
        targetChara = parent.GetComponentInChildren<Player>();

        if (!findedPlayer)
        {
            if (targetChara.existRoomNum >= 0 && targetChara.existRoomNum < mapMn.max_room) // ターゲットが部屋に居る
            {
                findedPlayer = (targetChara.existRoomNum == thisChara.existRoomNum); // プレイヤーと同部屋にいれば発見
                if (!findedPlayer && thisChara.existRoomNum >= mapMn.max_room)
                {
                    int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
                    int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
                    findedPlayer = (d_x <= searchTargetRange && d_z <= searchTargetRange);
                }
            }
            else                                                                            // ターゲットが部屋以外の場所に居る
            {
                int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
                int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
                findedPlayer = (d_x <= searchTargetRange && d_z <= searchTargetRange);
            }
        }
        else
        {
            ++countAfterFinded;
            if (countAfterFinded >= countDisappearance)
            {
                //消失処理
            }
        }
        if (!findedPlayer)
        {
            if (targetPos.x != -1 && thisChara.pos == targetPos)                                             // 目標地点がある かつ　着いた
            {
                targetPos.x = -1;   // ←NULLとして扱う
            }
            // 目標地点NULL かつ 敵自身が部屋に居る場合
            if (targetPos.x == -1 && thisChara.existRoomNum >= 0 && thisChara.existRoomNum < mapMn.max_room)
            {
                // 一番遠い出入り口を目標地点に登録
                Vector3[] gatewayPos = mapMn.room_info[thisChara.existRoomNum].gatewayPos;
                float maxDis = 0;
                int maxNum = 0;
                for (int i = 0; i < gatewayPos.Length; i++)
                {
                    if (gatewayPos[i].x == -1) continue;
                    float d = (gatewayPos[i] - thisChara.pos).magnitude;
                    if (d > maxDis)
                    {
                        maxNum = i;
                        maxDis = d;
                    }
                }
                targetPos = gatewayPos[maxNum];
            }
            // 目標地点NULL かつ 部屋以外に居る場合
            else if (targetPos.x == -1)
            {
                Vector3 dir = thisChara.charaDir;               // まっすぐ進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                dir = Quaternion.Euler(0, 90f, 0) * dir;    // 左に進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                dir = Quaternion.Euler(0, -180f, 0) * dir;  // 右に進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                dir = Quaternion.Euler(0, -90f, 0) * dir;       // 後ろに進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
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
        
        targetPos = targetChara.sPos;
        return GetVecToTarget(targetPos);
    }

    private bool GetVecToTarget(Vector3 targetPos)
    {
        Vector3 dis = targetPos - transform.position;
        if (dis == Vector3.zero)
            return false;

        if (findedPlayer)
            return AStarR(targetPos);
        else
            return AStarN(targetPos);
    }
}
