using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemUtility : MonoBehaviour {

    float dragTime = 0;
    GameObject parent;
    EventSystem eventSystem;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        eventSystem = parent.GetComponentInChildren<EventSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		//if(eventSystem)
	}
}
