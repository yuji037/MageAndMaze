using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInactivate : MonoBehaviour {

    [SerializeField]
    float inactivateTime = 4;

    private void OnEnable()
    {
        StartCoroutine(InactivateCoroutine());
    }

    IEnumerator InactivateCoroutine()
    {
        yield return new WaitForSeconds(inactivateTime);

        gameObject.SetActive(false);
    }
}
