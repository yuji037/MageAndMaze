using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerMagicButton : EventTrigger {

    GameObject parent;
    PlayerAttack _playerAttack;
    [SerializeField]
    public int _skillNum;

    private void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        _playerAttack = parent.GetComponentInChildren<PlayerAttack>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _playerAttack.SetSkillRangeActive(_skillNum);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _playerAttack.DestroySkillRange();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        _playerAttack.MagicAttack(_skillNum);
        _playerAttack.DestroySkillRange();
    }


    Coroutine dragEvent;
    public override void OnPointerDown(PointerEventData eventData)
    {
        dragEvent = StartCoroutine(DragSkillButton());
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(dragEvent);
        _playerAttack.dragTime = 0;
    }

    IEnumerator DragSkillButton()
    {
        while ( true )
        {
            _playerAttack.dragTime += Time.deltaTime;
            yield return null;
        }
    }
}
