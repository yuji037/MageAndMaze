using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour {

    public GameObject target = null;

    [SerializeField] bool _isSeekActive = true;
    // 線形補間の度合（0 ~ 1.0f)
    [SerializeField] float interpolation = 1.0f;

    // Use this for initialization
    void Start () {
		if ( target )
        {
            transform.position = target.transform.position;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!_isSeekActive) return;
        if ( target )
        {
            if ( interpolation == 1.0f )
            {
                transform.position = target.transform.position;
            }
            else
            {
                interpolation = Mathf.Clamp(interpolation, 0, 1);
                Vector3 dis = target.transform.position - transform.position;
                dis *= interpolation;
                transform.position += dis;
            }
        }
	}

    public void MoveInOneFrame()
    {
        transform.position = target.transform.position;
    }
}
