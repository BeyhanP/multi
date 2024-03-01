using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerMain : MonoBehaviour
{
    [SerializeField] Image fillImager;
    int currentLevel;
    float currentPower;
    public List<int> PowerNeededForNewCapacity = new List<int>();
    public static PlayerMain instance;
    private void Awake()
    {
        //AddPower(PlayerPrefs.GetFloat("StartCapacityPower"));
        StartAddPower(PlayerPrefs.GetFloat("StartCapacityPower"));
        instance = this;
    }
    public void StartAddPower(float addAmounter)
    {
        currentPower += addAmounter;
        int oldLevel = currentLevel;
        int smallestLevel = 0;
        for (int i = 0; i < PowerNeededForNewCapacity.Count; i++)
        {
            if (currentPower >= PowerNeededForNewCapacity[i])
            {
                smallestLevel = i;
            }
        }
        currentLevel = smallestLevel;
        float fillAmount = currentPower - (float)PowerNeededForNewCapacity[currentLevel];
        fillAmount /= (float)PowerNeededForNewCapacity[currentLevel + 1] - (float)PowerNeededForNewCapacity[currentLevel];
        fillImager.DOFillAmount(fillAmount, .2f);
        int difference = currentLevel - oldLevel;
        for(int i = 0;i< difference; i++)
        {
            FindObjectOfType<NewShootingScript>().AddPart();
        }
    }
    public void AddPower(float addAmounter)
    {
        currentPower += addAmounter;
        int smallestLevel = 0;
        for (int i = 0; i < PowerNeededForNewCapacity.Count; i++)
        {
            if (currentPower >= PowerNeededForNewCapacity[i])
            {
                smallestLevel = i;
            }
        }
        currentLevel = smallestLevel;
        float fillAmount = currentPower - (float)PowerNeededForNewCapacity[currentLevel];
        fillAmount /= (float)PowerNeededForNewCapacity[currentLevel + 1] - (float)PowerNeededForNewCapacity[currentLevel];
        fillImager.DOFillAmount(fillAmount, .2f);
    }
}
