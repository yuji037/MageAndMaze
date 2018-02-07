using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialActAI : MonoBehaviour {

    protected GameObject parent;
    protected MapManager mapMn;
    protected EnemyManager eneMn;
    protected BattleParticipant thisChara;
    public BattleParticipant targetChara { get; private set; }
    
    protected int probability = 0;//特殊行動の発生確率 (0 - 100 [%])

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
                if (d_x < AImove.searchTargetRange)
                {
                    if (d_z < AImove.searchTargetRange)
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

    public bool FindTarget()
    {
        // UFO は光源モンスターはターゲットしない
        //FindLightMonster();
        targetChara = parent.GetComponentInChildren<Player>();

        bool ans = false;

        if (targetChara.existRoomNum >= 0 && targetChara.existRoomNum < mapMn.max_room) // ターゲットが部屋に居る
        {
            ans = (targetChara.existRoomNum == thisChara.existRoomNum); // プレイヤーと同部屋にいれば発見
            if (!ans)
            {
                int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
                int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
                ans = (d_x <= AImove.searchTargetRange && d_z <= AImove.searchTargetRange);
            }
        }
        else                                                                            // ターゲットが部屋以外の場所に居る
        {
            int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
            int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
            ans = (d_x <= AImove.searchTargetRange && d_z <= AImove.searchTargetRange);
        }

        return ans;
    }

    public virtual void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        thisChara = GetComponent<BattleParticipant>();
        targetChara = parent.GetComponentInChildren<Player>();
    }

    public virtual bool ShouldSpecialAct()
    {

        return false;
    }
}
