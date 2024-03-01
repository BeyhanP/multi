using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using ElephantSDK;
public class GameManager : MonoBehaviour
{
    [Header("Canvas's")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject failCanvas;

    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI levelText;
    public bool started;
    [SerializeField] private GameObject nextButton;
    private int oldRandomLevel;

    public float collectableSpeed;

    public Transform _collectablePosition;


    public static GameManager instance;
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        RefreshCoinText();
        if (PlayerPrefs.GetInt("Level") == 0)
        {
            PlayerPrefs.SetInt("Level", 1);
        }
        levelText.text = "Level " + PlayerPrefs.GetInt("Level");
        if (PlayerPrefs.GetInt("LevelPref") == 0)
        {
            PlayerPrefs.SetInt("LevelPref", 1);
        }
        if (PlayerPrefs.GetInt("LevelPref") != SceneManager.GetActiveScene().buildIndex)
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("LevelPref"));
        }
        Elephant.LevelStarted(PlayerPrefs.GetInt("Level"));
    }
    public void RefreshCoinText()
    {
        coinText.text = PlayerPrefs.GetFloat("Coin").ToString("0");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextLevel();
        }else if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void StartGame()
    {
        //On Button Click
        
        startCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
        started = true;
        NewShootingScript.instance.GameStarted();
    }
    public void WinGame()
    {
        if (started)
        {
            Elephant.LevelCompleted(PlayerPrefs.GetInt("Level"));
            Taptic.Success();

            //Add Confetti Particle
            
            //

            StartCoroutine(CanvasSummon(winCanvas));
            started = false;
        }
    }
    public void FailGame()
    {
        if (started)
        {
            StartCoroutine(CanvasSummon(failCanvas));
            started = false;
        }
    }
    public void NextLevel()
    {
        if (PlayerPrefs.GetInt("LevelsFinished") == 0)
        {
            if (PlayerPrefs.GetInt("LevelPref") < SceneManager.sceneCountInBuildSettings - 1)
            {
                PlayerPrefs.SetInt("LevelPref", PlayerPrefs.GetInt("LevelPref") + 1);
            }
            else
            {
                PlayerPrefs.SetInt("LevelsFinished", 1);
                List<int> levelList = new List<int>();
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 1; i++)
                {
                    levelList.Add(i);
                }
                if (PlayerPrefs.GetInt("RandomizedLevels") == 0)
                {
                    PlayerPrefs.SetInt("RandomizedLevels", 1);
                }
                levelList.Remove(oldRandomLevel);
                int newLevel = levelList[Random.Range(1, levelList.Count)];
                if (newLevel == 0)
                {
                    newLevel = 1;
                }
                //oldRandomLevel = newLevel;
                PlayerPrefs.SetInt("OldRandomLevel", newLevel);
                PlayerPrefs.SetInt("LevelPref", newLevel);
            }
        }
        else
        {
            List<int> levelList = new List<int>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings ; i++)
            {
                levelList.Add(i);
            }
            if (PlayerPrefs.GetInt("RandomizedLevels") == 0)
            {
                PlayerPrefs.SetInt("RandomizedLevels", 1);
            }
            levelList.Remove(PlayerPrefs.GetInt("OldRandomLevel"));
            int newLevel = levelList[Random.Range(1, levelList.Count)];
            PlayerPrefs.SetInt("OldRandomLevel", newLevel);
            if (newLevel == 0)
            {
                newLevel = 1;
            }
            PlayerPrefs.SetInt("LevelPref", newLevel);
        }
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("LevelPref"));
    }

    public IEnumerator InstPowerAmountText(Vector3 instPos, float price)
    {
        GameObject newMoney = null;
        if (ObjectPooler.instance != null)
        {
            newMoney = ObjectPooler.instance.SpawnFromPool("PowerAddText", instPos, Quaternion.identity);
        }
        else
        {
            newMoney = FindObjectOfType<ObjectPooler>().SpawnFromPool("PowerAddText", instPos, Quaternion.identity);
        }
        newMoney.transform.DOKill();
        newMoney.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        newMoney.transform.position = Camera.main.WorldToScreenPoint(instPos);
        newMoney.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (price).ToString("0.0") + " Power";
        newMoney.transform.parent = inGameCanvas.transform;
        Vector3 startPos = newMoney.transform.position;
        newMoney.transform.localScale = Vector3.zero;
        newMoney.transform.DOScale(Vector3.one * .8f, .2f);
        newMoney.transform.position = new Vector3(newMoney.transform.position.x, newMoney.transform.position.y, newMoney.transform.position.z);
        newMoney.transform.DOMoveY(newMoney.transform.position.y + 300, 1.4f);
        yield return new WaitForSeconds(1f);
        newMoney.GetComponent<TextMeshProUGUI>().DOColor(new Color(), .3f);
        newMoney.transform.DOScale(Vector3.zero, .2f);
    }
    public IEnumerator InstNewMoney(Vector3 instPos, int side, float price)
    {
        //GameObject newMoney = Instantiate(newerMoney);
        price *= PlayerPrefs.GetFloat("IncomeMultiplier", 1);
        GameObject newMoney = ObjectPooler.instance.SpawnFromPool("GainText", transform.position, Quaternion.identity);
        newMoney.transform.DOKill();
        newMoney.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        newMoney.transform.position = Camera.main.WorldToScreenPoint(instPos);
        newMoney.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (price).ToString() + "$";
        newMoney.transform.parent = inGameCanvas.transform;
        Vector3 startPos = newMoney.transform.position;
        newMoney.transform.localScale = Vector3.zero;
        newMoney.transform.DOScale(Vector3.one * .8f, .2f);
        if (side == 0)
        {
            newMoney.transform.position = new Vector3(newMoney.transform.position.x, newMoney.transform.position.y, newMoney.transform.position.z);
        }
        else
        {
            newMoney.transform.position = new Vector3(newMoney.transform.position.x, newMoney.transform.position.y, newMoney.transform.position.z);
        }
        newMoney.transform.DOMoveY(newMoney.transform.position.y + 300, 1.4f);
        float oldCoinAmount = PlayerPrefs.GetFloat("Coin");
        PlayerPrefs.SetFloat("Coin", PlayerPrefs.GetFloat("Coin") + price);
        float to = PlayerPrefs.GetFloat("Coin");
        DOTween.To(() => oldCoinAmount, x => oldCoinAmount = x, to, .2f).OnUpdate(delegate {
            coinText.text = oldCoinAmount.ToString("0");
        }).OnComplete(delegate {
            RefreshCoinText();
        });
        yield return new WaitForSeconds(1f);
        newMoney.GetComponent<TextMeshProUGUI>().DOColor(new Color(), .3f);
        newMoney.transform.DOScale(Vector3.zero, .2f);
    }
    private IEnumerator CanvasSummon(GameObject canvasToSummon)
    {
        inGameCanvas.SetActive(false);
        yield return new WaitForSeconds(1);
        canvasToSummon.transform.localScale = Vector3.zero;
        canvasToSummon.SetActive(true);
        canvasToSummon.transform.DOScale(Vector3.one, .2f);
    }
}