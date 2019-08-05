using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public const int DUNGEON_WIDTH = 30;
    public const int DUNGEON_HEIGHT = 30;

    public int[,] dung_2D { get; private set; } //変更後の壁(-1)と床(0~)の情報
    GameObject[,] dark_2D;
    public int[,] dung_room_info2D { get; private set; } //元の部屋と道のみ(部屋の情報)
    public RoomInfo[] room_info { get; private set; }
    public int[,] chara_exist2D { get; private set; } //IDが入っている　誰もいない場合は-1
    public int[,] onground_exist2D { get; private set; }

    GameObject parent;
    Player player;
    int dungeonType = 0;
    int floor = 0;
    [SerializeField] GameObject[] MapChipPrefabs;
    [SerializeField] int mapChipTypeMax = 6;
    GameObject mapChipParent;
    public GameObject[,] mapChips;

    [SerializeField]
    private GameObject[] mapGenerator;
    
    public DungeonInitializer d_initializer;

    StageWallManager bigWallMn;
    // Update is called once per frame
    void Update()
    {
//#if UNITY_EDITOR
//        if (Input.GetKeyDown("return"))
//        {
//            UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon1");
//        }
//#endif
    }

    public int GetDungeonWidth() { return DUNGEON_WIDTH; }
    public int GetDungeonHeight() { return DUNGEON_HEIGHT; }
    public int GetDungeonInfo(int x, int z) {
        if ( !InsideMap(new Vector3(x, 0, z)) ) return -1;
        return dung_2D[z, x];
    }
    public int GetCharaExist(int x, int z) {
        if ( !InsideMap(new Vector3(x, 0, z)) ) return -1;
        return chara_exist2D[z, x];
    }
    public int GetOnGroundExist(int x, int z)
    {
        if ( !InsideMap(new Vector3(x, 0, z)) ) return -1;
        return onground_exist2D[z, x];
    }

    public int max_room { get; set; }

    public void DungeonGenerate()
    {
        parent = GameObject.Find("GameObjectParent");
        dung_2D = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH];
        dark_2D = new GameObject[DUNGEON_HEIGHT, DUNGEON_WIDTH];
        dung_room_info2D = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH];
        chara_exist2D = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH];
        onground_exist2D = new int[DUNGEON_HEIGHT, DUNGEON_WIDTH];
        for ( int z = 0; z < DUNGEON_HEIGHT; z++ )
        {
            for ( int x = 0; x < DUNGEON_WIDTH; x++ )
            {
                chara_exist2D[z, x] = -1;
                onground_exist2D[z, x] = -1;
            }
        }
        mapChips = new GameObject[DUNGEON_HEIGHT, DUNGEON_WIDTH];

        floor = parent.GetComponentInChildren<DungeonPartManager>().floor;

        //SaveData.SetInt("IsInterrupt", 0);
        
        // 中断後の再開であれば 1、新規作成であれば 0
        if ( 1 == SaveData.GetInt("IsInterrupt", 0))
        {
            Load();
        }
        else
        {

            GameObject mapGene;

            // 新規のダンジョン地形生成
            
            if ( floor == 30 )
            {
                // ボス1マップ
                mapGene = Instantiate(mapGenerator[1]);
                d_initializer.AllowCharaOnRoad = true;
            }
            else if ( floor % 8 == 0 )
            {
                // 大部屋マップ
                mapGene = Instantiate(mapGenerator[2]);
            }
            else if ( floor == 1 && SaveData.GetInt("IsTutorialON", 1 ) == 1)
            {
                // 固定マップ（現状チュートリアルのみ）
                mapGene = Instantiate(mapGenerator[3]);
                d_initializer.PopRandomEnemys = false;
                d_initializer.AllowCharaOnRoad = true;
                d_initializer.PopRandomGroundObjects = false;
                d_initializer.PopRandomObstacles = false;
            }
            else
            {
                // 通常ランダムマップ
                mapGene = Instantiate(mapGenerator[0]);
            }
            var generator = mapGene.GetComponent<MapGenerator>();
            generator.dungeon_height = DUNGEON_HEIGHT;
            generator.dungeon_width = DUNGEON_WIDTH;
            dung_2D = generator.DungeonInfoMake();
            dung_room_info2D = dung_2D;
            room_info = generator.GetRoomInfo();
            max_room = generator.GetMaxRoom();
            Destroy(mapGene);
        }

        mapChipParent = GameObject.Find("MapChips");
        dungeonType = parent.GetComponentInChildren<DungeonPartManager>().dungeonType;

        d_initializer.DungeonDataInit();
        
        // 階段の位置決め
        Vector3 _stairPos = d_initializer.StairsPosDecide();
        if (floor != 30) onground_exist2D[(int)_stairPos.z, (int)_stairPos.x] = 100;

        // マップチップの生成
        MapChipPrefabs = new GameObject[mapChipTypeMax];
        for (int i = 0; i < mapChipTypeMax; ++i )
        {
            var prefab = Resources.Load<GameObject>("MapChipPrefab/" + dungeonType + "/" + i) as GameObject;
            MapChipPrefabs[i] = prefab;
        }
        for (int z = 0; z < DUNGEON_HEIGHT; z++)
        {
            for (int x = 0; x < DUNGEON_WIDTH; x++)
            {
                int mapID = dung_2D[z, x];
                int chipType = 0;
                if (mapID <= -1) chipType = 0;                      // 壁(-2は破壊不可能な壁）
                else if (mapID <= max_room - 1) chipType = 1;       // 部屋
                else chipType = 2;                                  // 道
                if (onground_exist2D[z, x] == 100) chipType = 5;    // 階段
                var mapChip = Instantiate(MapChipPrefabs[chipType], mapChipParent.transform);
                mapChip.transform.position = new Vector3(x, 0, z);
                mapChips[z, x] = mapChip;

                Darken _darken = mapChip.GetComponentInChildren<Darken>();
                if (_darken) { dark_2D[z, x] = _darken.gameObject; }
            }
        }

        bigWallMn = parent.GetComponentInChildren<StageWallManager>();
        bigWallMn.Init();
    }

    public void SetCharaAndObjectInfo(int[,] chara_ex2D, int[,] onground_ex2D)
    {
        chara_exist2D = chara_ex2D;
        onground_exist2D = onground_ex2D;
    }
    // 場外判定
    public bool InsideMap(Vector3 pos)
    {
        if ( 0 < pos.z && pos.z < DUNGEON_HEIGHT - 1 && 0 < pos.x && pos.x < DUNGEON_WIDTH - 1 )
            return true;
        return false;
    }

    public bool IsWall(Vector3 pos)
    {
        return dung_2D[(int)pos.z, (int)pos.x] < 0;
    }

    public bool CanMoveCheck(Vector3 nPos, Vector3 sPos)
    {
        if ( !InsideMap(sPos) ) return false;

        if ( dung_2D[(int)sPos.z, (int)sPos.x] < 0 ) return false;
        else
        {
            // ナナメの判定
            if ( !DiagonalCheck(nPos, sPos))
            {
                return false;
            }
        }
        if ( chara_exist2D[(int)sPos.z, (int)sPos.x] != -1 ) return false;
        return true;
    }

    public bool CanPutCharacter(Vector3 pos)
    {
        if ( !InsideMap(pos) ) return false;

        if ( dung_2D[(int)pos.z, (int)pos.x] < 0 ) return false;

        // 200 は Obstacle の予約番号
        if ( chara_exist2D[(int)pos.z, (int)pos.x] != -1
            && chara_exist2D[(int)pos.z, (int)pos.x] != 200 ) return false;

        if ( onground_exist2D[(int)pos.z, (int)pos.x] != -1 ) return false;

        return true;
    }

    public bool CanPutOnGround(Vector3 pos)
    {
        if ( !InsideMap(pos) ) return false;

        // 壁の場合はダメ
        if ( dung_2D[(int)pos.z, (int)pos.x] < 0 ) return false;

        // 障害物はダメだが、プレイヤー・敵は居てもよい
        if ( chara_exist2D[(int)pos.z, (int)pos.x] != -1
            && chara_exist2D[(int)pos.z, (int)pos.x] >= 200
            && chara_exist2D[(int)pos.z, (int)pos.x] < 400 ) return false;
        
        if ( onground_exist2D[(int)pos.z, (int)pos.x] != -1 ) return false;

        return true;
    }

    public bool IsBreakableObstacle(Vector3 pos)
    {
        if ( !InsideMap(pos) ) return false;

        return ( 200 <= chara_exist2D[(int)pos.z, (int)pos.x] && chara_exist2D[(int)pos.z, (int)pos.x] < 400 );
    }

    // ナナメの判定（たすき掛けの位置2か所がどちらか壁なら false）
    public bool DiagonalCheck(Vector3 nPos, Vector3 sPos)
    {
        if ( dung_2D[(int)nPos.z, (int)sPos.x] < 0
                || dung_2D[(int)sPos.z, (int)nPos.x] < 0 )
        {
            return false;
        }
        return true;
    }
    public void SetCharaExistInfo(Vector3 pos, int charaNum = -1, bool _isExist = false)
    {
        if (!_isExist) chara_exist2D[(int)pos.z, (int)pos.x] = -1;
        else chara_exist2D[(int)pos.z, (int)pos.x] = charaNum;
    }

    public enum ChipType
    {
        NONE = -1,
        WALL,
        ROOM_SPACE,
        ROAD
    }
    public void ChangeMapChip(int z, int x, int before, int after)      // 壁を壊すなどマップ構造を変化させた時の判定
    {
        // 場外判定
        if ( !InsideMap(new Vector3(x, 0, z)))return;

        if (dung_2D[z,x] == before)
        {
            int afterChipType = 0;
            if (after == -1) afterChipType = 0;
            else if (after <= max_room - 1) afterChipType = 1;
            else afterChipType = 2;
            dung_2D[z, x] = after;
            Destroy(mapChips[z, x]);
            var newMapChip = Instantiate(MapChipPrefabs[afterChipType]);
            newMapChip.transform.position = new Vector3(x, 0, z);
            mapChips[z, x] = newMapChip;
        }
    }

    public void ChangeDungRoomInfo(int z, int x, int before, int after)
    {

    }

    public void DarkenSet()
    {
        if (!dark_2D[0, 0]) return;
        if (!player) player = parent.GetComponentInChildren<Player>();
        Vector3 _plPos = player.sPos;
        int _existRoomNum = player.ExistRoomNum;
        for (var z = 0; z < DUNGEON_HEIGHT; z++)
        {
            for (var x = 0; x < DUNGEON_WIDTH; x++)
            {
                if (z >= _plPos.z - 1 && z <= _plPos.z + 1 && x >= _plPos.x - 1 && x <= _plPos.x + 1)
                    dark_2D[z, x].SetActive(false);

                else if (dung_2D[z,x] == _existRoomNum)
                    dark_2D[z, x].SetActive(false);

                else dark_2D[z, x].SetActive(true);
            }
        }
    }

    [System.Serializable]
    public class SavableMapData
    {
        public int[] _dung_2D = new int[DUNGEON_HEIGHT * DUNGEON_WIDTH];
        public int[] _dung_room_info2D = new int[DUNGEON_HEIGHT * DUNGEON_WIDTH];
        public int[] _chara_exist2D = new int[DUNGEON_HEIGHT * DUNGEON_WIDTH];
        public int[] _onground_exist2D = new int[DUNGEON_HEIGHT * DUNGEON_WIDTH];
        public RoomInfo[] _room_info;
        public int _max_room;

        public SavableMapData()
        {

        }
        public SavableMapData(MapManager mapMn)
        {
            int i = 0;
            for ( var z = 0; z < DUNGEON_HEIGHT; z++ )
            {
                for ( var x = 0; x < DUNGEON_WIDTH; x++ )
                {
                    _dung_2D[i] = mapMn.dung_2D[z, x];
                    _dung_room_info2D[i] = mapMn.dung_room_info2D[z, x];
                    _chara_exist2D[i] = mapMn.chara_exist2D[z, x];
                    _onground_exist2D[i] = mapMn.onground_exist2D[z, x];
                    i++;
                }
            }
            _room_info = mapMn.room_info;
            _max_room = mapMn.max_room;
        }
        public void Load(MapManager mapMn)
        {
            int i = 0;
            for ( var z = 0; z < DUNGEON_HEIGHT; z++ )
            {
                for ( var x = 0; x < DUNGEON_WIDTH; x++ )
                {
                    mapMn.dung_2D[z, x] = _dung_2D[i];
                    mapMn.dung_room_info2D[z, x] = _dung_room_info2D[i];
                    mapMn.chara_exist2D[z, x] = _chara_exist2D[i];
                    mapMn.onground_exist2D[z, x] = _onground_exist2D[i];
                    i++;
                }
            }
            mapMn.room_info = _room_info;
            mapMn.max_room = _max_room;
        }
    }

    /// <summary>
    /// マップセーブ
    /// </summary>
    /// <param name="isLoadable"></param>
    public void Save(int isLoadable)
    {
        SavableMapData smd = new SavableMapData(this);
        SaveData.SetClass<SavableMapData>("MapData", smd);
        SaveData.SetInt("IsInterrupt", isLoadable);
    }

    /// <summary>
    /// マップロード
    /// </summary>
    public void Load()
    {
        SavableMapData smd = SaveData.GetClass<SavableMapData>("MapData", new SavableMapData());
        smd.Load(this);
    }
}
