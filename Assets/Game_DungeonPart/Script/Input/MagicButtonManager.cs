using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicButtonManager : MonoBehaviour {

    GameObject parent;
    Player player;

    [SerializeField]
    GameObject[] _magicButtons;
    Image[] _skillIcons;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        _skillIcons = new Image[_magicButtons.Length];
        for (int i = 0; i < _skillIcons.Length; i++)
        {
            _skillIcons[i] = _magicButtons[i].GetComponentInChildren<SkillImage>().GetComponent<Image>();
        }
        UpdateMagic();
	}

    private void OnEnable()
    {
        if ( !player ) Start();
        UpdateMagic();
    }

    public void UpdateMagic()
    {
        PlayerSkillTree sMn = player.GetComponent<PlayerSkillTree>();

        for (int i = 0; i < sMn.SetSkills.Length; i++)
        {
            if (sMn.SetSkills[i] == -1)
            {
                _magicButtons[i].SetActive(false);
            }
            else
            {
                int skillID = sMn.SetSkills[i];
                SkillData skill = sMn.Skills[skillID];
                if (skill != null)
                {
                    _magicButtons[i].SetActive(true);
                    _magicButtons[i].GetComponent<EventTriggerMagicButton>()._skillNum = skillID;
                    _skillIcons[i].sprite = skill.skillImage;
                }
            }
        }
    }
}
