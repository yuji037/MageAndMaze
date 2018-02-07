using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationChanger : MonoBehaviour {

    Animator anim;
    List<string> parameterNames;

    bool init = false;

	// Use this for initialization
	void Start () {
        //anim = GetComponent<Animator>();
        //bool end = false;

        //int num = 0;
        //parameterNames = new List<string>();
        //int parameterCount = anim.parameterCount;
        
        //while (!end && num < parameterCount)
        //{
        //    string name = anim.GetParameter(num).name;
        //    //Debug.Log(name);
        //    if (name != null)
        //    {
        //        parameterNames.Add(name);
        //        //parameterNames.RemoveAt(number);
        //        num++;
        //    }
        //    else { /*end = true;*/ }
        //}

	}

    public void Init()
    {
        anim = GetComponent<Animator>();
        bool end = false;

        int num = 0;
        parameterNames = new List<string>();
        int parameterCount = anim.parameterCount;

        while ( !end && num < parameterCount )
        {
            string name = anim.GetParameter(num).name;
            //Debug.Log(name);
            if ( name != null )
            {
                parameterNames.Add(name);
                //parameterNames.RemoveAt(number);
                num++;
            }
            else { /*end = true;*/ }
        }
        init = true;
    }
	
	// Update is called once per frame
	void Update () {

        if ( !init ) Init();

        var info = anim.GetCurrentAnimatorStateInfo(0);
        foreach (string name in parameterNames)
        {
            if (name == "Move") continue;
            if (name == "Idle") continue;
            if (name == "Grabbed") continue;
            if (info.IsName(name))
            {
                anim.SetBool(name, false);
            }
        }
	}

    public void TriggerAnimator(string name, bool on = true)
    {
        anim.SetBool(name, on);
    }
}
