using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SubUIType
{
    CHECK_NEXT_FLOOR,
    INTERACT,
    SubUITypeMax
}

public class UISwitch : MonoBehaviour {

    [SerializeField]
    GameObject[] uis = new GameObject[(int)DungUIType.DungUITypeMax];
    [SerializeField]
    GameObject[] subUIs = new GameObject[(int)SubUIType.SubUITypeMax];

    [SerializeField]
    GameObject subCamera;

    [SerializeField]
    GameObject uiMenuButtons;

    public bool Interactable = true;
    public DungUIType UIType;

	// Use this for initialization
	void Start () {
        for ( int t = 0; t < uis.Length; t++ )
        {
            if ( uis[t] )
            {
                // 可視化
                uis[t].GetComponent<Canvas>().enabled = true;
            }
        }
        uiMenuButtons.GetComponent<Canvas>().enabled = true;
    }
	
    public void SwitchUI(int type)
    {
        if ( !Interactable ) return;

        // type に合致するものだけアクティブ化
        for (int t = 0; t < (int)DungUIType.DungUITypeMax; t++)
        {
            if ( uis[t] )
            {
                uis[t].SetActive(( t == type ) ? true : false);
            }
        }

        // サブカメラON/OFF
        subCamera.SetActive((type == (int)DungUIType.INVENTRY) ? true : false);

        // 「修練＆精製」「ステータス」などのウィンドウ選択ボタン
        uiMenuButtons.SetActive(( (int)DungUIType.INVENTRY <= type && type <= (int)DungUIType.SKILLTREE ) ? 
            true : false);

        UIType = (DungUIType) type;
    }

    public void SwitchSubUI(int type, bool on)
    {
        subUIs[type].SetActive(on);
    }


}
