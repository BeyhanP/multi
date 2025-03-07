using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class ShowBullet : MonoBehaviour
{
    public List<GameObject> skillSprites = new List<GameObject>();
    public bool skillBullet;
    public float _power;
    public int skillNumber;
    public TextMeshPro _powerText;
    public TextMeshPro _multiLevelTexter;
    public Skills skill;
    public int _skillLevel;
    private IEnumerator DropAnimation()
    {
        GameObject newBullet = Instantiate(gameObject);
        newBullet.transform.parent = transform.parent;
        newBullet.GetComponentInChildren<TextMeshPro>().text = _power.ToString();
        newBullet.transform.localScale = transform.localScale;
        newBullet.transform.localPosition = transform.localPosition;
        newBullet.transform.localEulerAngles= transform.localEulerAngles;
        Vector3 currentScale = transform.localScale;
        transform.localScale = Vector3.zero;
        newBullet.gameObject.AddComponent<Rigidbody>().AddForce(new Vector3(0, .2f, -1) * 200);
        newBullet.gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(1, 0, 1) * 300);
        newBullet.gameObject.AddComponent<BoxCollider>();
        newBullet.transform.parent = null;
        yield return new WaitForSeconds(.2f + Time.deltaTime);
        transform.DOScale(currentScale, .2f);
    }
    public void SetBullet(float power,Skills _skill,int skillLevel,bool gotSkill=false)
    {
        skillBullet = gotSkill;
        skill = _skill;
        _skillLevel = skillLevel;
        if (!skillBullet)
        {
            //AnimationParter
            if (FindObjectOfType<GameManager>().started)
            {

                StartCoroutine(DropAnimation());
            }
            _power = power;
            _powerText.text = power.ToString("");
        }
        else
        {
            if (FindObjectOfType<GameManager>().started)
            {

                StartCoroutine(DropAnimation());
            }
            if (_skill == Skills.Multi)
            {
                _multiLevelTexter.text = "x" + (skillLevel + 2).ToString();
            }
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
