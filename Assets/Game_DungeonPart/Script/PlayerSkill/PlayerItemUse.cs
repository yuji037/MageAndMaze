using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemUse : MonoBehaviour {

    GameObject parent;
    Player player;
    PlayerItem playerItem;
    PlayerAttack playerAttack;

    RevealedMap revealedMap;
    UISwitch uiSwitch;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        playerItem = player.GetComponent<PlayerItem>();
        playerAttack = player.GetComponent<PlayerAttack>();
        revealedMap = parent.GetComponentInChildren<RevealedMap>();
        uiSwitch = parent.GetComponentInChildren<UISwitch>();
	}

    public bool UseItem(int ID)
    {
        if ( playerItem.items[ID].kosuu <= 0 ) return false;

        // 100 : 攻撃力アップのオーブ
        // 101 : 防御力アップのオーブ
        // 102 : 倍速効果のオーブ
        // 103 : 即時回復のオーブ
        // 104 : 透明化のオーブ
        // 105 : 気配察知のオーブ
        // 106 : 単体攻撃のオーブ
        // 107 : 範囲攻撃のオーブ
        // 108 : 暗視のオーブ
        playerItem.items[ID].kosuu -= 1;
        switch ( ID )
        {
            case 100:
                player.m_cAbnoState.SetTurn(AbnoStateType.AtkUp, 30f);
                break;
            case 101:
                player.m_cAbnoState.SetTurn(AbnoStateType.DefUp, 30f);
				break;
            case 102:
                player.m_cAbnoState.SetTurn(AbnoStateType.SpdUp, 30f);
				break;
            case 103:
                player.HealByPercent(0.7f);
                break;
            case 104:
                player.m_cAbnoState.SetTurn(AbnoStateType.Transparent, 15f);
				break;
            case 105:
                revealedMap.canAllEnemySearch = true;

                break;
            case 106:
                playerAttack.MagicAttack(1001);
                break;
            case 107:
                playerAttack.MagicAttack(1002);
                break;
            case 108:
                revealedMap.revealAll = true;
                break;
        }
        player.UpdateAbnoEffect();
        uiSwitch.SwitchUI((int)DungUIType.BATTLE);
        return true;
    }
}
