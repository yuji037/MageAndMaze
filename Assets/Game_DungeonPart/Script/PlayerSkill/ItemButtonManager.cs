using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    private Dictionary<int, ItemData> items;
    [SerializeField]
    private GameObject setumeiPanel;
    [SerializeField]
    private GameObject eleLvUpCheckPanel;
    [SerializeField]
    private GameObject syouhiPanel;
    [SerializeField]
    private GameObject blockkingPanel;
    [SerializeField]
    private GameObject AttributeTraningPanel;
    private Player player;
    private const int maxSyouhiItem = 3;
    private GameObject[] syouhiItemsImg = new GameObject[maxSyouhiItem];
    private List<GameObject> createItemsButton = new List<GameObject>();
    private List<int> canCreateItemsId = new List<int>();
    private int traningStoneColor;
    //0~11のパネルの番号-1は未選択
    private int selectPanelNum = -1;
    int[] useItemSyuren=new int[3];
    bool inited = false;
    private void Start()
    {
        items = GameObject.Find("GameObjectParent").GetComponentInChildren<PlayerItem>().items;
        player = GameObject.Find("GameObjectParent").GetComponentInChildren<Player>();
        for (int i = 0; i < 12; i++)
        {
            createItemsButton.Add(transform.Find("CreateImage (" + i + ")").gameObject);
        }
        for (int i = 0; i < maxSyouhiItem; i++)
        {
            syouhiItemsImg[i] = syouhiPanel.transform.Find("SyouhiItem (" + i + ")").gameObject;
        }
        selectedPanel();
        setCreateItems();
        useItemSyuren[(int)PlayerItem.stone.RED_STONE] = 5;
        useItemSyuren[(int)PlayerItem.stone.YELLOW_STONE] = 5;
        useItemSyuren[(int)PlayerItem.stone.BLUE_STONE] = 5;
        syouhiItemsImg[0].GetComponent<Image>().sprite = items[0].itemImage;
        syouhiItemsImg[1].GetComponent<Image>().sprite = items[1].itemImage;
        syouhiItemsImg[2].GetComponent<Image>().sprite = items[2].itemImage;
        lvUpCheckReload();
        useItemReload();
        eleLvUpCheckPanel.SetActive(false);
        inited = true;
    }
    private void OnEnable()
    {
        if (inited)
        {
            selectedPanel();
            setCreateItems();
            useItemReload();
            lvUpCheckReload();
            eleLvUpCheckPanel.SetActive(false);
        }

    }
    //-1はセレクトしなかった場合の処理
    private void selectedPanel(int panelnum = -1)
    {
        selectPanelNum = panelnum;
        bool active = (panelnum != -1);
        setumeiPanel.SetActive(active);
        blockkingPanel.SetActive(active);
        if (active)
        {
            setumeiPanel.transform.Find("description").GetComponent<Text>().text = items[canCreateItemsId[panelnum]].setumei;
            setumeiPanel.transform.Find("name").GetComponent<Text>().text = "" + items[canCreateItemsId[panelnum]].name;
            setumeiPanel.transform.Find("haveNumbers").GetComponent<Text>().text = "所持数:" + items[canCreateItemsId[panelnum]].kosuu;

            setumeiPanel.transform.Find("itemImage").GetComponent<Image>().sprite = items[canCreateItemsId[panelnum]].itemImage;
        }
    }
    public void BlockkingOnClick()
    {
        selectedPanel();
        eleLvUpCheckPanel.SetActive(false);
    }

    public void ItemsOnClick(int panelnum)
    {
        eleLvUpCheckPanel.SetActive(false);
        selectedPanel(panelnum);
        useItemReload(items[canCreateItemsId[panelnum]].syouhiSozai);
        lvUpCheckReload();
    }
    public void CreateOnClick()
    {
        int createItemId = canCreateItemsId[selectPanelNum];
        ItemCreate();
        setCreateItems();
        //もし前選んだスキルが今回も作れるなら、引き続き同じアイテムの場所に作成パネルが呼ばれる。
        int panelnum = -1;
        for (int i = 0; i < canCreateItemsId.Count; i++)
        {
            if (createItemId == canCreateItemsId[i])
            {
                panelnum = i;
            }
        }
        selectedPanel(panelnum);
        useItemReload((panelnum == -1) ? null : items[canCreateItemsId[panelnum]].syouhiSozai);

    }
    public void eleLvUpOnClick(int stone_color)
    {
        switch (stone_color)
        {
            case 0:
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().text = "火属性修練";
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().color = Color.red;
                break;
            case 1:
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().text = "雷属性修練";
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().color = Color.yellow;
                break;
            case 2:
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().text = "水属性修練";
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().color = Color.blue;
                break;
        }
        Dictionary<int, int> useStones = new Dictionary<int, int>();
        traningStoneColor = stone_color;
        useStones[stone_color] = useItemSyuren[traningStoneColor];
        useItemReload(useStones);
        blockkingPanel.SetActive(true);
        setumeiPanel.SetActive(false);
        eleLvUpCheckPanel.SetActive(true);
    }
    public void yesOnClick()
    {

        if (items[traningStoneColor] != null)
        {
            items[traningStoneColor].kosuu -= useItemSyuren[traningStoneColor];
            if (items[traningStoneColor].kosuu < useItemSyuren[traningStoneColor]) eleLvUpCheckPanel.SetActive(false);
        }
        player.ElementLevelUp(traningStoneColor + 1);
        Dictionary<int, int> useStones = new Dictionary<int, int>();
        useStones[(int)traningStoneColor] = useItemSyuren[traningStoneColor];
        useItemReload((items[traningStoneColor].kosuu >= useItemSyuren[traningStoneColor])?useStones:null);
        setCreateItems();
        lvUpCheckReload();
    }
    public void noOnClick()
    {
        useItemReload();
        eleLvUpCheckPanel.SetActive(false);
    }
    //アイテム個数を増やす
    private void ItemCreate()
    {
        foreach (var i in items[canCreateItemsId[selectPanelNum]].syouhiSozai)
        {
            items[i.Key].kosuu -= i.Value;
        }
        items[canCreateItemsId[selectPanelNum]].kosuu++;
        lvUpCheckReload();
    }

    private void lvUpCheckReload()
    {
        foreach (int s in Enum.GetValues(typeof(PlayerItem.stone)))
        {
            switch (s)
            {
                case 0:
                    AttributeTraningPanel.transform.Find("" + s).transform.Find("Rate").GetComponent<Text>().text = string.Format("{0:0.00}", player.atkAndDef.FlameMagicPower) + "倍";
                    break;
                case 1:
                    AttributeTraningPanel.transform.Find("" + s).transform.Find("Rate").GetComponent<Text>().text = string.Format("{0:0.00}", player.atkAndDef.LightMagicPower) + "倍";
                    break;
                case 2:
                    AttributeTraningPanel.transform.Find("" + s).transform.Find("Rate").GetComponent<Text>().text = string.Format("{0:0.00}",player.atkAndDef.IceMagicPower)  + "倍";
                    break;
            }

            if (useItemSyuren[s] <= items[s].kosuu)
            {
                AttributeTraningPanel.transform.Find("" + s).GetComponent<Button>().interactable = true;
              
            }
            else
            {
                AttributeTraningPanel.transform.Find("" + s).GetComponent<Button>().interactable = false;

            }

        }
    }
    //消費するアイテム個数を更新
    private void useItemReload(Dictionary<int, int> use_items = null)
    {

        int n = 0;
        //一旦リセット処理
        foreach (var i in syouhiItemsImg)
        {
            syouhiItemsImg[n].transform.Find("temoti").GetComponent<Text>().text = "x" + items[n].kosuu;
            syouhiItemsImg[n].transform.Find("syouhi").GetComponent<Text>().text = "";
            n++;
        }
        if (use_items != null)
        {
            n = 0;
            foreach (var i in use_items)
            {
                syouhiItemsImg[i.Key].transform.Find("temoti").GetComponent<Text>().text = "x" + items[i.Key].kosuu;
                syouhiItemsImg[i.Key].transform.Find("syouhi").GetComponent<Text>().text = (i.Value == 0) ? "" : "-" + i.Value;
                n++;
            }
        }
    }
    //作成できるアイテムを更新
    void setCreateItems()
    {
        canCreateItemsId.Clear();
        foreach (var i in items)
        {
            if (canCreate(i.Key))
            {
                canCreateItemsId.Add(i.Key);
            }
        }
        for (int n = 0; n < 12; n++)
        {
            if (n < canCreateItemsId.Count)
            {
                createItemsButton[n].transform.Find("ItemImage").GetComponent<Image>().enabled = true;
                createItemsButton[n].GetComponentInChildren<Button>().interactable = true;
                createItemsButton[n].transform.Find("ItemImage").GetComponent<Image>().sprite = items[canCreateItemsId[n]].itemImage;
            }
            else
            {
                createItemsButton[n].GetComponentInChildren<Button>().interactable = false;
                createItemsButton[n].transform.Find("ItemImage").GetComponent<Image>().enabled = false;
            }

        }
    }

    bool canCreate(int itemId)
    {
        if (items[itemId].syouhiSozai == null)
        {
            return false;
        }
        if (items[itemId].syouhiSozai.Count == 0)
        {
            return false;
        }
        foreach (var i in items[itemId].syouhiSozai)
        {
            if (!(items[i.Key].kosuu >= i.Value))
            {
                return false;
            }
        }
        return true;
    }
}
