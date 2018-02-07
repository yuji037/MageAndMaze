using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public ItemData(string Name,string Setumei, Dictionary<int, int> syouhi_Sozai)
    {
        name = Name;
        setumei = Setumei;
        kosuu = 0;
        syouhiSozai = syouhi_Sozai;
    }
    public void setImage(string imagePas)
    {
        itemImage = Resources.Load<Sprite>(imagePas) as Sprite;

    }
    public string name;
    public string setumei;
    public int kosuu;
    public Sprite itemImage;
    public Dictionary<int,int> syouhiSozai;

}
