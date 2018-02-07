using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ExpTable
{
    [SerializeField]
    public Text levelText;
    int[] expTable;

    Dictionary<int, int[]> levelTable = null;

    public int GetLevel(int exp)
    {
        if (levelTable == null )
        {
            levelTable = new Dictionary<int, int[]>();
            TextAsset levelData = Resources.Load<TextAsset>("PlayerExpTable/PlayerLevel") as TextAsset;
            StringReader reader = new StringReader(levelData.text);

            // 最初の行は日本語のみなので飛ばす
            reader.ReadLine();

            int _level = 0;
            while(reader.Peek() > -1 )
            {
                string line = reader.ReadLine();
                char[] delimiterChars = { ',' };
                string[] words = line.Split(delimiterChars);
                int.TryParse(words[0], out _level);

                int[] datas = new int[words.Length - 2];
                for ( int i = 0; i < words.Length - 2; ++i )
                {
                    int.TryParse(words[i+2], out datas[i]);
                }
                levelTable[_level] = datas;
            }
        }

        int level = 1;
        if (levelText == null)
        {
            //levelText = GameObject.Find("Level").GetComponent<Text>();
        }
        for (int i = 1; i <= levelTable.Count; ++i)
        {
            if (exp < levelTable[i][0])
            {
                level = i - 1;
                var nextExp = levelTable[i][0] - exp;
                //levelText.text = "Level: " + level + " 次のレベルまで" + nextExp;
                return level;
            }
        }
        return level;
    }

    public int GetHP(int level)
    {
        return levelTable[level][1];
    }
    public int GetMP(int level)
    {
        return levelTable[level][2];
    }
}
