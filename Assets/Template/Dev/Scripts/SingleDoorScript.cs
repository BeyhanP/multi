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
    SkillDoor,
    BulletDoor
}
public class SingleDoorScript : MonoBehaviour
{
    [Header("Door Properties")]
    public DoorType _doorType;
    public Skills _doorSkill;
    public float amount;
    private Vector3 startScale;
    [SerializeField] List<GameObject> skillSprites = new List<GameObject>();
    [SerializeField] List<Transform> skillBalls = new List<Transform>();
    [SerializeField] List<Vector3> skillBallsStartScales = new List<Vector3>();
    [SerializeField] Transform _bulletPosition;
    [Header("Lock Properties")]
    public bool locked;
    [SerializeField] private int lockPower;
    [SerializeField] private Transform lockTriangle;
    [SerializeField] private Transform lockCanvas;
    [SerializeField] private Vector2 trianglePositions;
    private float triangleXMoveAmount;
    private float currentTrianglePosition;


    [SerializeField] private TextMeshPro amountText;
    [SerializeField] private TextMeshPro typeText;
    [SerializeField] private List<GameObject> doors = new List<GameObject>();
    private bool shaking;


    [SerializeField] private List<GameObject> lockObjects = new List<GameObject>();
    float objectThrowAmount;
    float currentObjectNumber;
    [Header("Skill Doors")]
    [SerializeField] List<TextMeshPro> _levelTexts = new List<TextMeshPro>();
    [SerializeField] SkinnedMeshRenderer _barRenderer;
    [SerializeField] List<int> powerNeededForNewLevelSkill = new List<int>();
    public SingleDoorScript otherDoor;
    [SerializeField] int startLevel;
    float currentPower;
    bool canStamina;
    [SerializeField] float staminaSpeed;
    int currentLevel;

    public TextMeshPro bulletPowerTexter;
    public TextMeshPro bulletAmountTexter;
    public List<int> powerNeededForNewBulleter;
    public int bulletStartPower;
    public int bulletAmounter;
    public GameObject _bulletDoorBulletInsider;
    private Vector3 bulletStartScaler = new Vector3();
    [SerializeField] TextMeshPro _lockTexter;
    private void Awake()
    {
        for (int i = 0; i < skillBalls.Count; i++)
        {
            skillBallsStartScales.Add(skillBalls[i].transform.localScale);
        }
        if (_doorType == DoorType.SkillDoor)
        {
            currentPower = powerNeededForNewLevelSkill[startLevel];
            GetHit(0);
        }
        else if (_doorType == DoorType.BulletDoor)
        {
           bulletStartScaler = _bulletDoorBulletInsider.transform.localScale;
            currentPower = powerNeededForNewBulleter[startLevel];
            BulletDoorHitter(0);
        }
        foreach (SingleDoorScript sds in FindObjectsOfType<SingleDoorScript>())
        {
            if (sds.transform.position.z == transform.position.z)
            {
                if (sds.gameObject != this.gameObject)
                {
                    otherDoor = sds;
                }
            }
        }
        if(_doorType == DoorType.SkillDoor || _doorType == DoorType.BulletDoor)
        {

        }
        else
        {
            if (lockPower > 0)
            {
                triangleXMoveAmount = (trianglePositions.y - trianglePositions.x) / lockPower;
                currentTrianglePosition = trianglePositions.x;
                lockTriangle.transform.localPosition = new Vector3(currentTrianglePosition, lockTriangle.transform.localPosition.y, lockTriangle.transform.localPosition.z);
                locked = true;
                _lockTexter.text = lockPower.ToString();
            }
            else
            {
                locked = false;
                _lockTexter.gameObject.SetActive(false);
                for (int i = 0; i < lockObjects.Count; i++)
                {
                    lockObjects[i].SetActive(false);
                }
                lockTriangle.transform.parent.gameObject.SetActive(false);
            }
        }
        startScale = transform.localScale;
        objectThrowAmount = (float)lockObjects.Count / (float)lockPower;
        SetDoorProperties();
    }
    public void ThrowObjects(float hitPower)
    {
        currentObjectNumber += hitPower * objectThrowAmount;
        for (int i = 0; i < currentObjectNumber; i++)
        {
            if (i < lockObjects.Count)
            {
                if (lockObjects[i].GetComponent<Rigidbody>() == null)
                {
                    lockObjects[i].AddComponent<Rigidbody>().AddForce(new Vector3(0, 0, -1) * 80 * Random.Range(0.8f, 1.3f));
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
            case DoorType.SkillDoor:
                typeText.text = _doorSkill.ToString();
                break;
            case DoorType.BulletDoor:
                typeText.text = "Bullet".ToString();
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
            if (_doorType == DoorType.SkillDoor)
            {
                doorNumber = 3;
            }
            if (_doorType == DoorType.BulletDoor)
            {
                doorNumber = 3;
            }
        }
        for (int i = 0; i < doors.Count; i++)
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
        if (_doorType == DoorType.SkillDoor)
        {
            int skillNumber = ((int)_doorSkill);
            for (int i = 0; i < skillSprites.Count; i++)
            {
                if (i == skillNumber)
                {
                    skillSprites[i].SetActive(true);
                }
                else
                {
                    skillSprites[i].SetActive(false);
                }
            }
        }
    }
    private void LockHit()
    {
        StartCoroutine(ShakeLockCanvas());
        lockPower -= 1;
        _lockTexter.text = lockPower.ToString();
        currentTrianglePosition += triangleXMoveAmount * 1;
        lockTriangle.transform.DOLocalMoveX(currentTrianglePosition, .2f);
        ThrowObjects(1);
        if (lockPower <= 0)
        {
            _lockTexter.transform.DOScale(Vector3.zero, .2f);
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
        amount += bulletPower;
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
    public void GetHit(float hitPower)
    {
        currentPower += hitPower;
        int smallestLevel = 0;
        for (int i = 0; i < powerNeededForNewLevelSkill.Count; i++)
        {
            if (currentPower >= powerNeededForNewLevelSkill[i])
            {
                smallestLevel = i;
            }
        }
        _levelTexts[0].text = (smallestLevel + 1).ToString();
        _levelTexts[1].text = (smallestLevel + 2).ToString();
        _levelTexts[2].text = "lvl" + (smallestLevel + 1).ToString();
        float fillAmount = currentPower - (float)powerNeededForNewLevelSkill[smallestLevel];
        fillAmount /= (float)powerNeededForNewLevelSkill[smallestLevel + 1] - (float)powerNeededForNewLevelSkill[smallestLevel];
        fillAmount *= 100;
        float currentFillAmount = _barRenderer.GetBlendShapeWeight(0);
        DOTween.To(() => currentFillAmount, x => currentFillAmount = x, fillAmount, .2f).OnUpdate(delegate {
            _barRenderer.SetBlendShapeWeight(0, currentFillAmount);
        }).OnComplete(delegate {
            canStamina = true;
        });
        currentLevel = smallestLevel;
        int skillNumber = ((int)_doorSkill);
        for (int i = 0; i < skillBalls.Count; i++)
        {
            if (i == skillNumber)
            {
                skillBalls[i].transform.DOScale(skillBallsStartScales[i] * 1.2f, .1f);
                skillBalls[i].transform.DOScale(skillBallsStartScales[i], .1f).SetDelay(.1f + Time.deltaTime);
            }
        }
    }
    public void BulletDoorHitter(float hitPower)
    {
        currentPower += hitPower;
        int smallestLevel = 0;
        for (int i = 0; i < powerNeededForNewBulleter.Count; i++)
        {
            if (currentPower >= powerNeededForNewBulleter[i])
            {
                smallestLevel = i;
            }
        }
        _levelTexts[0].text = (smallestLevel + 1).ToString();
        _levelTexts[1].text = (smallestLevel + 2).ToString();
        //_levelTexts[2].text = "lvl" + (smallestLevel + 1).ToString();
        float fillAmount = currentPower - (float)powerNeededForNewBulleter[smallestLevel];
        fillAmount /= (float)powerNeededForNewBulleter[smallestLevel + 1] - (float)powerNeededForNewBulleter[smallestLevel];
        fillAmount *= 100;
        float currentFillAmount = _barRenderer.GetBlendShapeWeight(0);
        DOTween.To(() => currentFillAmount, x => currentFillAmount = x, fillAmount, .2f).OnUpdate(delegate {
            _barRenderer.SetBlendShapeWeight(0, currentFillAmount);
        }).OnComplete(delegate {
            canStamina = true;
        });
        currentLevel = smallestLevel;
        _bulletDoorBulletInsider.transform.DOScale(bulletStartScaler * 1.2f, .1f);
        _bulletDoorBulletInsider.transform.DOScale(bulletStartScaler, .1f).SetDelay(.1f + Time.deltaTime);
        bulletPowerTexter.text = (currentLevel + 1).ToString();
        bulletAmountTexter.text ="X"+ bulletAmounter.ToString();
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
            other.GetComponent<BulletScript>().BulletDeActivate(true, true, GetComponent<Ricochetable>());
            if (_doorType != DoorType.SkillDoor)
            {
                
            }
            if(_doorType ==DoorType.SkillDoor)
            {
                GetHit(other.GetComponent<BulletScript>().bulletPower);
            }else if(_doorType == DoorType.BulletDoor)
            {
                BulletDoorHitter(other.GetComponent<BulletScript>().bulletPower);
            }
            else
            {
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
        }
        else if (other.CompareTag("Player"))
        {
            if (!locked)
            {
                transform.DOMoveY(transform.position.y - 5, .2f);
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
                    case DoorType.SkillDoor:
                        //StartCoroutine(ShootingScript.instance.GetSkillBullet(_bulletPosition, _doorSkill));
                        NewShootingScript.instance.GetSkillBullet(_doorSkill);
                        break;
                    case DoorType.BulletDoor:
                        for(int i = 0; i < bulletAmounter; i++)
                        {
                            NewShootingScript.instance.GetPowerBullet(currentLevel + 1);
                        }
                        break;
                }
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //GetHit(1);
            if (_doorType == DoorType.BulletDoor)
            {
                BulletDoorHitter(1);
            }
        }
        if (canStamina)
        {
            if (currentPower > powerNeededForNewLevelSkill[currentLevel])
            {
                currentPower -= Time.deltaTime * staminaSpeed;
                float fillAmount = currentPower - (float)powerNeededForNewLevelSkill[currentLevel];
                fillAmount /= (float)powerNeededForNewLevelSkill[currentLevel + 1] - (float)powerNeededForNewLevelSkill[currentLevel];
                fillAmount *= 100;
                _barRenderer.SetBlendShapeWeight(0, fillAmount);
            }
        }
    }
}
