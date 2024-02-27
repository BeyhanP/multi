using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShowBullet : MonoBehaviour
{
    public List<GameObject> skillSprites = new List<GameObject>();
    public bool skillBullet;
    public float _power;
    public int skillNumber;
    public TextMeshPro _powerText;
    public Skills skill;
    public int _skillLevel;
    public void SetBullet(float power,Skills _skill,bool gotSkill=false)
    {
        skillBullet = gotSkill;
        skill = _skill;
        if (!skillBullet)
        {
            Debug.Log(power.ToString() + "PowerSetted");
            _power = power;
            _powerText.text = power.ToString("");
        }
        else
        {
            _power = 0;
            _powerText.gameObject.SetActive(false);
            int skillNumber = ((int)_skill);
            for(int i = 0; i < skillSprites.Count; i++)
            {
                if (i == skillNumber)
                {
                    skillSprites[i].gameObject.SetActive(true);
                }
                else
                {
                    skillSprites[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
