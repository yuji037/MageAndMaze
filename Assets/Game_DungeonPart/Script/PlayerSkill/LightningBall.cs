using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBall : StraightShot {

    public override void OnDecided()
    {
        battleTarget.abnoState.paralizeTurn = 10;
    }
}
