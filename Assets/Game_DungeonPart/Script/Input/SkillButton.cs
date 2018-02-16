using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillButton : MonoBehaviour
{
    public int skillnum;
    private PlayerSkillTree pst;
    public enum jotai { GOT, UN_GOT, CANT_GET };
    public List<GameObject> Branch = null;
    public jotai Jotai;
    public void init(int Skillnum)
    {
        var parent = GameObject.Find("GameObjectParent");
        //プレイヤーからスキルツリーデータを持ってくる
        pst = parent.GetComponentInChildren<PlayerSkillTree>();
        skillnum = Skillnum;
        jotaiKousin();
    }
    public void jotaiKousin()
    {
        //習得済み
        if (pst.Skills[skillnum].Syutoku)
        {
            Jotai = jotai.GOT;
        }
        //未習得
        else
        {
            if (pst.Skills[skillnum].ZenteiSkillId == null)
            {
                Jotai = jotai.UN_GOT;

            }
            else
            {
                bool ok = true;
                foreach (int i in pst.Skills[skillnum].ZenteiSkillId)
                {
                    if (!pst.Skills[i].Syutoku)
                    {
                        ok = false;
                    }
                }
                if (ok)
                {
                    Jotai = jotai.UN_GOT;
                }
                else
                {
                    Jotai = jotai.CANT_GET;
                }
            }
        }

        switch (Jotai)
        {
            //習得済み
            case jotai.GOT:
                GetComponent<Image>().color = Color.white;
                GetComponent<Button>().interactable = true;
                if (Branch!=null)
                {
                    foreach (var i in Branch)
                    {
                        i.GetComponent<Image>().color = Color.white;
                    }
                }
                break;
            //未習得&&前提スキル習得済み
            case jotai.UN_GOT:
                GetComponent<Image>().color = new Color(0,0,0,0.8f);
                GetComponent<Button>().interactable = true;
                break;
            //未習得&&前提スキル未習得
            case jotai.CANT_GET:
                GetComponent<Image>().color = new Color(0, 0, 0, 100);
                GetComponent<Button>().interactable = true; 
                break;
        }
    }
}
