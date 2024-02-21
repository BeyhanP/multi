using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShowBullet : MonoBehaviour
{
    public bool skillBullet;
    public TextMeshPro _powerText;
    public void SetBullet(float power)
    {
        if (!skillBullet)
        {
            _powerText.text = power.ToString("");
        }
    }
}
