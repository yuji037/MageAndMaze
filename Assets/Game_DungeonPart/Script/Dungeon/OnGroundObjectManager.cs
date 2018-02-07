using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundObjectManager : MonoBehaviour {

    [SerializeField]
    GameObject[] objectPrefabs;

    List<OnGroundObject> objectList = new List<OnGroundObject>();
    [SerializeField] int groundObjCount = 5;
    int gObjCount = 0;

    GameObject parent;
    MapManager mapMn;
    public DungeonInitializer d_init;

    [SerializeField]
    GameObject mapChipsParent;

    // 300 : 水たまり
    // 301 : 回復パネル
    // 100 : 階段

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();

        // 中断後の再開であれば 1、新規作成であれば 0
        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            LoadOnGroundObjectData();
        }
        else
        {
            while ( gObjCount < groundObjCount )
            {
                int gObjType = Random.Range(0, 2);
                Vector3 pos = d_init.GetRandomPos();

                AddOnGroundObject((OnGroundObject.Type)gObjType, pos);
            }
        }
    }

    public OnGroundObject GetOnGroundObject(int ID)
    {
        foreach ( OnGroundObject obj in objectList )
        {
            if ( obj.ID == ID )
            {
                return obj;
            }
        }
        return null;
    }

    public OnGroundObject AddOnGroundObject(OnGroundObject.Type type, Vector3 pos, bool placeAnyWay = false)
    {
        if ( !mapMn.CanPutOnGround(pos) && !placeAnyWay ) return null;

        var obj = Instantiate(objectPrefabs[(int)type], mapChipsParent.transform);
        obj.transform.position = pos;
        OnGroundObject ogo = obj.GetComponent<OnGroundObject>();
        ogo.type = type;
        ogo.pos = pos;

        // mapobject には 300～の IDをつける
        int _ID = gObjCount + 300;
        mapMn.onground_exist2D[(int)pos.z, (int)pos.x] = _ID;
        ogo.ID = _ID;
        objectList.Add(ogo);
        gObjCount++;
        return ogo;
    }

    [System.Serializable]
    class SavableOGOData
    {
        public OnGroundObject.Type type;
        public Vector3 pos;

        public SavableOGOData()
        {
            type = OnGroundObject.Type.WATER;
            pos = Vector3.zero;
        }
        public SavableOGOData(OnGroundObject obj)
        {
            type = obj.type;
            pos = obj.pos;
        }
        public void Load(OnGroundObject obs)
        {

        }
    }

    public void SaveOnGroundObjectData()
    {
        List<SavableOGOData> datas = new List<SavableOGOData>();
        foreach ( OnGroundObject obj in objectList )
        {
            SavableOGOData _data = new SavableOGOData(obj);
            datas.Add(_data);
        }
        SaveData.SetList("OnGroundObjectData", datas);
    }

    public void LoadOnGroundObjectData()
    {
        List<SavableOGOData> datas = SaveData.GetList("OnGroundObjectData", new List<SavableOGOData>());
        foreach ( SavableOGOData _data in datas )
        {
            // セーブデータロードなので何が何でも置く
            var obj = AddOnGroundObject(_data.type, _data.pos, true);
            _data.Load(obj);
            Debug.Log(obj);
        }
    }
}
