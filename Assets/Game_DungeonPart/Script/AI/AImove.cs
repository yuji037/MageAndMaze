using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AImove : MonoBehaviour {


    protected GameObject parent;
    protected BattleParticipant thisChara;
    protected MapManager mapMn;
    protected EnemyManager eneMn;
    public static int searchTargetRange = 2;

    // 大部屋など、敵がフロア全体を索敵出来る状態フラグ
    protected bool searchWholeFloor = false;

    // Use this for initialization
    protected virtual void Start ()
    {
        //parent = GameObject.Find("GameObjectParent");
        //thisChara = GetComponent<BattleParticipant>();
        //mapMn = parent.GetComponentInChildren<MapManager>();
        //eneMn = parent.GetComponentInChildren<EnemyManager>();
    }

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        thisChara = GetComponent<BattleParticipant>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();

        DungeonPartManager dMn = parent.GetComponentInChildren<DungeonPartManager>();
        searchWholeFloor = ( dMn.floor == 30 );
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public virtual bool GetMoveVec()
    {
        return false;
    }
    
    protected bool AStarN(Vector3 targetPos)
    {
        Vector3 moveVec = Vector3.zero;
        
        int[,] openPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//-1 : チェック前, 0 : 開いた, 1 : 閉じた
        int[,] distPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//目的地までの最小距離
        int[,] costPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//スコアとカウントの和
        int[,] countPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//何回目に開かれたか
        int[,] prevPointX = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//自分を開いたポイント
        int[,] prevPointZ = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//自分を開いたポイント

        //無限ループ回避用、減らしてもよい
        int maxLoop = MapManager.DUNGEON_HEIGHT * MapManager.DUNGEON_WIDTH;

        bool notArrive = false;

        //現在のチェック地点(そして最初の開放地点に使用する)
        int nowX = (int)thisChara.pos.x;
        int nowZ = (int)thisChara.pos.z;
        //目的地
        int targetX = (int)targetPos.x;
        int targetZ = (int)targetPos.z;

        bool priority = false;
        if (nowX == targetX) priority = true;
        if (nowZ == targetZ) priority = true;

        for (int z = 0; z < MapManager.DUNGEON_HEIGHT; ++z)
        {
            for (int x = 0; x < MapManager.DUNGEON_WIDTH; ++x)
            {
                //壁の場合、閉じておく
                if ( mapMn.dung_2D[z, x] < 0 )
                {
                    openPoint[z, x] = 1;
                    costPoint[z, x] = 0;
                }
                else if ( mapMn.chara_exist2D[z, x] > -1 && !mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) )//キャラクタがいても閉じておく
                {
                    openPoint[z, x] = 1;
                    costPoint[z, x] = 0;
                }
                else
                {
                    //初期化(チェック前にして、最小距離を求める)
                    openPoint[z, x] = -1;
                    int d_x = Mathf.Abs(targetX - x);
                    int d_z = Mathf.Abs(targetZ - z);
                    distPoint[z, x] = ( d_z > d_x ) ? d_z : d_x;
                    costPoint[z, x] = 0;
                    countPoint[z, x] = 0;
                }
            }
        }

        prevPointX[nowZ, nowX] = -1;
        prevPointZ[nowZ, nowX] = -1;
        openPoint[nowZ, nowX] = 0;
        costPoint[nowZ, nowX] = 0;
        countPoint[nowZ, nowX] = 0;
        if (openPoint[targetZ, targetX] == 1)
        {
            openPoint[targetZ, targetX] = -1;
            notArrive = true;
        }
        int checkX = nowX, checkZ = nowZ;
        bool surround = false;

        for (int j = 0; j < maxLoop; ++j)
        {
            if (openPoint[checkZ, checkX] != 0)
            {
                //エラー
                Debug.Log("エラー : 到達し得ない");
                return false;
            }

            //現在のチェック地点を閉じる
            openPoint[checkZ, checkX] = 1;
            //ステージ端は除く
            bool top = (checkZ != MapManager.DUNGEON_HEIGHT - 1);
            bool bottom = (checkZ != 0);
            bool left = (checkX != 0);
            bool right = (checkX != MapManager.DUNGEON_WIDTH - 1);

            if (top)
            {
                int z = checkZ + 1;
                int x = checkX;
                //真上の判定
                if (openPoint[z, x] < 0)
                {
                    openPoint[z, x] = 0;
                    countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                    costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                    prevPointX[z, x] = checkX;
                    prevPointZ[z, x] = checkZ;
                }
                if (left)
                {
                    x = checkX - 1;
                    //左上の判定
                    if (openPoint[z, x] < 0)
                    {
                        // ↓ナナメに行くためには左と上の判定が必要
                        bool upWall = (mapMn.dung_2D[z, checkX] > -1);//真上が壁か
                        bool leftWall = (mapMn.dung_2D[checkZ, x] > -1);//左が壁か

                        if (upWall && leftWall)
                        {
                            openPoint[z, x] = 0;
                            countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                            costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                            prevPointX[z, x] = checkX;
                            prevPointZ[z, x] = checkZ;
                        }
                    }

                }
                if (right)
                {
                    x = checkX + 1;
                    //右上の判定
                    if (openPoint[z, x] < 0)
                    {
                        bool upWall = (mapMn.dung_2D[z, checkX] > -1);//真上が壁か
                        bool rightWall = (mapMn.dung_2D[checkZ, x] > -1);//右が壁か

                        if (upWall && rightWall)
                        {
                            openPoint[z, x] = 0;
                            countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                            costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                            prevPointX[z, x] = checkX;
                            prevPointZ[z, x] = checkZ;
                        }
                    }
                }

            }
            if (bottom)
            {
                int z = checkZ - 1;
                int x = checkX;
                //真下の判定
                if (openPoint[z, x] < 0)
                {
                    openPoint[z, x] = 0;
                    countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                    costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                    prevPointX[z, x] = checkX;
                    prevPointZ[z, x] = checkZ;
                }
                if (left)
                {
                    x = checkX - 1;
                    //左下の判定
                    if (openPoint[z, x] < 0)
                    {
                        bool downWall = (mapMn.dung_2D[z, checkX] > -1);//真下が壁か
                        bool leftWall = (mapMn.dung_2D[checkZ, x] > -1);//左が壁か

                        if (downWall && leftWall)
                        {
                            openPoint[z, x] = 0;
                            countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                            costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                            prevPointX[z, x] = checkX;
                            prevPointZ[z, x] = checkZ;
                        }
                    }

                }
                if (right)
                {
                    x = checkX + 1;
                    //右下の判定
                    if (openPoint[z, x] < 0)
                    {
                        bool downWall = (mapMn.dung_2D[z, checkX] > -1);//真下が壁か
                        bool rightWall = (mapMn.dung_2D[checkZ, x] > -1);//右が壁か

                        if (downWall && rightWall)
                        {
                            openPoint[z, x] = 0;
                            countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                            costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                            prevPointX[z, x] = checkX;
                            prevPointZ[z, x] = checkZ;
                        }
                    }
                }

            }
            {
                int x;
                int z = checkZ;
                if (left)
                {
                    x = checkX - 1;
                    //左の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }

                }
                if (right)
                {
                    x = checkX + 1;
                    //右の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x] + ( mapMn.IsBreakableObstacle(new Vector3(x, 0, z)) ? 2 : 0 );
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }
                }
            }

            int minX = -1;//次のチェックポイントを探す
            int minZ = -1;
            for (int i = 0; i < MapManager.DUNGEON_HEIGHT; ++i)
            {
                for (int k = 0; k < MapManager.DUNGEON_WIDTH; ++k)
                {
                    if (openPoint[i, k] != 0)
                        continue;

                    if (minX < 0)
                    {
                        minX = k;
                        minZ = i;
                        continue;
                    }

                    if (costPoint[i, k] < costPoint[minZ, minX])
                    {
                        minX = k;
                        minZ = i;
                    }
                    else if (costPoint[i, k] == costPoint[minZ, minX])
                    {
                        if (distPoint[i, k] < distPoint[minZ, minX])
                        {
                            minX = k;
                            minZ = i;
                        }
                        else if (distPoint[i, k] == distPoint[minZ, minX])
                        {
                            if (priority)//縦か横
                            {
                                int checkNow = (prevPointX[i, k] - k == 0) ? 0 : 1;
                                checkNow += (prevPointZ[i, k] - i == 0) ? 0 : 1;
                                int checkMin = (prevPointX[minZ, minX] - minX == 0) ? 0 : 1;
                                checkMin += (prevPointZ[minZ, minX] - minZ == 0) ? 0 : 1;
                                if (checkNow < checkMin)
                                {
                                    minX = k;
                                    minZ = i;
                                }
                            }
                            else//斜め
                            {
                                int checkTargetX = targetX - nowX;
                                if (checkTargetX < 0) checkTargetX = -1;
                                else if (checkTargetX > 0) checkTargetX = 1;
                                int checkTargetZ = targetZ - nowZ;
                                if (checkTargetZ < 0) checkTargetZ = -1;
                                else if (checkTargetZ > 0) checkTargetZ = 1;

                                int checkNowX = k - prevPointX[i, k];
                                if (checkNowX < 0) checkNowX = -1;
                                else if (checkNowX > 0) checkNowX = 1;
                                int checkNowZ = i - prevPointZ[i, k];
                                if (checkNowZ < 0) checkNowZ = -1;
                                else if (checkNowZ > 0) checkNowZ = 1;

                                int checkMinX = minX - prevPointX[minZ, minX];
                                if (checkMinX < 0) checkMinX = -1;
                                else if (checkMinX > 0) checkMinX = 1;
                                int checkMinZ = minZ - prevPointZ[minZ, minX];
                                if (checkMinZ < 0) checkMinZ = -1;
                                else if (checkMinZ > 0) checkMinZ = 1;

                                int ScoreNow = 0;
                                int ScoreMin = 0;
                                if (checkTargetX == checkNowX) ScoreNow++;
                                else if (checkTargetX == -checkNowX) ScoreNow--;
                                if (checkTargetZ == checkNowZ) ScoreNow++;
                                else if (checkTargetZ == -checkNowZ) ScoreNow--;
                                if (checkTargetX == checkMinX) ScoreMin++;
                                else if (checkTargetX == -checkMinX) ScoreMin--;
                                if (checkTargetZ == checkMinZ) ScoreMin++;
                                else if (checkTargetZ == -checkMinZ) ScoreMin--;

                                if (ScoreMin < ScoreNow)
                                {
                                    minX = k;
                                    minZ = i;
                                }
                            }
                        }
                    }
                }
            }

            if (minX == targetX)
                if (minZ == targetZ)
                    break;

            if (minZ < 0 || minX < 0)
            {
                //エラー
                //Debug.Log("目的地までたどり着けませんでした");
                surround = true;
                break;
            }

            checkX = minX;
            checkZ = minZ;
        }

        if (surround)
        {
            int minX = -1;
            int minZ = -1;
            for (int i = 0; i < MapManager.DUNGEON_HEIGHT; ++i)
            {
                for (int k = 0; k < MapManager.DUNGEON_WIDTH; ++k)
                {
                    if (0 < costPoint[i, k])
                    {
                        if (minX < 0)
                        {
                            minX = k;
                            minZ = i;
                        }
                        else if (costPoint[i, k] < costPoint[minZ, minX])
                        {
                            minX = k;
                            minZ = i;
                        }
                    }
                }
            }
            if (minX > -1 && minZ > -1)
            {
                int prevX = minX;
                int prevZ = minZ;
                for (int j = 0; j < maxLoop; ++j)
                {
                    if (prevPointX[prevZ, prevX] == nowX && prevPointZ[prevZ, prevX] == nowZ)
                    {
                        break;
                    }
                    else
                    {
                        int bufX = prevX;
                        prevX = prevPointX[prevZ, prevX];
                        prevZ = prevPointZ[prevZ, bufX];
                    }
                }

                thisChara.moveVec = new Vector3(prevX, 0, prevZ) - thisChara.pos;
                return true;
            }
        }
        else
        {
            int prevX = targetX;
            int prevZ = targetZ;
            int bufX = prevX;
            for (int j = 0; j < maxLoop; ++j)
            {
                if (prevPointX[prevZ, prevX] == nowX && prevPointZ[prevZ, prevX] == nowZ)
                {
                    break;
                }
                else
                {
                    bufX = prevX;
                    prevX = prevPointX[prevZ, prevX];
                    prevZ = prevPointZ[prevZ, bufX];
                }
            }
            if (notArrive)
                if (prevX == targetX)
                    if (prevZ == targetZ)
                    {
                        //Debug.Log("near");
                        int bestX = 0, bestZ = 0;
                        int bestScore = 0;
                        int checkTargetX = targetX - nowX;
                        if (checkTargetX < 0) checkTargetX = -1;
                        else if (checkTargetX > 0) checkTargetX = 1;
                        int checkTargetZ = targetZ - nowZ;
                        if (checkTargetZ < 0) checkTargetZ = -1;
                        else if (checkTargetZ > 0) checkTargetZ = 1;
                        for (int x = -1; x < 2; ++x)
                        {
                            if (nowX + x < 0 || nowX + x > MapManager.DUNGEON_WIDTH - 1)
                                continue;
                            for (int z = -1; z < 2; ++z)
                            {
                                if (nowZ + z < 0 || nowZ + z > MapManager.DUNGEON_HEIGHT - 1)
                                    continue;
                                if (x == 0 && z == 0)
                                    continue;
                                if (mapMn.chara_exist2D[nowZ + z, nowX + x] > -1
                                    && !mapMn.IsBreakableObstacle(new Vector3(nowX + x, 0, nowZ + z))) continue;
                                if (mapMn.dung_2D[nowZ + z, nowX + x] < 0) continue;
                                if (mapMn.dung_2D[nowZ, nowX + x] < 0) continue;
                                if (mapMn.dung_2D[nowZ + z, nowX] < 0) continue;
                                int nowScore = 0;
                                if (checkTargetX == 0)
                                {
                                    if (x == 0) nowScore += (checkTargetZ == z) ? 1 : -1;
                                }
                                else
                                {
                                    nowScore += (checkTargetX == x) ? 1 : 0;
                                    nowScore += (checkTargetX == -x) ? -1 : 0;
                                }
                                if (checkTargetZ == 0)
                                {
                                    if (z == 0) nowScore += (checkTargetX == x) ? 1 : -1;
                                }
                                else
                                {
                                    nowScore += (checkTargetZ == z) ? 1 : 0;
                                    nowScore += (checkTargetZ == -z) ? -1 : 0;
                                }
                                if (nowScore > bestScore)
                                {
                                    bestScore = nowScore;
                                    bestX = x;
                                    bestZ = z;
                                }
                                else if (bestX == 0 && bestZ == 0)
                                {
                                    bestScore = nowScore;
                                    bestX = x;
                                    bestZ = z;
                                }
                            }
                        }
                        thisChara.moveVec = new Vector3(bestX, 0, bestZ);
                        return true;
                    }
            thisChara.moveVec = new Vector3(prevX, 0, prevZ) - thisChara.pos;
            return true;
        }
        return false;
    }

    protected bool AStarG(Vector3 targetPos)
    {
        Vector3 moveVec = Vector3.zero;

        int[,] openPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//-1 : チェック前, 0 : 開いた, 1 : 閉じた
        int[,] distPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//目的地までの最小距離
        int[,] costPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//スコアとカウントの和
        int[,] countPoint = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//何回目に開かれたか
        int[,] prevPointX = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//自分を開いたポイント
        int[,] prevPointZ = new int[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];//自分を開いたポイント

        //無限ループ回避用、減らしてもよい
        int maxLoop = MapManager.DUNGEON_HEIGHT * MapManager.DUNGEON_WIDTH;

        bool notArrive = false;

        //現在のチェック地点(そして最初の開放地点に使用する)
        int nowX = (int)thisChara.pos.x;
        int nowZ = (int)thisChara.pos.z;
        //目的地
        int targetX = (int)targetPos.x;
        int targetZ = (int)targetPos.z;

        bool YUSE = false;
        if (nowX == targetX) YUSE = true;
        if (nowZ == targetZ) YUSE = true;

        for (int i = 0; i < MapManager.DUNGEON_HEIGHT; ++i)
        {
            for (int k = 0; k < MapManager.DUNGEON_WIDTH; ++k)
            {
                if (mapMn.chara_exist2D[i, k] > -1)//キャラクタがいても閉じておく
                {
                    openPoint[i, k] = 1;
                    costPoint[i, k] = 0;
                }
                else
                {
                    //初期化(チェック前にして、最小距離を求める)
                    openPoint[i, k] = -1;
                    int d_x = Mathf.Abs(targetX - k);
                    int d_z = Mathf.Abs(targetZ - i);
                    distPoint[i, k] = (d_z > d_x) ? d_z : d_x;
                    costPoint[i, k] = 0;
                    countPoint[i, k] = 0;
                }
            }
        }

        prevPointX[nowZ, nowX] = -1;
        prevPointZ[nowZ, nowX] = -1;
        openPoint[nowZ, nowX] = 0;
        costPoint[nowZ, nowX] = 0;
        countPoint[nowZ, nowX] = 0;
        if (openPoint[targetZ, targetX] == 1)
        {
            openPoint[targetZ, targetX] = -1;
            notArrive = true;
        }
        int checkX = nowX, checkZ = nowZ;
        bool surround = false;

        for (int j = 0; j < maxLoop; ++j)
        {
            if (openPoint[checkZ, checkX] != 0)
            {
                //エラー
                Debug.Log("エラー ");
                return false;
            }

            //現在のチェック地点を閉じる
            openPoint[checkZ, checkX] = 1;
            //ステージ端は除く
            bool top = (checkZ != MapManager.DUNGEON_HEIGHT - 1);
            bool bottom = (checkZ != 0);
            bool left = (checkX != 0);
            bool right = (checkX != MapManager.DUNGEON_WIDTH - 1);

            if (top)
            {
                int z = checkZ + 1;
                int x = checkX;
                //真上の判定
                if (openPoint[z, x] < 0)
                {
                    openPoint[z, x] = 0;
                    countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                    costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                    prevPointX[z, x] = checkX;
                    prevPointZ[z, x] = checkZ;
                }
                if (left)
                {
                    x = checkX - 1;
                    //左上の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }

                }
                if (right)
                {
                    x = checkX + 1;
                    //右上の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }
                }

            }
            if (bottom)
            {
                int z = checkZ - 1;
                int x = checkX;
                //真下の判定
                if (openPoint[z, x] < 0)
                {
                    openPoint[z, x] = 0;
                    countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                    costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                    prevPointX[z, x] = checkX;
                    prevPointZ[z, x] = checkZ;
                }
                if (left)
                {
                    x = checkX - 1;
                    //左下の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }

                }
                if (right)
                {
                    x = checkX + 1;
                    //右下の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }
                }

            }
            {
                int x;
                int z = checkZ;
                if (left)
                {
                    x = checkX - 1;
                    //左の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }

                }
                if (right)
                {
                    x = checkX + 1;
                    //右の判定
                    if (openPoint[z, x] < 0)
                    {
                        openPoint[z, x] = 0;
                        countPoint[z, x] = countPoint[checkZ, checkX] + 1;
                        costPoint[z, x] = countPoint[z, x] + distPoint[z, x];
                        prevPointX[z, x] = checkX;
                        prevPointZ[z, x] = checkZ;
                    }
                }
            }

            int minX = -1;//次のチェックポイントを探す
            int minZ = -1;
            for (int i = 0; i < MapManager.DUNGEON_HEIGHT; ++i)
            {
                for (int k = 0; k < MapManager.DUNGEON_WIDTH; ++k)
                {
                    if (openPoint[i, k] != 0)
                        continue;

                    if (minX < 0)
                    {
                        minX = k;
                        minZ = i;
                        continue;
                    }

                    if (costPoint[i, k] < costPoint[minZ, minX])
                    {
                        minX = k;
                        minZ = i;
                    }
                    else if (costPoint[i, k] == costPoint[minZ, minX])
                    {
                        if (distPoint[i, k] < distPoint[minZ, minX])
                        {
                            minX = k;
                            minZ = i;
                        }
                        else if (distPoint[i, k] == distPoint[minZ, minX])
                        {
                            if (YUSE)//縦か横
                            {
                                int checkNow = (prevPointX[i, k] - k == 0) ? 0 : 1;
                                checkNow += (prevPointZ[i, k] - i == 0) ? 0 : 1;
                                int checkMin = (prevPointX[minZ, minX] - minX == 0) ? 0 : 1;
                                checkMin += (prevPointZ[minZ, minX] - minZ == 0) ? 0 : 1;
                                if (checkNow < checkMin)
                                {
                                    minX = k;
                                    minZ = i;
                                }
                            }
                            else//斜め
                            {
                                int checkTargetX = targetX - nowX;
                                if (checkTargetX < 0) checkTargetX = -1;
                                else if (checkTargetX > 0) checkTargetX = 1;
                                int checkTargetZ = targetZ - nowZ;
                                if (checkTargetZ < 0) checkTargetZ = -1;
                                else if (checkTargetZ > 0) checkTargetZ = 1;

                                int checkNowX = k - prevPointX[i, k];
                                if (checkNowX < 0) checkNowX = -1;
                                else if (checkNowX > 0) checkNowX = 1;
                                int checkNowZ = i - prevPointZ[i, k];
                                if (checkNowZ < 0) checkNowZ = -1;
                                else if (checkNowZ > 0) checkNowZ = 1;

                                int checkMinX = minX - prevPointX[minZ, minX];
                                if (checkMinX < 0) checkMinX = -1;
                                else if (checkMinX > 0) checkMinX = 1;
                                int checkMinZ = minZ - prevPointZ[minZ, minX];
                                if (checkMinZ < 0) checkMinZ = -1;
                                else if (checkMinZ > 0) checkMinZ = 1;

                                int ScoreNow = 0;
                                int ScoreMin = 0;
                                if (checkTargetX == checkNowX) ScoreNow++;
                                else if (checkTargetX == -checkNowX) ScoreNow--;
                                if (checkTargetZ == checkNowZ) ScoreNow++;
                                else if (checkTargetZ == -checkNowZ) ScoreNow--;
                                if (checkTargetX == checkMinX) ScoreMin++;
                                else if (checkTargetX == -checkMinX) ScoreMin--;
                                if (checkTargetZ == checkMinZ) ScoreMin++;
                                else if (checkTargetZ == -checkMinZ) ScoreMin--;

                                if (ScoreMin < ScoreNow)
                                {
                                    minX = k;
                                    minZ = i;
                                }
                            }
                        }
                    }
                }
            }

            if (minX == targetX)
                if (minZ == targetZ)
                    break;

            if (minZ < 0 || minX < 0)
            {
                //エラー
                //Debug.Log("目的地までたどり着けませんでした");
                surround = true;
                break;
            }

            checkX = minX;
            checkZ = minZ;
        }

        if (surround)
        {
            int minX = -1;
            int minZ = -1;
            for (int i = 0; i < MapManager.DUNGEON_HEIGHT; ++i)
            {
                for (int k = 0; k < MapManager.DUNGEON_WIDTH; ++k)
                {
                    if (0 < costPoint[i, k])
                    {
                        if (minX < 0)
                        {
                            minX = k;
                            minZ = i;
                        }
                        else if (costPoint[i, k] < costPoint[minZ, minX])
                        {
                            minX = k;
                            minZ = i;
                        }
                    }
                }
            }
            if (minX > -1 && minZ > -1)
            {
                int prevX = minX;
                int prevZ = minZ;
                for (int j = 0; j < maxLoop; ++j)
                {
                    if (prevPointX[prevZ, prevX] == nowX && prevPointZ[prevZ, prevX] == nowZ)
                    {
                        break;
                    }
                    else
                    {
                        int bufX = prevX;
                        prevX = prevPointX[prevZ, prevX];
                        prevZ = prevPointZ[prevZ, bufX];
                    }
                }

                thisChara.moveVec = new Vector3(prevX, 0, prevZ) - thisChara.pos;
                return true;
            }
        }
        else
        {
            int prevX = targetX;
            int prevZ = targetZ;
            int bufX = prevX;
            for (int j = 0; j < maxLoop; ++j)
            {
                if (prevPointX[prevZ, prevX] == nowX && prevPointZ[prevZ, prevX] == nowZ)
                {
                    break;
                }
                else
                {
                    bufX = prevX;
                    prevX = prevPointX[prevZ, prevX];
                    prevZ = prevPointZ[prevZ, bufX];
                }
            }
            if (notArrive)
                if (prevX == targetX)
                    if (prevZ == targetZ)
                    {
                        Debug.Log("near");
                        int bestX = 0, bestZ = 0;
                        int bestScore = 0;
                        int checkTargetX = targetX - nowX;
                        if (checkTargetX < 0) checkTargetX = -1;
                        else if (checkTargetX > 0) checkTargetX = 1;
                        int checkTargetZ = targetZ - nowZ;
                        if (checkTargetZ < 0) checkTargetZ = -1;
                        else if (checkTargetZ > 0) checkTargetZ = 1;
                        for (int x = -1; x < 2; ++x)
                        {
                            if (nowX + x < 0 || nowX + x > MapManager.DUNGEON_WIDTH - 1)
                                continue;
                            for (int z = -1; z < 2; ++z)
                            {
                                if (nowZ + z < 0 || nowZ + z > MapManager.DUNGEON_HEIGHT - 1)
                                    continue;
                                if (x == 0 && z == 0)
                                    continue;
                                //ここから
                                if (mapMn.chara_exist2D[nowZ + z, nowX + x] > -1) continue;
                                int nowScore = 0;
                                if (checkTargetX == 0)
                                {
                                    if (x == 0) nowScore += (checkTargetZ == z) ? 1 : -1;
                                }
                                else
                                {
                                    nowScore += (checkTargetX == x) ? 1 : 0;
                                    nowScore += (checkTargetX == -x) ? -1 : 0;
                                }
                                if (checkTargetZ == 0)
                                {
                                    if (z == 0) nowScore += (checkTargetX == x) ? 1 : -1;
                                }
                                else
                                {
                                    nowScore += (checkTargetZ == z) ? 1 : 0;
                                    nowScore += (checkTargetZ == -z) ? -1 : 0;
                                }
                                if (nowScore > bestScore)
                                {
                                    bestScore = nowScore;
                                    bestX = x;
                                    bestZ = z;
                                }
                                else if (bestX == 0 && bestZ == 0)
                                {
                                    bestScore = nowScore;
                                    bestX = x;
                                    bestZ = z;
                                }
                            }
                        }
                        thisChara.moveVec = new Vector3(bestX, 0, bestZ);
                        return true;
                    }
            thisChara.moveVec = new Vector3(prevX, 0, prevZ) - thisChara.pos;
            return true;
        }
        return false;
    }

    protected bool AStarR(Vector3 targetPos)
    {
        const int deep = 6;//探索深さ 
        //pow(8, deep)のオーダーで増加の可能性あり deep > 8 を控える
        int x = (int)thisChara.pos.x;//関数に渡す際は自分の居場所
        int z = (int)thisChara.pos.z;//関数から受け取るときは次に移動すべき方向(-1 ~ 1)
        
        int ans = RunAwayMinMax(ref x, ref z, (int)targetPos.x, (int)targetPos.z, 0, deep, -1);

        thisChara.moveVec = new Vector3(x, 0, z);
        return (x != 0 || z != 0);
    }

    int RunAwayScore(int _thisX, int _thisZ, int _targetX, int _targetZ)
    {
        int d_x = Mathf.Abs(_targetX - _thisX);
        int d_z = Mathf.Abs(_targetZ - _thisZ);
        int ans = (d_z > d_x) ? d_z : d_x;
        if (mapMn.dung_room_info2D[_targetZ, _targetX] != mapMn.dung_room_info2D[_thisZ, _thisX]) ++ans;
        return ans;
    }

    int RunAwayMinMax(ref int _thisX, ref int _thisZ, int _targetX, int _targetZ, int _count, int _deep, int minMax)
    {
        int ans = RunAwayScore(_thisX, _thisZ, _targetX, _targetZ);
        int score;
        ++_count;
        int thisX = _thisX, thisZ = _thisZ;
        int targetX = _targetX, targetZ = _targetZ;
        int firstAns = 4;
        bool cutFlag = false;
        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                if (x == 0 && z == 0) continue;
                if (_count % 2 == 1)
                {
                    thisX = _thisX + x;
                    thisZ = _thisZ + z;
                    //マップの範囲を超えるものは除外
                    if (thisX < 0) continue;
                    if (thisX > MapManager.DUNGEON_WIDTH - 1) continue;
                    if (thisZ < 0) continue;
                    if (thisZ > MapManager.DUNGEON_HEIGHT - 1) continue;
                    //壁の場合、移動不能
                    if (mapMn.dung_2D[thisZ, thisX] < 0) continue;
                    //他のキャラクターがいる場合、移動不能
                    if (mapMn.chara_exist2D[thisZ, thisX] > -1) continue;
                    //斜め移動の際は縦もしくは横に壁がある場合、移動不能
                    if (x != 0 && z != 0)
                    {
                        if (mapMn.dung_2D[_thisZ, thisX] < 0) continue;
                        if (mapMn.dung_2D[thisZ, _thisX] < 0) continue;
                    }
                }
                else
                {
                    targetX = _targetX + x;
                    targetZ = _targetZ + z;
                    //マップの範囲を超えるものは除外
                    if (targetX < 0) continue;
                    if (targetX > MapManager.DUNGEON_WIDTH - 1) continue;
                    if (targetZ < 0) continue;
                    if (targetZ > MapManager.DUNGEON_HEIGHT - 1) continue;
                    //壁の場合、移動不能
                    if (mapMn.dung_2D[targetZ, targetX] < 0) continue;
                    //他のキャラクターがいる場合、移動不能
                    if (mapMn.chara_exist2D[targetZ, targetX] > -1) continue;
                    //斜め移動の際は縦もしくは横に壁がある場合、移動不能
                    if (x != 0 && z != 0)
                    {
                        if (mapMn.dung_2D[_targetZ, targetX] < 0) continue;
                        if (mapMn.dung_2D[targetZ, _targetX] < 0) continue;
                    }
                }

                if (_count < _deep)
                {
                    if (cutFlag)
                        score = RunAwayMinMax(ref thisX, ref thisZ, targetX, targetZ, _count, _deep, ans);
                    else
                        score = RunAwayMinMax(ref thisX, ref thisZ, targetX, targetZ, _count, _deep, -1);
                }
                else
                    score = RunAwayScore(thisX, thisZ, targetX, targetZ);

                if (_count % 2 == 1)
                {
                    if (ans > score && firstAns != 4) continue;
                    //ベータカット
                    if (minMax > -1 && score > minMax) { ans = score; break; }
                }
                else
                {
                    if (ans < score && firstAns != 4) continue;
                    //アルファカット
                    if (minMax > -1 && score < minMax) { ans = score; break; }
                }
                ans = score;
                firstAns = (x + 1) * 3 + (z + 1);
                cutFlag = true;
            }
        }
        if (_count == 1)
        {
            _thisX = (firstAns / 3) - 1;
            _thisZ = (firstAns % 3) - 1;
        }
        return ans;
    }
}
