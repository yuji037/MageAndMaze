using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour {

    GameObject parent;
    DungeonPartManager dMn;
    public Player player;
    public Slider hpSlider;
    public Slider mpSlider;
    public Slider stSlider;
    Text floorText;
    Text hpText;
    Text mpText;

    int hp = 0;
    int maxHp = 0;
    int mp = 0;
    int maxMp = 0;
    int stamina = 0;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        player = parent.GetComponentInChildren<Player>();
        hpSlider = GameObject.Find("HP").GetComponentInChildren<Slider>();
        mpSlider = GameObject.Find("MP").GetComponentInChildren<Slider>();
        stSlider = GameObject.Find("Stamina").GetComponentInChildren<Slider>();
        floorText = GameObject.Find("FloorText").GetComponent<Text>();
        hpText = hpSlider.GetComponentInChildren<Text>();
        mpText = mpSlider.GetComponentInChildren<Text>();
    }

    int floor = -1;
	// Update is called once per frame
	void Update () {

        bool shouldUpdate = ( floor != dMn.floor
            || hp != player.HP || maxHp != player.MaxHP || mp != player.MP || maxMp != player.MaxMP || stamina != player.Stamina);

        if ( shouldUpdate )
        {
            UpdatePlayerUI();
        }
    }


    void UpdatePlayerUI()
    {
        floorText.text = "英魂の洞窟" + dMn.floor + "F";

        hpSlider.value = (float)player.HP / player.MaxHP;
        mpSlider.value = (float)player.MP / player.MaxMP;
        stSlider.value = (float)player.Stamina / 100;
        hpText.text = player.HP + " / " + player.MaxHP;
        mpText.text = player.MP + " / " + player.MaxMP;

        floor = dMn.floor;
        hp = player.HP;
        maxHp = player.MaxHP;
        mp = player.MP;
        maxMp = player.MaxMP;
        stamina = player.Stamina;
    }
}
