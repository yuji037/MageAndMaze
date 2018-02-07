using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbnoStateType
{
    FREEZE,
    PARALIZE,

}

[System.Serializable]
public class AbnormalState {

    public int freezeTurn = 0;
    public int paralizeTurn = 0;

    public int atkUpTurn = 0;
    public int defUpTurn = 0;
    public int invincibleTurn = 0;
    public int transparentTurn = 0;
    public int spdUpTurn = 0;
}
