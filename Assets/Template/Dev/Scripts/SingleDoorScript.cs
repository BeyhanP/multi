using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public enum DoorType
{
    FireRate,
    FireRange,
    FirePower,
}
public class SingleDoorScript : MonoBehaviour
{
    [Header ("Door Properties")]
    public DoorType _doorType;
    public float amount;
    private Vector3 startScale;

    [Header ("Lock Properties")]
    public bool locked;
    [SerializeField] private int lockPower;
    [SerializeField] private Transform lockTriangle;
    [SerializeField] private Transform lockCanvas;
    [SerializeField] private Vector2 trianglePositions;
    private float triangleXMoveAmount;
    private float currentTrianglePosition;


    [SerializeField] private TextMeshPro amountText;
    [SerializeField] private TextMeshPro typeText;
    [SerializeField]private List<GameObject> doors = new List<GameObject>();
    private bool shaking;


    [SerializeField]private List<GameObject> lockObjects = new List<GameObject>();
    float objectThrowAmount;
    float currentObjectNumber;

    public SingleDoorScript otherDoor;
    private void Awake()
    {
        foreach(SingleDoorScript sds in FindObjectsOfType<SingleDoorScript>())
        {
            if (sds.transform.position.z == transform.position.z)
            {
                if (sds.gameObject != this.gameObject)
                {
                    otherDoor = sds;
                }
            }
        }
        if (lockPower > 0)
        {
            triangleXMoveAmount = (trianglePositions.y - trianglePositions.x) / lockPower;
            currentTrianglePosition = trianglePositions.x;
            lockTriangle.transform.localPosition = new Vector3(currentTrianglePosition, lockTriangle.transform.localPosition.y, lockTriangle.transform.localPosition.z);
            locked = true;
        }
        else
        {
            locked = false;
            for(int i = 0; i < lockObjects.Count; i++)
            {
                lockObjects[i].SetActive(false);
            }
            lockTriangle.transform.parent.gameObject.SetActive(false);
        }
        startScale = transform.localScale;
        objectThrowAmount = (float)lockObjects.Count / (float)lockPower;
        SetDoorProperties();
    }
    public void ThrowObjects(float hitPower)
    {
        currentObjectNumber += hitPower*objectThrowAmount;
        for(int i = 0; i < currentObjectNumber; i++)
        {
            if (i < lockObjects.Count)
            {
                if (lockObjects[i].GetComponent<Rigidbody>() == null)
                {
                    lockObjects[i].AddComponent<Rigidbody>().AddForce(new Vector3(0, 0, -1) * 80*Random.Range(0.8f,1.3f));
                    lockObjects[i].GetComponent<Rigidbody>().AddTorque(Vector3.one * 200);
                    lockObjects[i].AddComponent<BoxCollider>();
                    lockObjects[i].transform.parent = null;
                }
            }
        }
    }
    private void SetDoorProperties()
    {
        switch (_doorType)
        {
            case DoorType.FireRate:
                typeText.text = "RATE";
                break;
            case DoorType.FireRange:
                typeText.text = "RANGE";
                break;
            case DoorType.FirePower:
                typeText.text = "POWER";
                break;
        }
        if (amount < 0)
        {
            amountText.text = amount.ToString("0");
            
        }
        else
        {
            amountText.text = "+" + amount.ToString("0");
        }
        int doorNumber;
        if (locked)
        {
            doorNumber = 2;
        }
        else
        {
            if (amount >= 0)
            {
                doorNumber = 0;
            }
            else
            {
                doorNumber = 1;
            }
        }
        for(int i = 0; i < doors.Count; i++)
        {
            if (i == doorNumber)
            {
                doors[i].SetActive(true);
            }
            else
            {
                doors[i].SetActive(false);
            }
        }
    }
    private void LockHit()
    {
        StartCoroutine(ShakeLockCanvas());
        lockPower -= 1;
        currentTrianglePosition += triangleXMoveAmount * 1;
        lockTriangle.transform.DOLocalMoveX(currentTrianglePosition, .2f);
        ThrowObjects(1);
        if (lockPower < 0)
        {
            lockCanvas.transform.DOScale(Vector3.zero, .2f);
            locked = false;
            SetDoorProperties();
        }
    }
    private IEnumerator ShakeLockCanvas()
    {
        lockCanvas.transform.DORotate(new Vector3(0, 0, 3), .1f);
        yield return new WaitForSeconds(.1f);
        lockCanvas.transform.DORotate(new Vector3(0, 0, -3), .1f);
        yield return new WaitForSeconds(.1f);
        lockCanvas.transform.DORotate(new Vector3(0, 0, 0), .1f);
    }
    private void IncreaseAmount(float bulletPower)
    {
        float beforeAmount = amount;
        amount+=bulletPower;
        SetDoorProperties();
        if (!shaking)
        {
            StartCoroutine(Shake());
            shaking = true;
        }
    }
    private IEnumerator Shake()
    {
        amountText.transform.DOScale(Vector3.one * 1.1f, .1f);
        amountText.transform.DORotate(new Vector3(0, 0, 5), .1f);
        transform.DOScaleY(startScale.y * 1.1f, .1f);
        transform.DOScaleX(startScale.x * 1.05f, .1f);
        yield return new WaitForSeconds(.1f + Time.deltaTime);
        amountText.transform.DORotate(new Vector3(0, 0, -5), .1f);
        transform.DOScaleY(startScale.y, .1f);
        transform.DOScaleX(startScale.x, .1f);
        yield return new WaitForSeconds(.1f + Time.deltaTime);
        amountText.transform.DORotate(new Vector3(0, 0, 0), .1f);
        shaking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            /*
            GameObject bulletHit = ObjectPooler.instance.SpawnFromPool("DoorHit", other.transform.position, Quaternion.identity);
            foreach (ParticleSystem ps in bulletHit.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
            if (amount >= 0)
            {
                GameObject doorPositiveHit = ObjectPooler.instance.SpawnFromPool("DoorPositiveHit", other.transform.position, Quaternion.identity);
                foreach (ParticleSystem ps in doorPositiveHit.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
            }
            */
            other.GetComponent<BulletScript>().BulletDeActivate(true);
            if (!locked)
            {
                IncreaseAmount(other.GetComponent<BulletScript>().bulletPower);
                Taptic.Light();
            }
            else
            {
                LockHit();
                Taptic.Medium();
            }
        }
        else if (other.CompareTag("Player"))
        {
            transform.DOMoveY(transform.position.y - 5,.2f);
            GetComponent<Collider>().enabled = false;
            switch (_doorType)
            {
                case DoorType.FireRate:
                    ShootingScript.instance.FireRateUpgrade(amount);
                    break;
                case DoorType.FireRange:
                    ShootingScript.instance.FireRangeUpgrade(amount);
                    break;
                case DoorType.FirePower:
                    ShootingScript.instance.FirePowerUpgrade(amount);
                    break;
            }
        }
    }
}
