using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillData
{

    // スキルデータを入れるコンストラクタ
    public SkillData(string skillName, int use_mp, int _damage, List<int> zenteiSkillId, string _description, int[] useSoul)
    {
        Syutoku = false;
        SkillName = skillName;
        ZenteiSkillId = zenteiSkillId;
        UseSoul = useSoul;
        skillDescription = _description;
        useMp = use_mp;
        damage = _damage;
    }
    public void setImage(string imagePas)
    {
        skillImage = Resources.Load<Sprite>(imagePas) as Sprite;

    }
    public int[] UseSoul;
    public List<int> ZenteiSkillId;
    public bool Syutoku;
    public int useMp;
    public int damage;
    public Sprite skillImage;
    public string SkillName;
    public string skillDescription;
}
