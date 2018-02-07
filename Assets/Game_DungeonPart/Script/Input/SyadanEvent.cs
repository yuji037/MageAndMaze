using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SyadanEvent : EventTrigger{
    public override void OnPointerDown(PointerEventData eventData)
    {   
        base.OnPointerClick(eventData);
        GameObject.Find("SkillTreePanel").GetComponent<SkillTreeButtonManager>().Syadan();
    }
}
