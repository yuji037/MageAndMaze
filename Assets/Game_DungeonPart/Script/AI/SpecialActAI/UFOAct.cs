using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOAct : SpecialActAI
{
    private int range = 0;//プレイヤーとの発動可能な距離（マス）
    //自分も移動させる
    public override void Init()
    {
        base.Init();

        range = 1;
        probability = 50;
    }

    public bool FindOtherMonster()
    {
        int myNumber;
        bool canCarry = false;

        if (GetComponent<Enemy>() != null)
            myNumber = GetComponent<Enemy>().idNum;
        else
            return false;

        foreach (Enemy ene in eneMn.enemys)
        {
            if (ene.idNum == myNumber) continue;

            //同じ部屋なら
            //if (0 > thisChara.existRoomNum || thisChara.existRoomNum > mapMn.max_room)
            //{
            //    int d_x = Mathf.Abs((int)(ene.sPos.x - thisChara.pos.x));
            //    int d_z = Mathf.Abs((int)(ene.sPos.z - thisChara.pos.z));
            //    if (d_x > searchTargetRange) continue;
            //    if (d_z > searchTargetRange) continue;
            //}

            canCarry = false;
            int enemyX = 0;
            int enemyZ = 0;
            for (int i = -1; i < 2; ++i)
            {
                enemyX = (int)ene.pos.x;
                enemyZ = (int)ene.pos.z;
                if (i == -1)
                {
                    if (enemyX == 0)
                        continue;
                }
                else if (i == 1)
                    if (enemyX == MapManager.DUNGEON_WIDTH - 1)
                        continue;
                for (int k = -1; k < 2; ++k)
                {
                    if (k == -1)
                    {
                        if (enemyZ == 0)
                            continue;
                    }
                    else if (k == 1)
                        if (enemyZ == MapManager.DUNGEON_HEIGHT - 1)
                            continue;

                    int x = enemyX + i;
                    int z = enemyZ + k;
                    if (mapMn.dung_2D[z, x] < 0) continue;
                    if (mapMn.chara_exist2D[z, x] > -1) continue;

                    bool top = (z != MapManager.DUNGEON_HEIGHT - 1);
                    bool bottom = (z != 0);
                    bool right = (x != MapManager.DUNGEON_WIDTH - 1);
                    bool left = (x != 0);

                    if (top)
                    {
                        if (mapMn.dung_2D[z + 1, x] > -1)
                            if (mapMn.chara_exist2D[z + 1, x] < 0)
                            {
                                canCarry = true;
                                break;
                            }
                        if (left)
                            if (mapMn.dung_2D[z + 1, x - 1] > -1)
                                if (mapMn.chara_exist2D[z + 1, x - 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                        if (right)
                            if (mapMn.dung_2D[z + 1, x + 1] > -1)
                                if (mapMn.chara_exist2D[z + 1, x + 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                    }
                    if (bottom)
                    {
                        if (mapMn.dung_2D[z - 1, x] > -1)
                            if (mapMn.chara_exist2D[z - 1, x] < 0)
                            {
                                canCarry = true;
                                break;
                            }
                        if (left)
                            if (mapMn.dung_2D[z - 1, x - 1] > -1)
                                if (mapMn.chara_exist2D[z - 1, x - 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                        if (right)
                            if (mapMn.dung_2D[z - 1, x + 1] > -1)
                                if (mapMn.chara_exist2D[z - 1, x + 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                    }
                    {
                        if (left)
                            if (mapMn.dung_2D[z, x - 1] > -1)
                                if (mapMn.chara_exist2D[z - 1, x - 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                        if (right)
                            if (mapMn.dung_2D[z, x + 1] > -1)
                                if (mapMn.chara_exist2D[z - 1, x + 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                    }

                }
                if (canCarry) break;
            }
            if (canCarry) break;
        }

        return canCarry;
    }

    public override bool ShouldSpecialAct()
    {
        //特殊行動を行うかの判定
        int rnd = Random.Range(0, 100);
        if ( rnd >= probability ) return false;

        if (!FindTarget()) return false;

        int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
        int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
        if (d_x > range) return false;//xの範囲を超えている場合は除外
        if (d_z > range) return false;//zの範囲を超えている場合は除外

        return FindOtherMonster();
    }
}
