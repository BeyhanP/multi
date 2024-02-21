using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishManager : MonoBehaviour
{
    public GameObject highScoreSign;
    [HideInInspector] public List<GameObject> varils;
    public static FinishManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void PlaceHighScoreTabela()
    {
        int highestZnum = PlayerPrefs.GetInt("HighScore");
        if (highestZnum != 0)
        {
            bool found = false;
            for (int i = 0; i < varils.Count; i++)
            {
                if (varils[i].GetComponent<FinalObject>().zNum == highestZnum)
                {
                    if (!found)
                    {
                        highScoreSign.transform.position = new Vector3(highScoreSign.transform.position.x, highScoreSign.transform.position.y, varils[i].transform.position.z + 2);
                        Vector3 localScale = highScoreSign.transform.localScale;
                        highScoreSign.transform.localScale = Vector3.zero;
                        highScoreSign.transform.DOScale(localScale, .3f);
                        highScoreSign.transform.DOJump(highScoreSign.transform.position, 1, 1, .3f);
                        found = true;
                    }
                }
            }
        }
        else
        {
            highScoreSign.transform.localScale = Vector3.zero;
        }
    }
    public void CheckHighScore(int zNumber)
    {
        Debug.Log("HighScore"+"_"+zNumber);
        if (PlayerPrefs.GetInt("HighScore") < zNumber)
        {
            bool found = false;
            for (int i = 0; i < varils.Count; i++)
            {
                if (varils[i].GetComponent<FinalObject>().zNum == zNumber)
                {
                    if (!found)
                    {
                        highScoreSign.SetActive(true);
                        highScoreSign.transform.position = new Vector3(FindObjectOfType<PlayerMovement>().transform.position.x, highScoreSign.transform.position.y, varils[i].transform.position.z + 2);
                        Vector3 localScale = Vector3.one;
                        highScoreSign.transform.localScale = Vector3.zero;
                        highScoreSign.transform.DOScale(localScale, .3f);
                        highScoreSign.transform.DOJump(highScoreSign.transform.position, 1, 1, .3f);
                        found = true;
                    }
                }
            }
            PlayerPrefs.SetInt("HighScore", zNumber);
        }
    }
}
