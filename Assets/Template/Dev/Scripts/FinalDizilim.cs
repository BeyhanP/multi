using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDizilim : MonoBehaviour
{
    [SerializeField] private float zDif, xDife;
    [SerializeField] private float zetAmount;
    [SerializeField] private GameObject finalObjectPrefab;
    public float amountIncrease;
    public float amountIncreaseIncreaseAmount;
    public float carpan;
    public float startAmount;
    private void Awake()
    {
        Diz();
    }
    private void Diz()
    {
        for (int i = 0; i < zetAmount; i++)
        {
            for (int a = 0; a < 3; a++)
            {
                GameObject finalObject = Instantiate(finalObjectPrefab, new Vector3(transform.position.x + a * xDife, transform.position.y, transform.position.z + i * zDif), Quaternion.Euler(Vector3.zero));
                finalObject.GetComponent<FinalCollectable>().power = startAmount;
                finalObject.GetComponent<FinalCollectable>().SetPower();
            }


            startAmount += amountIncrease;
            if (i % carpan == 0)
            {
                if (i != 0)
                {
                    amountIncrease += amountIncreaseIncreaseAmount;
                }
            }
        }
    }
}
