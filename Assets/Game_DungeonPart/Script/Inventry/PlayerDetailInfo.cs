using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetailInfo : MonoBehaviour {

    [SerializeField]
    Text statusText;

    GameObject parent;
    DungeonPartManager dMn;
    Player player;
    AtkAndDef pl_AAD;

    private void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        player = parent.GetComponentInChildren<Player>();
        pl_AAD = player.GetComponent<AtkAndDef>();
        statusText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        UpdateStatusText();
    }

    public void UpdateStatusText()
    {
        if ( !parent )
        {
            Start();
        }
        statusText.text =
                "英魂の洞窟" + dMn.floor + "F\n" +
                "Level : " + player.Level + "\n" +
                "EXP : " + player.Exp + "\n" +
                "HP : " + player.HP + " / " + player.MaxHP + "\n" +
                "MP : " + player.MP + " / " + player.MaxMP + "\n" +
                //"ｽﾀﾐﾅ : " + player.Stamina + " / 100" + "\n" +
                "ﾍﾞｰｽ魔法威力 : " + pl_AAD.BaseMagicPower.ToString("N2") + "\n" +
                "炎属性魔法威力 : " + (100 * pl_AAD.FlameMagicPower).ToString("N0") + "\n" +
                "雷属性魔法威力 : " + (100 * pl_AAD.LightMagicPower).ToString("N0") + "\n" +
                "氷属性魔法威力 : " + (100 * pl_AAD.IceMagicPower).ToString("N0");
    }
}
