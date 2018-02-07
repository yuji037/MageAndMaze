using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Enemy {

    protected override void DeathCheck()
    {
        if ( HP <= 0 && isAlive )
        {
            //KillReward();
            mapMn.SetCharaExistInfo(sPos);
            foreach ( GameObject deadObj in deadObjPrefab )
            {
                var obj = Instantiate(deadObj);
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                Destroy(obj, 5.0f);
            }
            //foreach ( GameObject aliveBody in myBodys )
            //{
            //    Destroy(aliveBody);
            //}

            isAlive = false;
        }
    }

    //IEnumerator
}
