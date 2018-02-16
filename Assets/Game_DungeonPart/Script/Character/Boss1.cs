using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Enemy {

    StageBoss stageBoss;

    protected override void DeathCheck()
    {
        if ( HP <= 0 && isAlive )
        {
            stageBoss = GetComponent<StageBoss>();
            StartCoroutine(stageBoss.DeathCoroutine());
            mapMn.SetCharaExistInfo(sPos);
            isAlive = false;
            Debug.Log("ボス死亡確認");
        }
    }

    public void DestroyMyBodys()
    {
        foreach ( GameObject aliveBody in myBodys )
        {
            Destroy(aliveBody);
        }
    }
}
