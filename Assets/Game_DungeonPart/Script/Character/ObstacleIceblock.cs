using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleIceblock : Obstacle {

    protected override void DeathCheck(bool emitDeathEffect = true)
    {
        base.DeathCheck();
        OnGroundObjectManager ogoMn = parent.GetComponentInChildren<OnGroundObjectManager>();
        ogoMn.AddOnGroundObject(OnGroundObject.Type.WATER, pos);
    }
}
