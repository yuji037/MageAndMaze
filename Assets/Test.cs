using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Test : MonoBehaviour
{

    [SerializeField]
    public class PlayerT
    {
        [SerializeField]
        public int hp;
        public float atk;
        //public string name;
        //public List<string> items;
        //public Vector3 vec;
        public PlayerT()
        {

            //items = new List<string>();
            //items.Add("ポン");
            //items.Add("エーテル");
            //items.Add("エリクサー");
            hp = 10;
            atk = 100f;
            //name = "クラウド";
            //vec = new Vector3(20, 10, 0);
        }

    }
    // Use this for initialization
    void Start()
    {
        //セーブデータの設定
        //SaveData.SetInt("i", 10);
        //SaveData.SetClass<PlayerT>("p1", new PlayerT());
        //SaveData.Save();

        PlayerT getPlayer = SaveData.GetClass<PlayerT>("p1", new PlayerT());

        //Debug.Log("取得したint値は" + SaveData.GetInt("i"));
        //Debug.Log(getPlayer.name);
        //Debug.Log(getPlayer.items.Count + "こアイテムを持ってます");
        //Debug.Log(getPlayer.items[0] + getPlayer.items[1] + getPlayer.items[2]);
        //Debug.Log(getPlayer.vec);

        //SaveData.Remove("TestList");

        //List<PlayerT> list = new List<PlayerT>();
        //list.Add(new PlayerT());
        //list.Add(new PlayerT());
        //list.Add(new PlayerT());

        //SaveData.SetList<PlayerT>("TestList", list);

        //SaveData.Save();

        //List<PlayerT> load = new List<PlayerT>();
        //load = SaveData.GetList<PlayerT>("TestList", new List<PlayerT>());
        //Debug.Log(load[0].hp);
        //Debug.Log(load[1].hp);
        //Debug.Log(load[2].hp);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
