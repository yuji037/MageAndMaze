using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpdUp : StraightShot
{
    public override void OnDecided()
    {
        base.OnDecided();
        player.abnoState.spdUpTurn = Mathf.FloorToInt(abnoEffectPower);
    }
}
