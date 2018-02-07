using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCharaSwitch : MonoBehaviour {

    [SerializeField] GameObject angel;
    [SerializeField] GameObject reaper;
    [SerializeField] Avatar angelAvatar;
    [SerializeField] Avatar reaperAvatar;

    GameObject parent;
    Player player;

    public bool isAngel = false;

    private void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
    }

    public void SwitchChara()
    {
        isAngel = !isAngel;

        ChangeChara(isAngel);
    }

    public void ChangeChara(bool _isAngel)
    {
        reaper.SetActive(!_isAngel);
        angel.SetActive(_isAngel);
        player.GetComponent<Animator>().avatar = _isAngel ? angelAvatar : reaperAvatar;
    }

    public void LoadChara()
    {
        int load = SaveData.GetInt("AngelOrReaper", 0);
        isAngel = ( load == 0 ) ? true : false;
        ChangeChara(isAngel);
    }

    public void SaveChara()
    {
        int save = ( isAngel ) ? 0 : 1;
        SaveData.SetInt("AngelOrReaper", save);
    }
}
