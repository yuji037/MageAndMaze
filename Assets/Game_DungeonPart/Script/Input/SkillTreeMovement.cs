using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeMovement : MonoBehaviour {

    // VerticalLayoutGroup の Padding を固定する

    [SerializeField] GameObject content;
    VerticalLayoutGroup verticalLayoutGroup;
    float startPadding = 0;

    // Use this for initialization
    void Start () {
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        startPadding = verticalLayoutGroup.padding.right;
        StartCoroutine(CheckCoroutine());
	}
    
    IEnumerator CheckCoroutine()
    {
        while ( true )
        {
            if(Input.touchCount >= 2 )
            {
                yield return StartCoroutine(FixPadding());
            }
            yield return null;
        }
    }

    IEnumerator FixPadding()
    {
        while ( Input.touchCount >= 2 )
        {
            verticalLayoutGroup.padding.right = Mathf.FloorToInt(startPadding / content.transform.localScale.x);
            yield return null;
        }
    }
}
