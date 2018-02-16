using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class PlayerItem : MonoBehaviour
{
    public Dictionary<int, ItemData> items = new Dictionary<int, ItemData>();
    public enum stone{ RED_STONE,YELLOW_STONE,BLUE_STONE};
    [SerializeField] GameObject getTextParent;
    [SerializeField] GameObject getText;

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

    public void GetItem(int ID, int count)
    {
        ItemData data;
        if ( !items.TryGetValue(ID, out data) ) return;

        data.kosuu += count;
        var obj = Instantiate(getText, getTextParent.transform);
        var _text = obj.GetComponentInChildren<Text>();
        _text.text = ((count >= 0) ? "+" : "-") + data.name + "×" + count;
        StartCoroutine(FadeOutCoroutine(_text));
    }

    IEnumerator FadeOutCoroutine(Text text)
    {
        Color color = text.color;
        yield return new WaitForSeconds(1);

        for (float t = 0; t < 1; t+= Time.deltaTime )
        {
            text.color = new Color(color.r, color.g, color.b, ( 1 - t ));
            text.transform.localPosition = new Vector3(0, t * 50, 0);
            yield return null;
        }
        Destroy(text.transform.parent.gameObject);
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
