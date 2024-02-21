using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class BulletCase : MonoBehaviour
{
    public List<GameObject> bulletsInside = new List<GameObject>();
    public List<Transform> bulletPositions = new List<Transform>();
    public List<Transform> revolverParts = new List<Transform>();
    float singleBulletAngle;
    [SerializeField]int bulletAmount;
    [SerializeField]float radius;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _bulletTransformReference;
    [SerializeField] float power;
    public bool miniGameCase;
    int bulletNum;
    public float throwBackForce;
    private void Awake()
    {
        bulletsInside.Clear();
        for (int i = 0; i < bulletAmount; i++)
        {
            GameObject newBullet = Instantiate(_bulletPrefab);
            newBullet.transform.parent = bulletPositions[i];
            newBullet.transform.localScale = Vector3.one;
            newBullet.transform.localPosition = Vector3.zero;
            newBullet.transform.localEulerAngles = Vector3.zero;
            bulletsInside.Add(newBullet);
        }
        bulletAmount = bulletsInside.Count;
        singleBulletAngle = 360f / (float)bulletAmount;
        FormateInCircle();
    }
    private void Start()
    {
        for (int i = 0; i < bulletsInside.Count; i++)
        {
            bulletsInside[i].transform.parent = revolverParts[i].transform;
        }
    }
    public GameObject GetBullet(int bulletNumber)
    {
        GameObject bullet = bulletsInside[bulletNumber];
        return bullet;
    }
    public void FormateInCircle()
    {
        for(int i = 0; i < bulletAmount; i++)
        {
            float currentAngle = i * singleBulletAngle;
            float xPosition = radius * Mathf.Sin(Mathf.Deg2Rad * currentAngle);
            Debug.Log(xPosition + "xPosition"+i+ "_CosValue" + Mathf.Cos(Mathf.Deg2Rad * currentAngle) + "_CurrentAngle" + currentAngle);
            float yPosition = radius * Mathf.Cos(Mathf.Deg2Rad * currentAngle);
            bulletsInside[i].transform.localPosition = new Vector3(0, yPosition, xPosition);
        }
    }
}
