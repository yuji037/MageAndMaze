using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour {

    GameObject parent;
    Player player;
    PlayerItem playerItem;
    PlayCharaSwitch playCharaSwitch;

    private void Awake()
    {
        parent = GameObject.Find("GameObjectParent");
        playerItem = parent.GetComponentInChildren<PlayerItem>();
        playCharaSwitch = parent.GetComponentInChildren<PlayCharaSwitch>();
    }

    // Use this for initialization
    void Start () {
	}
	
    public void AcquireSoulStone(int itemID, float plus)
    {
        if ( !playCharaSwitch ) Awake();

        if ( itemID < 0 || 2 < itemID ) return;
        float rate = 0;
        float reaperBonus = playCharaSwitch.isAngel ? 0.7f : 1.3f;
        rate = Random.Range(0.8f, 1.2f);
        playerItem.GetItem(itemID, Mathf.RoundToInt(plus * rate * reaperBonus));
    }
}
