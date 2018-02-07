using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RateData
{
    public int ID;
    public int rate;
}

public class EnemySpawnData {


    public List<RateData> data = new List<RateData>();

    public EnemySpawnData(params int[] p)
    {
        for (var i = 0; i < p.Length; i += 2)
        {
            //if (p[i+1] == 0 ) {
            //    // rate 0% なので無効データ
            //    continue;
            //}
            RateData _dat = new RateData();
            _dat.ID = p[i];
            _dat.rate = p[i + 1];
            //data.Add( RateData { p[i], p[i + 1] });
            data.Add(_dat);
        }
    }
}
