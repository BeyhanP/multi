using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class MiniGameMain : MonoBehaviour
{
    [SerializeField]private List<int> powerNeededForNewLayer = new List<int>();
    float currentPower;
    int currentLevel;
    [SerializeField] private List<GameObject> addSprites = new List<GameObject>();
    public TextMeshPro capacityTexter;
    private void Awake()
    {
        powerNeededForNewLayer.Clear();
        for(int i = 0; i < FindObjectOfType<PlayerMain>().PowerNeededForNewCapacity.Count; i++)
        {
            powerNeededForNewLayer.Add(FindObjectOfType<PlayerMain>().PowerNeededForNewCapacity[i]);
        }
        AddPower(PlayerPrefs.GetFloat("StartCapacityPower"));
    }
    public void AddPower(float hitPower)
    {
        currentPower += hitPower;
        int smallestLevel = 0;
        for(int i = 0; i < powerNeededForNewLayer.Count; i++)
        {
            if (currentPower >= powerNeededForNewLayer[i])
            {
                smallestLevel = i;
            }
        }
        for(int i = 0; i < addSprites.Count; i++)
        {
            if (i< smallestLevel)
            {
                if (!addSprites[i].gameObject.activeInHierarchy)
                {
                    addSprites[i].gameObject.SetActive(true);
                    Vector3 startScale = addSprites[i].transform.localScale;
                    addSprites[i].transform.localScale = Vector3.zero;
                    addSprites[i].transform.DOScale(startScale,.2f);
                }
            }
            else
            {
                addSprites[i].gameObject.SetActive(false);
            }
        }
        currentLevel = smallestLevel;
        capacityTexter.text = "+" + currentLevel.ToString() + " Capacity";
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOScale(Vector3.zero, .2f);
            GetComponent<Collider>().enabled = false;
            for(int i = 0; i < currentLevel; i++)
            {
                NewShootingScript.instance.AddPart();
            }
        }else if (other.CompareTag("Bullet"))
        {
            other.GetComponent<BulletScript>().BulletDeActivate(false, true);   
        }
    }
}
