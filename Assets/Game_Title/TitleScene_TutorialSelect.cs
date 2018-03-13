using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene_TutorialSelect : MonoBehaviour {

    [SerializeField] Toggle toggle_isTutorialON;

	// Use this for initialization
	void Start () {
        toggle_isTutorialON.isOn = ( SaveData.GetInt("IsTutorialON", 1) == 1 ) ? true : false;
	}

    public void OnTapToggle()
    {
        SaveData.SetInt("IsTutorialON", toggle_isTutorialON.isOn ? 1 : 0);
        Debug.Log("セーブ");
        SaveData.Save();
    }
}
