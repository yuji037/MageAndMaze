using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSkillSet : MonoBehaviour {

    GameObject parent;
    PlayerSkillTree skillTree;

    [SerializeField]
    int[] setSkillNums;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        skillTree = parent.GetComponentInChildren<PlayerSkillTree>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DebugSetSkill()
    {
        int[] skills = skillTree.SetSkills;
        for ( var i = 0; i < skills.Length; i++ )
        {
            skills[i] = setSkillNums[i];
        }
    }
}
