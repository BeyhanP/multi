using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CameraValues
{
    public Vector3 Angle;
    public Vector3 Offset;
}

public class CameraFollow : MonoBehaviour
{
    [SerializeField]private Transform target;
    public Vector3 offset;
    public bool clamped;
    [SerializeField]private Vector2 clamp;

    public List<CameraValues> _camValues;
    public bool follow;
    private void Awake()
    {
        follow = true;
        offset = target.transform.position - transform.position;
        _camValues[0].Offset = offset;
        _camValues[0].Angle = transform.localEulerAngles;
    }
    private void LateUpdate()
    {
        if (follow)
        {
            transform.position = target.transform.position - offset;
            if (clamped)
            {
                Vector3 transformPosition = transform.position;
                transformPosition.x = Mathf.Clamp(transformPosition.x,clamp.x, clamp.y);
                transform.position = transformPosition;
            }
        }
    }
    public void SetValues(int num)
    {
        Vector3 angle = _camValues[num].Angle;
        Vector3 offsetNew = _camValues[num].Offset;
        transform.DORotate(angle, .5f);
        // Tween customVector to 3,4,2 in 2 seconds
        DOTween.To(() => offset, x => offset = x, offsetNew, .5f);
    }
    
}
