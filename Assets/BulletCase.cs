using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class BulletCase : MonoBehaviour
{
    public List<int> skillNumber = new List<int>();
    public List<bool> isSkill = new List<bool>();
    public List<GameObject> bulletsInside = new List<GameObject>();
    public List<Transform> bulletPositions = new List<Transform>();
    public List<Transform> revolverParts = new List<Transform>();
    float singleBulletAngle;
    [SerializeField]int bulletAmount;
    [SerializeField]float radius;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _bulletTransformReference;
    [SerializeField] float power;
    public bool miniGameCase;
    int bulletNum;
    public float throwBackForce;
    public int startPartNumber;


    [SerializeField]List<int> fullData = new List<int>();
    public bool firstRevolver;
    private void Start()
    {
        if (firstRevolver)
        {
            
        }
        for (int i = 0; i < bulletPositions.Count; i++)
        {
            bulletPositions[i].transform.parent = revolverParts[i].transform;
            revolverParts[i].GetComponent<RevolverParts>()._bulletPosition = bulletPositions[i].gameObject;
        }
    }
    public void AddSkillBullet(Skills _bulletSkill,int skillLevel)
    {
        bool foundEmptyPosition = false;
        int emptyPosition = 0;
        for (int i = 0; i < fullData.Count; i++)
        {
            if (fullData[i] == 0)
            {
                if (!foundEmptyPosition)
                {
                    emptyPosition = i;
                    foundEmptyPosition = true;
                }
            }
        }
        if (foundEmptyPosition)
        {
            GameObject newBullet = Instantiate(_bulletPrefab);
            newBullet.transform.parent = bulletPositions[emptyPosition];
            newBullet.transform.localScale = Vector3.one;
            newBullet.transform.localPosition = Vector3.zero;
            newBullet.transform.localEulerAngles = Vector3.zero;
            bulletsInside.Add(newBullet);
            newBullet.GetComponent<ShowBullet>().SetBullet(0, _bulletSkill,skillLevel, true);
            revolverParts[emptyPosition].GetComponent<RevolverParts>()._bulletInside = newBullet.GetComponent<ShowBullet>();
            NewShootingScript.instance.bulletsInside.Add(newBullet.GetComponent<ShowBullet>());
            Debug.Log("AddedBullet");
            fullData[emptyPosition] = 1;
        }
        else
        {
            for(int i = 0; i < bulletsInside.Count; i++)
            {
                if (bulletsInside[i].GetComponent<ShowBullet>().skillBullet)
                {

                }
            }
        }
    }
    public void AddPowerBullet(float power)
    {
        bool foundEmptyPosition = false;
        int emptyPosition = 0;
        for (int i = 0; i < fullData.Count; i++)
        {
            if (fullData[i] == 0)
            {
                if (!foundEmptyPosition)
                {
                    emptyPosition = i;
                    foundEmptyPosition = true;
                }
            }
        }
        if (foundEmptyPosition)
        {
            GameObject newBullet = Instantiate(_bulletPrefab);
            newBullet.transform.parent = bulletPositions[emptyPosition];
            newBullet.transform.localScale = Vector3.one;
            newBullet.transform.localPosition = Vector3.zero;
            newBullet.transform.localEulerAngles = Vector3.zero;
            bulletsInside.Add(newBullet);
            newBullet.GetComponent<ShowBullet>().SetBullet(power, Skills.BiggerBullets,0, false);
            revolverParts[emptyPosition].GetComponent<RevolverParts>()._bulletInside = newBullet.GetComponent<ShowBullet>();
            NewShootingScript.instance.bulletsInside.Add(newBullet.GetComponent<ShowBullet>());
            fullData[emptyPosition] = 1;
        }
    }
    public GameObject GetBullet(int bulletNumber)
    {
        GameObject bullet = bulletsInside[bulletNumber];
        return bullet;
    }
    public void FormateInCircle()
    {
        for(int i = 0; i < bulletAmount; i++)
        {
            float currentAngle = i * singleBulletAngle;
            float xPosition = radius * Mathf.Sin(Mathf.Deg2Rad * currentAngle);
            float yPosition = radius * Mathf.Cos(Mathf.Deg2Rad * currentAngle);
            bulletsInside[i].transform.localPosition = new Vector3(0, yPosition, xPosition);
        }
    }
}
