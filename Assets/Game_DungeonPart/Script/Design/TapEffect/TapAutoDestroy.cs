using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapAutoDestroy : MonoBehaviour {

    private float speed = 2.0f;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.8f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += Vector3.one * Time.deltaTime * speed;
	}
}
