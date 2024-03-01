using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using ElephantSDK;
[System.Serializable]
public class UpgradeClass
{
    public string upgradeName;
    public Image upgradeImage;
    public List<Sprite> sprites = new List<Sprite>();
    public TextMeshProUGUI upgradeLevelText;
    public TextMeshProUGUI upgradePriceText;
    public List<int> upgradePrices = new List<int>();
}
[System.Serializable]
public class UpgradePrices
{
    public List<int> powerPrices = new List<int>();
    public List<int> ratePrices = new List<int>();
    public List<int> capacityPrices = new List<int>();
    public List<int> incomePrices = new List<int>();
}

public class UpgradeManager : MonoBehaviour
{
    public TextAsset defaultUpgradePrices;
    [SerializeField] private List<UpgradeClass> upgrades = new List<UpgradeClass>();
    private void Awake()
    {
        UpgradePrices up = new UpgradePrices();
        PlayerPrefs.SetFloat("Coin", 100000);
        up = JsonUtility.FromJson<UpgradePrices>(RemoteConfig.GetInstance().Get("UpgradePrices", defaultUpgradePrices.text));
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].upgradePrices.Clear();
        }
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (i == 0)
            {
                for (int u = 0; u < up.powerPrices.Count; u++)
                {
                    upgrades[i].upgradePrices.Add(up.powerPrices[u]);
                }
            }
            else if (i == 1)
            {
                for (int u = 0; u < up.ratePrices.Count; u++)
                {
                    upgrades[i].upgradePrices.Add(up.ratePrices[u]);
                }
            }
            else if (i == 2)
            {
                for (int u = 0; u < up.capacityPrices.Count; u++)
                {
                    upgrades[i].upgradePrices.Add(up.capacityPrices[u]);
                }
            }
            else if (i == 3)
            {
                for (int u = 0; u < up.incomePrices.Count; u++)
                {
                    upgrades[i].upgradePrices.Add(up.incomePrices[u]);
                }
            }
        }
        RefreshUI();
    }
    public void RefreshUI()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            RefreshUpgrade(i);
        }
    }
    public void RefreshUpgrade(int upgradeNumber)
    {
        UpgradeClass uc = upgrades[upgradeNumber];
        int price = 0;
        if (PlayerPrefs.GetInt(uc.upgradeName + "Level") < uc.upgradePrices.Count)
        {
            price = uc.upgradePrices[PlayerPrefs.GetInt(uc.upgradeName + "Level")];
        }
        else
        {
            price = uc.upgradePrices[uc.upgradePrices.Count - 1];
        }
        if (PlayerPrefs.GetFloat("Coin") >= price)
        {
            uc.upgradeImage.sprite = uc.sprites[0];
        }
        else
        {
            uc.upgradeImage.GetComponent<Button>().interactable = false;
        }
        uc.upgradeLevelText.text = "lvl " + (PlayerPrefs.GetInt(uc.upgradeName + "Level") + 1).ToString();
        uc.upgradePriceText.text = price.ToString() + "$";
    }
    public void PurchaseUpgrade(int upgradeNum)
    {
        UpgradeClass uc = upgrades[upgradeNum];
        int price = 0;
        if (PlayerPrefs.GetInt(uc.upgradeName + "Level") < uc.upgradePrices.Count)
        {
            price = uc.upgradePrices[PlayerPrefs.GetInt(uc.upgradeName + "Level")];
        }
        else
        {
            price = uc.upgradePrices[uc.upgradePrices.Count - 1];
        }
        switch (upgradeNum)
        {
            case 0:
                NewShootingScript.instance.StartPowerUpgrade();
                break;
            case 1:
                NewShootingScript.instance.StartRateUpgrade();
                break;
            case 2:
                NewShootingScript.instance.StartCapacityUpgrade();
                break;
            case 3:
                NewShootingScript.instance.IncomeMultiplierUpgrade();
                break;
        }
        if (PlayerPrefs.GetFloat("Coin") >= price)
        {
            PlayerPrefs.SetFloat("Coin", PlayerPrefs.GetFloat("Coin") - price);
            PlayerPrefs.SetInt(uc.upgradeName + "Level", PlayerPrefs.GetInt(uc.upgradeName + "Level") + 1);
            GameManager.instance.RefreshCoinText();
            StartCoroutine(ShakeMainBar(uc.upgradeImage.gameObject));
            RefreshUI();
        }
    }
    private IEnumerator ShakeMainBar(GameObject mainBar)
    {
        yield return new WaitForSeconds(0);
        Vector3 startScale = Vector3.one*.8f;
        Vector3 startEuler = Vector3.zero;
        mainBar.transform.DOScale(startScale * 1.1f, .15f);
        mainBar.transform.DOLocalRotate(startEuler + new Vector3(0, 0, -5), .1f);
        yield return new WaitForSeconds(.1f);
        mainBar.transform.DOLocalRotate(startEuler + new Vector3(0, 0, 5), .05f);
        yield return new WaitForSeconds(.05f);
        mainBar.transform.DOLocalRotate(startEuler + new Vector3(0, 0, 0), .05f);
        yield return new WaitForSeconds(.05f);
        mainBar.transform.DOScale(startScale, .1f);
    }
}
