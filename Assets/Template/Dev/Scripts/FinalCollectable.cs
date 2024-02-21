using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FinalCollectable : MonoBehaviour
{
    //BlendShape
    private float firstBlend;
    private float secondBlend;
    private float thirdBlend;
    float currentFirstBlend;
    float currentSeconfBlend;
    float currentThirdBlend;
    public SkinnedMeshRenderer _renderer;

    float startPower;
    private float blendIncraseAmount;




    [HideInInspector]public float power;
    [SerializeField]private List<GameObject> moneysInside =new List<GameObject>();
    private TextMeshPro powerText;
    private void Awake()
    {
        powerText = GetComponentInChildren<TextMeshPro>();
        startPower = power;
        powerText.text = power.ToString("0");
        blendIncraseAmount = 250f / startPower;
    }
    public void SetPower()
    {
        startPower = power;
        blendIncraseAmount = 250f / startPower;
        powerText.text = power.ToString("0");
    }
    public void IncreaseBlendShape(float increaseAmount)
    {
        float left = 0;
        float leftFromSecond = 0;
        if (firstBlend < 100)
        {
            firstBlend += increaseAmount*1.5f;
            if (firstBlend > 100)
            {
                left = firstBlend - 100;
                IncreaseBlendShape(left);
                firstBlend = 100;
            }
        }
        else if(secondBlend<100)
        {
            secondBlend += increaseAmount+left;
            if (secondBlend >= 98)
            {
                leftFromSecond= secondBlend- 100;
                IncreaseBlendShape(leftFromSecond);
                secondBlend = 100;
            }
        }else if (thirdBlend < 50 &&secondBlend>=100)
        {
            thirdBlend += increaseAmount+left;
            if (thirdBlend > 50)
            {
                thirdBlend = 50f;
            }
        }


        float toThree = thirdBlend;
        DOTween.To(() => currentThirdBlend, x => currentThirdBlend = x, toThree, .5f).OnUpdate(delegate {
            _renderer.SetBlendShapeWeight(2, currentThirdBlend);
        }).OnComplete(delegate {
            _renderer.SetBlendShapeWeight(2, thirdBlend);
        });

        float to = firstBlend;
        DOTween.To(() => currentFirstBlend, x => currentFirstBlend = x, to, .2f).OnUpdate(delegate {
            _renderer.SetBlendShapeWeight(0, currentFirstBlend);
        }).OnComplete(delegate {
            _renderer.SetBlendShapeWeight(0, firstBlend);
        });


        float toGo = secondBlend;
        DOTween.To(() => currentSeconfBlend, x => currentSeconfBlend = x, toGo, .2f).OnUpdate(delegate {
            _renderer.SetBlendShapeWeight(1, currentSeconfBlend);
        }).OnComplete(delegate {
            _renderer.SetBlendShapeWeight(1, secondBlend);
        });
        if (power <= 0)
        {
            transform.DOScale(Vector3.zero, .2f);
            GetComponent<Collider>().enabled = false;
            for (int i = 0; i < moneysInside.Count; i++)
            {
                moneysInside[i].transform.parent = null;
                moneysInside[i].transform.DOJump(moneysInside[i].transform.position, 3, 1, .5f).SetEase(Ease.Linear);
                moneysInside[i].transform.DORotate(moneysInside[i].transform.eulerAngles + new Vector3(0, 180, 0), .5f);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            other.GetComponent<Collider>().enabled = false;
            other.transform.localScale = Vector3.zero;
            if (power > 0)
            {
                power -= FindObjectOfType<ShootingScript>().power;
            }
            if (power < 0)
            {
                powerText.text = "0";

            }
            else
            {
                powerText.text = power.ToString("0");

            }
            IncreaseBlendShape(blendIncraseAmount* FindObjectOfType<ShootingScript>().power);
        }else if (other.CompareTag("Player"))
        {
            for (int i = 0; i < moneysInside.Count; i++)
            {
                moneysInside[i].transform.parent = null;
                moneysInside[i].transform.DOJump(moneysInside[i].transform.position, 3, 1, .5f).SetEase(Ease.Linear);
                moneysInside[i].transform.DORotate(moneysInside[i].transform.eulerAngles + new Vector3(0, 180, 0), .5f);
            }
        }
    }
}
