using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCollectablePiecer : MonoBehaviour
{
    public bool move;
    public float _power;
    private void Update()
    {
        if (move)
        {
            transform.Translate(new Vector3(0, 0, 1) * GameManager.instance.collectableSpeed * Time.deltaTime,Space.World);
        }
    }
}
