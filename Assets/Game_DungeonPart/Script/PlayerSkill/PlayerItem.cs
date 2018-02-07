using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class PlayerItem : MonoBehaviour
{
    public Dictionary<int, ItemData> items = new Dictionary<int, ItemData>();
    public enum stone{ RED_STONE,YELLOW_STONE,BLUE_STONE};
    // Use this for initialization
    void Awake()
    {
        TextAsset playerItemData = Resources.Load<TextAsset>("ItemData/item_data") as TextAsset;

        StringReader reader = new StringReader(playerItemData.text);
        reader.ReadLine();
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            char[] delimiterChars = { ',' };
            string[] words = line.Split(delimiterChars);

            // スキルデータの格納
            int num = 0;
            Dictionary<int, int> syouhiSozai=new Dictionary<int, int>();
            if (int.TryParse(words[3], out num))syouhiSozai[num]=int.Parse(words[4]);
            if (int.TryParse(words[5], out num)) syouhiSozai[num] = int.Parse(words[6]);
            if (int.TryParse(words[7], out num)) syouhiSozai[num] = int.Parse(words[8]);
            items[int.Parse(words[0])] = new ItemData(words[1], words[2], syouhiSozai);
        }
        
        //画像入れる
        foreach (var i in items)
        {
            i.Value.setImage("Image/ItemUI/" + i.Key);
        }

        LoadItemData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Serializable]
    public class SavableItem
    {
        public int ID;
        public int Count;

        public SavableItem() { }
        public SavableItem(int id, int count)
        {
            ID = id;
            Count = count;
        }
    }

    void LoadItemData()
    {
        // 所持アイテムのロード
        List<SavableItem> data = SaveData.GetList<SavableItem>("Items", new List<SavableItem>());
        foreach ( SavableItem d in data )
        {
            items[d.ID].kosuu = d.Count;
        }
    }

    public void SaveItemData()
    {
        // 所持アイテムのセーブ
        List<SavableItem> data = new List<SavableItem>();
        Dictionary<int, ItemData>.KeyCollection keyColl = items.Keys;
        foreach ( int key in keyColl )
        {
            data.Add(new SavableItem(key, items[key].kosuu));
        }
        SaveData.SetList<SavableItem>("Items", data);
        
        // 変更点のセーブ（必須）
        SaveData.Save();
    }
    public bool stoneEnoughCheck(int _red, int _yellow, int _blue)
    {
        if (items[(int)stone.RED_STONE].kosuu < _red) return false;
        if (items[(int)stone.YELLOW_STONE].kosuu < _yellow) return false;
        if (items[(int)stone.BLUE_STONE].kosuu < _blue) return false;
        return true;
    }
}
