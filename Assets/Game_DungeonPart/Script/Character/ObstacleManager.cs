using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour {

    [SerializeField]
    GameObject[] obstaclePrefabs;

    public List<Obstacle> obstacles = new List<Obstacle>();
    [SerializeField] int obstacleCount = 5;
    int obsCount = 0;

    GameObject parent;
    MapManager mapMn;
    public DungeonInitializer d_init;

    [SerializeField]
    GameObject charactersParent;


    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();

        // 中断後の再開であれば 1、新規作成であれば 0
        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            LoadObstacleData();
        }
        else
        {
            while ( obsCount < obstacleCount )
            {
                int obsType = Random.Range(0, 3);
                Vector3 pos = d_init.GetRandomPos();
                AddObstacle((Obstacle.Type)obsType, pos);
            }
        }
    }

    public Obstacle AddObstacle(Obstacle.Type type, Vector3 pos, bool placeAnyWay = false)
    {
        if ( !mapMn.CanPutCharacter(pos) && !placeAnyWay ) return null;

        var obj = Instantiate(obstaclePrefabs[(int)type], charactersParent.transform);
        obj.transform.position = pos;
        Obstacle obstacle = obj.GetComponent<Obstacle>();
        obstacle.type = type;
        obstacle.Init();

        // obstacle には 201～の IDをつける
        int _ID = obsCount + 201;
        obstacle.idNum = _ID;
        mapMn.chara_exist2D[(int)pos.z, (int)pos.x] = _ID;

        obstacles.Add(obj.GetComponent<Obstacle>());
        obsCount++;
        return obstacle;
    }

    public void RemoveObstacle(int ID)
    {
        for(int i = obstacles.Count - 1; i >= 0; i-- )
        {
            if ( obstacles[i].idNum == ID )
            {
                obstacles.RemoveAt(i);
                return;
            }
        }
    }

    public Obstacle GetObstacle(int ID)
    {
        foreach(Obstacle obs in obstacles )
        {
            if(obs.idNum == ID )
            {
                return obs;
            }
        }
        return null;
    }

    public void DestroyCheck()
    {
        for ( int i = obstacles.Count - 1; i >= 0; i-- )
        {
            if ( !obstacles[i].IsAlive )
            {
                var obs = obstacles[i];
                RemoveObstacle(obstacles[i].idNum);
                Destroy(obs.gameObject);
            }
        }
    }

    [System.Serializable]
    class SavableObsData
    {
        public Obstacle.Type type;
        public int HP;
        public Vector3 pos;

        public SavableObsData()
        {
            type = Obstacle.Type.ROCK;
            HP = 10;
            pos = Vector3.zero;
        }
        public SavableObsData(Obstacle obs)
        {
            type = obs.type;
            HP = obs.HP;
            pos = obs.pos;
        }
        public void Load(Obstacle obs)
        {
            obs.HP = HP;
        }

    }

    public void SaveObstacleData()
    {
        List<SavableObsData> datas = new List<SavableObsData>();
        foreach(Obstacle obs in obstacles )
        {
            SavableObsData _data = new SavableObsData(obs);
            datas.Add(_data);
        }
        SaveData.SetList("ObstacleData", datas);
    }

    public void LoadObstacleData()
    {
        List<SavableObsData> datas = SaveData.GetList("ObstacleData", new List<SavableObsData>());
        foreach(SavableObsData _data in datas )
        {
            // セーブデータロードなので何が何でも置く
            var obs = AddObstacle(_data.type, _data.pos, true);
            if (obs) _data.Load(obs);
        }
    }
}
