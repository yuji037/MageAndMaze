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
    [SerializeField]
    GameObject switchEffect;
    [SerializeField]
    float switchCharaTime = 1;

    UISwitch uiSwitch;

    private void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        uiSwitch = parent.GetComponentInChildren<UISwitch>();
    }

    Coroutine playerSwitchCoroutine = null;
    public void SwitchChara()
    {
        // 切り替えアニメーション中はボタン入力を受け付けない
        if ( playerSwitchCoroutine != null) return;

        playerSwitchCoroutine = StartCoroutine(PlayerSwitchCoroutine());
    }

    IEnumerator PlayerSwitchCoroutine()
    {
        uiSwitch.Interactable = false;

        switchEffect.SetActive(true);
        yield return new WaitForSeconds(switchCharaTime);
        isAngel = !isAngel;
        ChangeChara(isAngel);
        yield return new WaitForSeconds(3 - switchCharaTime);
        playerSwitchCoroutine = null;

        uiSwitch.Interactable = true;
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
