using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour {

    GameObject target;

	// Use this for initialization
	void Start () {
        target = Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target.transform.position);
	}
}
