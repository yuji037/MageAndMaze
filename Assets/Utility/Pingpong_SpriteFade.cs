using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pingpong_SpriteFade : MonoBehaviour {

    SpriteRenderer _sprite;
    //Color color;

	// Use this for initialization
	void Start () {
        _sprite = GetComponent<SpriteRenderer>();
        //color = _sprite.color;
	}
	
	// Update is called once per frame
	void Update () {
        _sprite.color = new Color(1, 1, 1, Mathf.PingPong(Time.time, 1.0f));
    }
}
