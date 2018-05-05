using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventryInfo : MonoBehaviour {

    GameObject parent;
    PlayerItem playerItem;
    PlayerItemUse playerItemUse;

    const int MAX_INVENTRY = 9;
    GameObject[] cells = new GameObject[MAX_INVENTRY];
    int[] haveItemIDs = new int[MAX_INVENTRY];
    [SerializeField]
    GameObject popUpDesctiption;
    //float timeCount = 0;

    // メモリに確保せず UpdateInventry のたびにGetComponen がいいかもしれない
    //Image[] icons = new Image[MAX_INVENTRY];
    //Text

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        playerItem = parent.GetComponentInChildren<PlayerItem>();
        playerItemUse = parent.GetComponentInChildren<PlayerItemUse>();

        OneCell[] _cells = GetComponentsInChildren<OneCell>();
        for(int i = 0; i < MAX_INVENTRY; ++i ) 
        {
            cells[i] = _cells[i].gameObject;
            _cells[i].GetComponentInChildren<EventItemPopUp>().cellNum = i;
        }
        popUpDesctiption.SetActive(false);
	}

    public void UpdateInventry()
    {
        var items = playerItem.items;
        int count = 0;

        foreach ( KeyValuePair<int, ItemData> item in items )
        {
            // ソウルストーン系はインベントリに表示しない
            if ( item.Key < 100 ) continue;

            if ( item.Value.kosuu > 0 )
            {
                Image icon = cells[count].GetComponentInChildren<ImageIcon>().GetComponent<Image>();
                icon.color = Color.white;
                icon.sprite = item.Value.itemImage;

                Text text = cells[count].GetComponentInChildren<Text>();
                text.text = "x" + item.Value.kosuu.ToString();
                haveItemIDs[count] = item.Key;
                
                ++count;
            }
        }
        for (int i = count; i < MAX_INVENTRY; ++i )
        {
            Image icon = cells[i].GetComponentInChildren<ImageIcon>().GetComponent<Image>();
            // 透明
            icon.color = Color.clear;
            Text text = cells[i].GetComponentInChildren<Text>();
            text.text = "";

            haveItemIDs[i] = -1;
        }
    }

    private void OnEnable()
    {
        if ( !parent ) Start();

        UpdateInventry();
    }

    public void PopUpItemDescription(bool on, int num = 0)
    {
        if ( !on || haveItemIDs[num] == -1 )
        {
            popUpDesctiption.SetActive(false);
        }
        else
        {
            popUpDesctiption.SetActive(true);
            Text description = popUpDesctiption.GetComponentInChildren<OneText>().GetComponent<Text>();
            var item = playerItem.items[haveItemIDs[num]];
            string str = item.name + "\n" + item.setumei;
            description.text = str;

            popUpDesctiption.GetComponentInChildren<ItemUseButton>().selectItemID = haveItemIDs[num];
        }
    }

    public void OffPopUpDescription()
    {
        popUpDesctiption.SetActive(false);
    }

    public void UseItem(int ID)
    {
        playerItemUse.UseItem(ID);
        UpdateInventry();
        PopUpItemDescription(false);
    }
}
