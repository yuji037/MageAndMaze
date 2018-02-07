using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pingpong_TextFade : MonoBehaviour {

    [SerializeField]
    Text text;
    Color color;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        color = text.color;
	}
	
	// Update is called once per frame
	void Update () {
        text.color = new Color(color.r, color.g, color.b, Mathf.PingPong(Time.time, 1.0f));
    }
}
