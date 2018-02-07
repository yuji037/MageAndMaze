using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLineRenderer : MonoBehaviour {

    [SerializeField] GameObject lineRendererPrefab;
    [SerializeField] float emissionRate = 3;
    [SerializeField] Color baseColor;
    LineRenderer lineRen;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
