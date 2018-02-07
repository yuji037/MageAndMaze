using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealedMap : MonoBehaviour {

    public bool[,] reveal2D { get; private set; }
    [SerializeField]
    int revealRange = 2; // 道に居る時、周囲 2 マスのマッピングをする

    GameObject parent;
    MapManager mapMn;
    Player player;

    [SerializeField]
    public bool revealAll = false;

    [SerializeField]
    bool debugMode = false;

    [SerializeField]
    public bool canAllEnemySearch = false;

    // Use this for initialization
    void Start () {
        //parent = GameObject.Find("GameObjectParent");
        //mapMn = parent.GetComponentInChildren<MapManager>();
        //reveal2D = new bool[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];
        //player = parent.GetComponentInChildren<Player>();
	}

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        player = parent.GetComponentInChildren<Player>();

        reveal2D = new bool[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];
        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            // 中断データのロード
            Load();
        }

        DungeonPartManager dMn = parent.GetComponentInChildren<DungeonPartManager>();
        revealAll = ( dMn.floor == 30 );
    }

    public void UpdateRevealMap()
    {
        if ( debugMode || revealAll )
        {
            for ( int z = 0; z < MapManager.DUNGEON_HEIGHT; ++z )
            {
                for ( int x = 0; x < MapManager.DUNGEON_WIDTH; ++x )
                {
                    reveal2D[z, x] = true;
                }
            }
        }

        int _existRoomNum = player.existRoomNum;
        if (_existRoomNum < mapMn.max_room )
        {
            // プレイヤーが部屋に居る
            for (int z = 0; z < MapManager.DUNGEON_HEIGHT; ++z )
            {
                for (int x = 0; x < MapManager.DUNGEON_WIDTH; ++x )
                {
                    if ( _existRoomNum == mapMn.dung_room_info2D[z, x] )
                    {
                        reveal2D[z, x] = true;
                        continue;
                    }
                    if ((int)player.sPos.z - revealRange <= z && z <= (int)player.sPos.z + revealRange
                        && (int)player.sPos.x - revealRange <= x && x <= (int)player.sPos.x + revealRange )
                        reveal2D[z, x] = true;
                }
            }
        }
        else
        {
            // プレイヤーが部屋以外（道など）に居る
            for (int z = (int)player.sPos.z - revealRange; z <= (int)player.sPos.z + revealRange; ++z )
            {
                for ( int x = (int)player.sPos.x - revealRange; x <= (int)player.sPos.x + revealRange; ++x )
                {
                    if ( z < 0 || z >= MapManager.DUNGEON_HEIGHT
                        || x < 0 || x >= MapManager.DUNGEON_WIDTH ) continue;

                    reveal2D[z, x] = true;
                }
            }
        }
    }

    public bool IsSightRange(Vector3 pos)
    {
        if ( debugMode || revealAll || canAllEnemySearch ) return true;

        int z = (int)pos.z;
        int x = (int)pos.x;
        if ( z < 0 || z >= MapManager.DUNGEON_HEIGHT
            || x < 0 || x >= MapManager.DUNGEON_WIDTH ) return false;

        int _existRoomNum = player.existRoomNum;
        if ( _existRoomNum < mapMn.max_room )
        {
            // プレイヤーが部屋に居る
            if ( mapMn.dung_room_info2D[z, x] == _existRoomNum )
            {
                return true;
            }
        }
        else
        {
            // プレイヤーが部屋以外（道など）に居る
            if ( z >= player.sPos.z - revealRange && z <= player.sPos.z + revealRange 
                && x >= player.sPos.x - revealRange && x <= player.sPos.x + revealRange )
            {
                return true;
            }
        }
        return false;
    }

    public void SwitchDebugMode()
    {
        debugMode = !debugMode;
    }

    [System.Serializable]
    class SaveRevealedMap
    {
        public bool[] reveal;

        public SaveRevealedMap()
        {
            reveal = new bool[MapManager.DUNGEON_HEIGHT * MapManager.DUNGEON_WIDTH];
        }

        public SaveRevealedMap(bool[,] _reveal2D)
        {
            reveal = new bool[MapManager.DUNGEON_HEIGHT * MapManager.DUNGEON_WIDTH];
            int i = 0;
            for (var z = 0; z < MapManager.DUNGEON_HEIGHT; z++ )
            {
                for ( var x = 0; x < MapManager.DUNGEON_WIDTH; x++ )
                {
                    reveal[i] = _reveal2D[z, x];
                    i++;
                }
            }
        }

        public void Load(RevealedMap rm)
        {
            int i = 0;
            for ( var z = 0; z < MapManager.DUNGEON_HEIGHT; z++ )
            {
                for ( var x = 0; x < MapManager.DUNGEON_WIDTH; x++ )
                {
                    rm.reveal2D[z, x] = reveal[i];
                    i++;
                }
            }
        }
        
        public void Show()
        {
            string str = "";
            for ( var z = MapManager.DUNGEON_HEIGHT - 1; z >= 0; z-- )
            {
                for ( var x = 0; x < MapManager.DUNGEON_WIDTH; x++ )
                {
                    str += reveal[z * MapManager.DUNGEON_WIDTH + x] ? 1 : 0;
                }
                str += "\n";
            }
            Debug.Log(str);
        }
    }

    public void Save()
    {
        SaveRevealedMap data = new SaveRevealedMap(this.reveal2D);
        SaveData.SetClass("RevealedMap", data);
        //data.Show();
    }

    public void Load()
    {
        SaveRevealedMap data = SaveData.GetClass("RevealedMap", new SaveRevealedMap());
        data.Load(this);
        //data.Show();
    }
}
