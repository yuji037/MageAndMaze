using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbnoStateType
{
    Freeze,
    Paralize,
	AtkUp,
	DefUp,
	Invincible,
	Transparent,
	SpdUp,
	MAX,
}

[System.Serializable]
public class AbnormalState {

	public float[] m_fRemainTurns = new float[(int)AbnoStateType.MAX];

	public float GetTurn(AbnoStateType type)
	{
		return m_fRemainTurns[(int)type];
	}

	public void SetTurn(AbnoStateType type, float turn)
	{
		m_fRemainTurns[(int)type] = turn;
	}

	public void AddTurn(AbnoStateType type, float turn)
	{
		m_fRemainTurns[(int)type] += turn;
	}
}
