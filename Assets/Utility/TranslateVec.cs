using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateVec : MonoBehaviour {

    [SerializeField]
    Vector3 vector;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(vector * Time.deltaTime);
	}
}
