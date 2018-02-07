using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseButton : MonoBehaviour {

    public int selectItemID = 0;

    GameObject parent;
    InventryInfo inventryInfo;

    private void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        inventryInfo = parent.GetComponentInChildren<InventryInfo>();
    }

    public void UseItem()
    {
        inventryInfo.UseItem(selectItemID);
    }
}
