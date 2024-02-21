using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ElephantSDK;
[System.Serializable]
public class FinalVarilPowers
{
    public List<int> varilPowers = new List<int>();
}
public class FinalVarilParent : MonoBehaviour
{
    [SerializeField] private float zDif, xDife;
    [SerializeField] private float zetAmount;
    [SerializeField] private GameObject finalObjectPrefab;
    [HideInInspector] public List<GameObject> varils;
    [SerializeField]
    public GameObject highScoreSign;
    private List<int> varilPowers = new List<int>();
    private FinalVarilPowers fvp;
    [SerializeField]private TextAsset varilPowersTextAsset;
    private void Start()
    {
        varilPowers.Clear();
        fvp = JsonUtility.FromJson<FinalVarilPowers>(RemoteConfig.GetInstance().Get("VarilPowers", varilPowersTextAsset.text));
        //fvp = JsonUtility.FromJson<FinalVarilPowers>(varilPowersTextAsset.text);
        for (int i = 0; i < fvp.varilPowers.Count; i++)
        {
            varilPowers.Add(fvp.varilPowers[i]);
        }
        Diz();
    }
    private void Diz()
    {
        if(PlayerPrefs.GetInt("HighScore") == 0)
        {
            highScoreSign.SetActive(false);
        }
        for (int i = 0; i < zetAmount; i++)
        {
            for (int a = 0; a < 3; a++)
            {
                GameObject finalObject = Instantiate(finalObjectPrefab, new Vector3(transform.position.x + a * xDife, transform.position.y, transform.position.z + i * zDif), Quaternion.Euler(Vector3.zero));
                FinishManager.instance.varils.Add(finalObject);
                finalObject.GetComponent<FinalObject>().zNum = i;
                finalObject.GetComponent<FinalObject>().power= varilPowers[i];
                finalObject.GetComponent<FinalObject>().SetPower();
            }
        }
        FinishManager.instance.PlaceHighScoreTabela();
    }
}
