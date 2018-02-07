using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSelectWindowManager : MonoBehaviour {

    GameObject content;
    [SerializeField]
    RectTransform contentRect;

    [SerializeField]
    GameObject[] skillSetButton;
    Image[,] buttonImage = new Image[6, 2];

    [SerializeField]
    bool[] buttonActive;

    [SerializeField]
    float centerX = 900.0f;

    [SerializeField]
    float str = 300.0f;

	// Use this for initialization
	void Start () {
        contentRect = GetComponentInChildren<HorizontalLayoutGroup>().gameObject.GetComponent<RectTransform>();
        content = contentRect.gameObject;
        for (int i = 0; i < 6; i++)
        {
            buttonImage[i, 0] = skillSetButton[i].GetComponent<Image>();
            buttonImage[i, 1] = skillSetButton[i].GetComponentInChildren<SkillImage>().GetComponent<Image>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        int count = 0;
        float _parentX = contentRect.gameObject.transform.localPosition.x + 350;
        //Debug.Log(_parentX);
        Debug.Log(content.transform.localPosition);
		for (int i = 0; i < 6; i++)
        {
            skillSetButton[i].SetActive(buttonActive[i]);
            if (buttonActive[i])
            {
                float _btnPosX = skillSetButton[i].transform.localPosition.x;
                _btnPosX += 900;
                //Debug.Log(_btnPosX);
                float _deltaX = centerX - _parentX - _btnPosX;
                _deltaX = _deltaX * _deltaX;
                float _size = 1.0f - _deltaX / str;
                buttonImage[i, 0].color = new Color(0, 0, 0, 1 - (1 - _size) * 2.0f);
                buttonImage[i, 1].color = new Color(1, 1, 1, 1 - (1 - _size) * 2.0f);
                skillSetButton[i].transform.localScale = Vector3.one * _size;
                //Debug.Log("位置 = " + skillSetButton[0].transform.position.x);
                count++;
            }
        }
        contentRect.sizeDelta = new Vector2(count * 300, 300);
	}
}
