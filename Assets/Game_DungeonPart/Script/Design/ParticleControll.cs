using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControll : MonoBehaviour {

    float displayRange = 5;
    [SerializeField] ParticleSystem particle;

    GameObject parent;
    GameObject player;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dis = player.transform.position - transform.position;
        if ( dis.magnitude < 5 )
        {
            if ( !particle.isPlaying )
                particle.Play(true);
        }
        else if ( particle.isPlaying) particle.Stop(true);
        

	}
}
