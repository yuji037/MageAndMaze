using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRandomMove : AImove
{
    //thisChara.type　でタイプを取得できる

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {

    }

    public override bool GetMoveVec()
    {
        //ランダムに動く
        int thisX = (int)thisChara.pos.x;
        int thisZ = (int)thisChara.pos.z;
        //Vector3 ans = new Vector3(0, 0, 0);
        int count = 0;
        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                bool selectedStop = (x == 0 && z == 0);
                if (selectedStop) continue;//この条件を外した場合、停止がランダムの動きに加わる
                int nextX = thisX + x;
                int nextZ = thisZ + z;
                //マップの範囲外を除く
                if (nextX < 0) continue;
                if (nextX > MapManager.DUNGEON_WIDTH - 1) continue;
                if (nextZ < 0) continue;
                if (nextZ > MapManager.DUNGEON_HEIGHT - 1) continue;
                //他のキャラクターがいる場合、除く
                if (mapMn.chara_exist2D[nextZ, nextX] > -1 || selectedStop) continue;
                //壁がある場合、除く
                if (mapMn.dung_2D[nextZ, nextX] < 0) continue;
                if (mapMn.dung_room_info2D[nextZ, nextX] >= mapMn.max_room) continue;
                ++count;
                if (0 != Random.Range(0, count)) continue;
                thisChara.moveVec = new Vector3(x, 0, z);
            }
        }

        return (count != 0);
    }
}
