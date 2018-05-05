using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class PlayerSkillTree : MonoBehaviour
{
    //添字がスキルのIdになっている。中身はSkillData
    public Dictionary<int, SkillData> Skills = new Dictionary<int, SkillData>();
    public int[] SetSkills = new int[6] { 1, -1, -1, -1, -1, -1 };
    // Use this for initialization
    void Awake()
    {
        TextAsset playerSkillData= Resources.Load<TextAsset>("SkillData/skill_data")as TextAsset;

        StringReader reader = new StringReader(playerSkillData.text);
        reader.ReadLine();
        while (reader.Peek() > -1)
        {

            string line = reader.ReadLine();

            char[] delimiterChars = { ',' };
            string[] words = line.Split(delimiterChars);

            // スキルデータの格納
            int num=0;
            List<int> zenteiList=new List<int>();
            if(int.TryParse(words[4],out num)){ zenteiList.Add(num); }
            if (int.TryParse(words[5], out num)) { zenteiList.Add(num); }
            if (int.TryParse(words[6], out num)) { zenteiList.Add(num); }
            int[] useSoul= new int[] { int.Parse(words[8]), int.Parse(words[9]), int.Parse(words[10]) };
            Skills[int.Parse(words[0])] = new SkillData(words[1],int.Parse(words[2]),int.Parse(words[3]),zenteiList,words[7],useSoul);
        }
        //画像入れる。
        foreach (var i in Skills)
        {
            i.Value.setImage("Image/SkillUI/" + i.Key);
        }
        // セーブデータのロード
        LoadSkillData();
        // 序盤のマジックショットなら最初から覚えておく
        Skills[1].Syutoku = true;
    }

    [System.Serializable]
    public class SavableSkill
    {
        public int ID;
        public bool isSyutoku;

        public SavableSkill() { }
        public SavableSkill(int id, bool syutoku)
        {
            ID = id;
            isSyutoku = syutoku;
        }
    }

    void LoadSkillData()
    {
        // 習得状況のロード
        List<SavableSkill> data = SaveData.GetList<SavableSkill>("Skills", new List<SavableSkill>());
        foreach (SavableSkill d in data)
        {
            Skills[d.ID].Syutoku = d.isSyutoku;
        }

        // セット登録のロード
        List<int> setSkillList = SaveData.GetList<int>("SetSkills", new List<int>());
        if (setSkillList.Count < 6) return;
        for (int i = 0; i < SetSkills.Length; ++i)
        {
            SetSkills[i] = setSkillList[i];
        }
    }

    public void SaveSkillData()
    {
        // 習得状況のセーブ
        List<SavableSkill> data = new List<SavableSkill>();
        Dictionary<int, SkillData>.KeyCollection keyColl = Skills.Keys;
        foreach (int key in keyColl)
        {
            data.Add(new SavableSkill(key, Skills[key].Syutoku));
        }
        SaveData.SetList<SavableSkill>("Skills", data);

        // セット登録のセーブ
        List<int> setSkillList = new List<int>();
        for (int i = 0; i < SetSkills.Length; ++i)
        {
            setSkillList.Add(SetSkills[i]);
        }
        SaveData.SetList<int>("SetSkills", setSkillList);

        // 変更点のセーブ（必須）
        SaveData.Save();
    }
}
