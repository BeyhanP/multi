using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerMain : MonoBehaviour
{
    [SerializeField] Image fillImager;
    public int currentLevel;
    float currentPower;
    public List<int> PowerNeededForNewCapacity = new List<int>();
    
    public static PlayerMain instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        PowerNeededForNewCapacity.Clear();
        GameManager gm = FindObjectOfType<GameManager>();
        for (int i = 0; i < gm._specs.miniGamePowers.Count; i++)
        {
            PowerNeededForNewCapacity.Add(gm._specs.miniGamePowers[i]);
        }
        StartAddPower(PlayerPrefs.GetFloat("StartCapacityPower"));
    }
    public void StartAddPower(float addAmounter)
    {
        Debug.Log("PowerAdded");
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
        if (currentLevel >= PowerNeededForNewCapacity.Count - 1)
        {
            int dif = currentLevel - oldLevel;
            for (int i = 0; i < dif; i++)
            {
                
                FindObjectOfType<NewShootingScript>().AddPart();
            }
            Debug.Log("Returning");
            return;
        }
        Debug.Log("FillCalled");
        float fillAmount = currentPower - (float)PowerNeededForNewCapacity[currentLevel];
        fillAmount /= (float)PowerNeededForNewCapacity[currentLevel + 1] - (float)PowerNeededForNewCapacity[currentLevel];
        fillImager.DOFillAmount(fillAmount, .2f);
        Debug.Log("FillAmounter"+fillAmount);
        int difference = currentLevel - oldLevel;
        for (int i = 0; i < difference; i++)
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
        if (currentLevel >= PowerNeededForNewCapacity.Count - 1)
        {
            fillAmount = 1;
            fillImager.DOFillAmount(fillAmount, .2f);
            return;
        }
        fillAmount /= (float)PowerNeededForNewCapacity[currentLevel + 1] - (float)PowerNeededForNewCapacity[currentLevel];
        fillImager.DOFillAmount(fillAmount, .2f);
    }
}
