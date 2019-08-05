using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInitializer : MonoBehaviour {

    GameObject				m_oParent;
    DungeonPartManager		m_cDungeonMgr;
    [SerializeField]
	GameObject				player;
    MapManager				m_cMapMgr;
    EnemyManager			m_cEnemyMgr;
    OnGroundObjectManager	m_cOnGroundObjMgr;
    ObstacleManager			m_cObstacleMgr;
    TutorialManager			m_cTutorialMgr;
    int						m_iWidth;
    int						m_iHeight;
    [SerializeField]
	int						enemyCount = 5;

    public int				eneCount = 1;

    // キャラが部屋だけでなく道の上にポップすることを許可するかどうか
    public bool AllowCharaOnRoad		= false;
    // 敵のランダムポップをするかどうか
    public bool PopRandomEnemys			= true;
    // 特殊床のランダムポップをするかどうか
    public bool PopRandomGroundObjects	= true;
    // 障害物のランダムポップをするかどうか
    public bool PopRandomObstacles		= true;

	// フロア開始時、近くに沸く敵の数を制限する
    int			m_iPlayerCloseEnemyMax		= 2; // +1匹目以降はプレイヤーから
	// 半径		
    float		m_fCloseRangeMin			= 7; // 以下の場所にはスポーンさせない

	// プレイヤー、階段が固定位置に出現する場合の位置
	public Vector3 fixedPlayerPos = Vector3.one * -1;
    public Vector3 fixedStairsPos = Vector3.one * -1;

    private void Awake()
    {
        m_oParent					= GameObject.Find("GameObjectParent");
        m_cDungeonMgr				= m_oParent.GetComponentInChildren<DungeonPartManager>();
        m_cMapMgr					= m_oParent.GetComponentInChildren<MapManager>();
        m_iWidth					= MapManager.DUNGEON_WIDTH;
        m_iHeight					= MapManager.DUNGEON_HEIGHT;

        m_cEnemyMgr					= m_oParent.GetComponentInChildren<EnemyManager>();
        m_cEnemyMgr.d_init			= this;
        m_cOnGroundObjMgr			= m_oParent.GetComponentInChildren<OnGroundObjectManager>();
        m_cOnGroundObjMgr.d_init	= this;
        m_cObstacleMgr				= m_oParent.GetComponentInChildren<ObstacleManager>();
        m_cObstacleMgr.d_init		= this;
        m_cTutorialMgr				= m_oParent.GetComponentInChildren<TutorialManager>();
    }

    public void DungeonDataInit()
    {
        //キャラなどの配置
        PlayerStartPosDecide();

        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            m_cEnemyMgr.LoadEnemys();
        }
        else if ( PopRandomEnemys )
        {
            while ( eneCount <= enemyCount )
            {
                EnemySet();
            }
            // 確率を満たせばNPCが出現
            int random = Random.Range(0, 100);
            if ( m_cDungeonMgr.floor != 30 && random < 100 )
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
        if (PopRandomGroundObjects) m_cOnGroundObjMgr.Init();

        // 爆弾、岩ブロックなど障害物の配置
        if (PopRandomObstacles) m_cObstacleMgr.Init();

        if ( m_cTutorialMgr.IsTutorialON ) m_cEnemyMgr.NonPatrolMode();
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
            m_cMapMgr.chara_exist2D[(int)pos.z, (int)pos.x] = 1;
        }
        else
        {
            // プレイヤーは固定位置
            pos = fixedPlayerPos;
            charaDir = new Vector3(0, 0, 1);
            m_cMapMgr.chara_exist2D[(int)pos.z, (int)pos.x] = 1;
        }
        player.transform.position = pos;
        Player pl = player.GetComponent<Player>();
        pl.pos = pos;
        pl.charaDir = charaDir;
        pl.init = false;
        
    }

    // ランダム位置に敵を配置する場合
    Enemy EnemySet(eEnemyType fixedType = (eEnemyType)(-1))
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
        } while ( eneCount <= m_iPlayerCloseEnemyMax && sqrPlayerCloseRange < m_fCloseRangeMin * m_fCloseRangeMin );

        var ene = m_cEnemyMgr.EnemyAdd(pos, fixedType);
        if ( !ene ) return null;
        if ( (int)fixedType != -1 && ene) ene.type = fixedType;

        // キャラの位置を配列に入れて予約（他とかぶらないようにする）
        m_cMapMgr.chara_exist2D[(int)pos.z, (int)pos.x] = ene.idNum;

        eneCount++;

        return ene;
    }

    // 固定位置に敵を配置する場合
    public Enemy EnemySet(Vector3 fixPos, eEnemyType fixedType = (eEnemyType)( -1 ))
    {
        if ( fixPos.x == -1 )
        {
            // マップ範囲外なので生成不可、これ以上の生成をしない
            eneCount = enemyCount + 1;
            Debug.Log("敵生成：マップに許容範囲が少なく、これ以上生成できません。");
            return null;
        }

        var ene = m_cEnemyMgr.EnemyAdd(fixPos, fixedType);
        if ( !ene ) return null;
        if ( (int)fixedType != -1 && ene ) ene.type = fixedType;

        // キャラの位置を配列に入れて予約（他とかぶらないようにする）
        m_cMapMgr.chara_exist2D[(int)fixPos.z, (int)fixPos.x] = ene.idNum;

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
        int px = Random.Range(0, m_iWidth);
        int pz = Random.Range(0, m_iHeight);
        int attempt = 0;

        while (m_cMapMgr.IsWall(new Vector3(px,0,pz)) //壁である 
            || (!AllowCharaOnRoad && m_cMapMgr.GetDungeonInfo(px, pz) >= m_cMapMgr.max_room )  // 通路である
            || m_cMapMgr.chara_exist2D[pz,px] != -1  //既にキャラが存在する
            || m_cMapMgr.onground_exist2D[pz,px] != -1) //既に床オブジェクトが存在する
        {
            px = Random.Range(0, m_iWidth);
            pz = Random.Range(0, m_iHeight);
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
