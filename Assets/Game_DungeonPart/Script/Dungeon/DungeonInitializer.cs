using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInitializer : MonoBehaviour {

    GameObject parent;
    DungeonPartManager dMn;
    [SerializeField] GameObject player;
    MapManager mapMn;
    EnemyManager enemyMn;
    OnGroundObjectManager groundObjMn;
    ObstacleManager obsMn;
    TutorialManager tutorialMn;
    int width;
    int height;
    [SerializeField] int enemyCount = 5;

    public int eneCount = 1;

    // キャラが部屋だけでなく道の上にポップすることを許可するかどうか
    public bool AllowCharaOnRoad = false;
    // 敵のランダムポップをするかどうか
    public bool PopRandomEnemys = true;
    // 特殊床のランダムポップをするかどうか
    public bool PopRandomGroundObjects = true;
    // 障害物のランダムポップをするかどうか
    public bool PopRandomObstacles = true;

    // 難易度調整用
    // playerCloseEnemyMax + 1匹目以降はプレイヤーから半径 closeRangeMin 以下の場所にはスポーンさせない
    int playerCloseEnemyMax = 2;
    float closeRangeMin = 7;

    // プレイヤー、階段が固定位置に出現する場合の位置
    public Vector3 fixedPlayerPos = Vector3.one * -1;
    public Vector3 fixedStairsPos = Vector3.one * -1;

    private void Awake()
    {
        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        width = MapManager.DUNGEON_WIDTH;
        height = MapManager.DUNGEON_HEIGHT;

        enemyMn = parent.GetComponentInChildren<EnemyManager>();
        enemyMn.d_init = this;
        groundObjMn = parent.GetComponentInChildren<OnGroundObjectManager>();
        groundObjMn.d_init = this;
        obsMn = parent.GetComponentInChildren<ObstacleManager>();
        obsMn.d_init = this;
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();
    }

    public void DungeonDataInit()
    {
        //キャラなどの配置
        PlayerStartPosDecide();

        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            enemyMn.LoadEnemys();
        }
        else if ( PopRandomEnemys )
        {
            while ( eneCount <= enemyCount )
            {
                EnemySet();
            }
            // 確率を満たせばNPCが出現
            int random = Random.Range(0, 100);
            if ( dMn.floor != 30 && random < 5 )
            {
                var npc = EnemySet();
                if (npc) npc.isSpeakable = true;
            }
            DebugMessage.UpdateText();
        }
        else
        {

        }

        // 回復パネル等床オブジェクトの配置
        if (PopRandomGroundObjects) groundObjMn.Init();

        // 爆弾、岩ブロックなど障害物の配置
        if (PopRandomObstacles) obsMn.Init();

        if ( tutorialMn.IsTutorialON ) enemyMn.NonPatrolMode();
    }


    void PlayerStartPosDecide()
    {
        player.GetComponent<PlayerMove>().charaID = 1;

        Vector3 pos;
        Vector3 charaDir = new Vector3(0, 0, 1);
        // ボスマップなど、プレイヤーの位置が固定されてるかどうか
        bool isFixedPlayerPos = ( fixedPlayerPos.x != -1 );
        //プレイヤー位置の決定
        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            Player.PosData _data = SaveData.GetClass("PlayerPosData", new Player.PosData());
            pos = new Vector3(_data.PosX, 0, _data.PosZ);
            charaDir = new Vector3(_data.DirX, 0, _data.DirZ);
        }
        else if (!isFixedPlayerPos)
        {
            // プレイヤーはランダム位置
            pos = GetRandomPos();
            charaDir = Calc.RandomDir();
            // キャラの位置を配列に入れて予約（他とかぶらないようにする）
            mapMn.chara_exist2D[(int)pos.z, (int)pos.x] = 1;
        }
        else
        {
            // プレイヤーは固定位置
            pos = fixedPlayerPos;
            charaDir = new Vector3(0, 0, 1);
            mapMn.chara_exist2D[(int)pos.z, (int)pos.x] = 1;
        }
        player.transform.position = pos;
        Player pl = player.GetComponent<Player>();
        pl.pos = pos;
        pl.charaDir = charaDir;
        pl.init = false;
        
    }

    // ランダム位置に敵を配置する場合
    Enemy EnemySet(EnemyType fixedType = (EnemyType)(-1))
    {
        float sqrPlayerCloseRange = 0;
        Vector3 pos = Vector3.zero;

        // プレイヤーに近い敵が一定数以上にならないよう難易度調整
        do
        {
            pos = GetRandomPos();
            if ( pos.x == -1 )
            {
                // マップ範囲外なので生成不可、これ以上の生成をしない
                eneCount = enemyCount + 1;
                Debug.Log("敵生成：マップに許容範囲が少なく、これ以上生成できません。");
                return null;
            }
            sqrPlayerCloseRange = ( pos - player.transform.position ).sqrMagnitude;
        } while ( eneCount <= playerCloseEnemyMax && sqrPlayerCloseRange < closeRangeMin * closeRangeMin );

        var ene = enemyMn.EnemyAdd(pos, fixedType);
        if ( !ene ) return null;
        if ( (int)fixedType != -1 && ene) ene.type = fixedType;

        // キャラの位置を配列に入れて予約（他とかぶらないようにする）
        mapMn.chara_exist2D[(int)pos.z, (int)pos.x] = ene.idNum;

        eneCount++;

        return ene;
    }

    // 固定位置に敵を配置する場合
    public Enemy EnemySet(Vector3 fixPos, EnemyType fixedType = (EnemyType)( -1 ))
    {
        if ( fixPos.x == -1 )
        {
            // マップ範囲外なので生成不可、これ以上の生成をしない
            eneCount = enemyCount + 1;
            Debug.Log("敵生成：マップに許容範囲が少なく、これ以上生成できません。");
            return null;
        }

        var ene = enemyMn.EnemyAdd(fixPos);
        if ( !ene ) return null;
        if ( (int)fixedType != -1 && ene ) ene.type = fixedType;

        // キャラの位置を配列に入れて予約（他とかぶらないようにする）
        mapMn.chara_exist2D[(int)fixPos.z, (int)fixPos.x] = ene.idNum;

        eneCount++;
        return ene;
    }

    public Vector3 StairsPosDecide()
    {
        Vector3 pos;
        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            // 中断した場合はロード
            pos = new Vector3(SaveData.GetInt("StairX", 0), 0, SaveData.GetInt("StairZ", 0));
        }
        else if ( fixedStairsPos.x != -1 )
        {
            // 固定マップなど階段の位置が固定している場合
            pos = fixedStairsPos;
        }
        else
        {
            // それ以外ならばランダム
            pos = GetRandomPos();
        }

        //if (dMn.floor == 8) pos = new Vector3(23, 0, 16);
        
        // 中断用にセーブしておく
        SaveData.SetInt("StairX", (int)pos.x);
        SaveData.SetInt("StairZ", (int)pos.z);
        return pos;
    }

    public Vector3 GetRandomPos()
    {
        int px = Random.Range(0, width);
        int pz = Random.Range(0, height);
        int attempt = 0;

        while (mapMn.IsWall(new Vector3(px,0,pz)) //壁である 
            || (!AllowCharaOnRoad && mapMn.GetDungeonInfo(px, pz) >= mapMn.max_room )  // 通路である
            || mapMn.chara_exist2D[pz,px] != -1  //既にキャラが存在する
            || mapMn.onground_exist2D[pz,px] != -1) //既に床オブジェクトが存在する
        {
            px = Random.Range(0, width);
            pz = Random.Range(0, height);
            attempt++;
            if ( attempt >= 30 )
            {
                Debug.Log("敵の初期位置のランダム設定に失敗、場外に配置しました。");
                return new Vector3(-1, 0, -1);
            }
        }

        return new Vector3(px, 0, pz);
    }
}
