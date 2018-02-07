using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchInOut : MonoBehaviour
{
    
    // 2本指で拡大・縮小するためのクラス
    [SerializeField] GameObject content;
    [SerializeField] float defaultScale = 0.7f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 1.5f;
    [SerializeField] float sensitivity = 10.0f;


    // Use this for initialization
    void Start()
    {
        content.transform.localScale = Vector3.one * defaultScale;
        StartCoroutine(CheckCoroutine());
    }
    

    IEnumerator CheckCoroutine()
    {
        while ( true )
        {
            if ( Input.touchCount >= 2 && content.activeInHierarchy)
            {
                yield return StartCoroutine(PinchInPinchOut());
            }
            yield return null;
        }
    }

    

    IEnumerator PinchInPinchOut()
    {
        Touch t1 = Input.GetTouch(0);
        Touch t2 = Input.GetTouch(1);
        float sqrMagStartDist = ( t2.position - t1.position ).sqrMagnitude;


        Vector3 startContentPos = content.transform.position;
        Vector3 startTouchPos = t1.position;
        Vector3 startDist = startContentPos - startTouchPos;

        float startScale = content.transform.localScale.x;
        
        while ( Input.touchCount >= 2 )
        {
            t1 = Input.GetTouch(0);
            t2 = Input.GetTouch(1);
            float sqrMagNowDist = ( t2.position - t1.position ).sqrMagnitude;
            float diff = sqrMagNowDist - sqrMagStartDist;
            diff *= 0.000001f * sensitivity;
            float scale = startScale + diff;
            scale = Mathf.Clamp(scale, minScale, maxScale);
            content.transform.localScale = Vector3.one * scale;

            Vector3 contentDist = startDist * ( 1 + diff );
            content.transform.position = (Vector3)t1.position + contentDist;


            
            yield return null;
        }
    }
}
