using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using ElephantSDK;
public class NewShootingScript : MonoBehaviour
{
    [SerializeField] List<Transform> rotatePositions = new List<Transform>();
    float angleAddAmount = 360f / 5f;
    [SerializeField] float fireRate;
    [SerializeField] AnimationCurve rotateAnimCurve;
    int openBaseCount;
    [SerializeField] List<GameObject> basesInside = new List<GameObject>();
    List<Transform> basePositions = new List<Transform>();
    [SerializeField] List<Transform> _singlePosition;
    [SerializeField] List<Transform> _doublePosition;
    [SerializeField] List<Transform> _triplePosition;
    public List<ShowBullet> bulletsInside;


    public float waitTime;
    int partCount;
    [SerializeField] List<RevolverParts> _partsInside = new List<RevolverParts>();
    [SerializeField] List<RevolverParts> _unlockedParts = new List<RevolverParts>();


    public Skills skillToPut;
    List<int> fullData = new List<int>();
    [SerializeField] List<Transform> shootPositions = new List<Transform>();
    [SerializeField] private List<GameObject> _weaponsInside = new List<GameObject>();


    [Header("ShootPart")]
    [SerializeField] float currentRange;
    public float shootForce;
    [SerializeField] Transform shootPosition;
    private bool spreadShotUnlocked;
    bool canShoot;
    [SerializeField] List<ParticleSystem> muzzleParticles = new List<ParticleSystem>();
    int rotAmounter;
    public static NewShootingScript instance;
    private void Awake()
    {
        instance = this;
        canShoot = true;
        for (int i = 0; i < _partsInside.Count; i++)
        {
            fullData.Add(0);
        }
        partCount = 5;
        for (int i = 0; i < partCount; i++)
        {
            _unlockedParts.Add(_partsInside[i]);
            PowerBulletAdd(PlayerPrefs.GetFloat("StartPower", 1));
            fullData[i] = 1;
        }
        fireRate = RemoteConfig.GetInstance().GetFloat("StartRate", 1) + PlayerPrefs.GetInt("RateUpgradedAmount") * RemoteConfig.GetInstance().GetFloat("UpgradeRateIncAmount", .02f);
        currentRange = RemoteConfig.GetInstance().GetFloat("StartRange", 20);
        PlaceBases();
    }
    public void StartRateUpgrade()
    {
        PlayerPrefs.SetInt("RateUpgradedAmount", PlayerPrefs.GetInt("RateUpgradedAmount") + 1);
        fireRate = RemoteConfig.GetInstance().GetFloat("StartRate", 1) + PlayerPrefs.GetInt("RateUpgradedAmount") * RemoteConfig.GetInstance().GetFloat("UpgradeRateIncAmount", .02f);
    }
    public void FireRateUpgrade(float amount)
    {
        fireRate += RemoteConfig.GetInstance().GetFloat("RateIncAmount", .01f) * amount;
    }
    public void FireRangeUpgrade(float amount)
    {
        currentRange+= RemoteConfig.GetInstance().GetFloat("RangeIncAmount", .01f) * amount;
    }
    public void IncomeMultiplierUpgrade()
    {
        PlayerPrefs.SetFloat("IncomeMultiplier", PlayerPrefs.GetFloat("IncomeMultiplier") + .1f);
    }
    public void StartCapacityUpgrade()
    {
        PlayerPrefs.SetFloat("StartCapacityPower", PlayerPrefs.GetFloat("StartCapacityPower") + RemoteConfig.GetInstance().GetFloat("CapacityUpgradeAmount", 3));
        PlayerMain.instance.StartAddPower(RemoteConfig.GetInstance().GetFloat("CapacityUpgradeAmount", 3));
        
        foreach(MiniGameMain mgm in FindObjectsOfType<MiniGameMain>())
        {
            mgm.AddPower(RemoteConfig.GetInstance().GetFloat("CapacityUpgradeAmount", 3));
        }
    }
    public void StartPowerUpgrade()
    {
        PlayerPrefs.SetFloat("StartPower", PlayerPrefs.GetFloat("StartPower") + .5f);
        for(int i = 0; i < bulletsInside.Count; i++)
        {
            bulletsInside[i].GetComponent<ShowBullet>()._power = PlayerPrefs.GetFloat("StartPower");
            bulletsInside[i].GetComponent<ShowBullet>().SetBullet(PlayerPrefs.GetFloat("StartPower"), Skills.BiggerBullets,0, false);
        }
        
    }
    public void PowerBulletAdd(float power)
    {
        basesInside[0].GetComponent<BulletCase>().AddPowerBullet(power);
    }
    public void GameStarted()
    {
        StartCoroutine(RotateToNext());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddPart();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            GetPowerBullet(2);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GetSkillBullet(skillToPut,1);
        }
    }
    public void GetSkillBullet(Skills _bulletSkill,int skillLevel)
    {
        bool gotEmptyPosition = false;
        int smallestEmptyPosition = 0;
        for (int i = 0; i < partCount; i++)
        {
            if (fullData[i] == 0)
            {
                if (!gotEmptyPosition)
                {
                    smallestEmptyPosition = i;
                    gotEmptyPosition = true;
                    fullData[i] = 1;
                }
            }
        }
        if (gotEmptyPosition)
        {
            if (smallestEmptyPosition < 5)
            {
                basesInside[0].GetComponent<BulletCase>().AddSkillBullet(_bulletSkill, skillLevel);
            }
            else if (smallestEmptyPosition >= 5 && smallestEmptyPosition < 10)
            {
                basesInside[1].GetComponent<BulletCase>().AddSkillBullet(_bulletSkill, skillLevel);
            }
            else if (smallestEmptyPosition >= 10)
            {
                basesInside[2].GetComponent<BulletCase>().AddSkillBullet(_bulletSkill, skillLevel);
            }
        }
        else
        {

        }
    }
    public void GetPowerBullet(float newPower)
    {
        bool gotEmptyPosition = false;
        int smallestEmptyPosition = 0;
        for (int i = 0; i < partCount; i++)
        {
            if (fullData[i] == 0)
            {
                if (!gotEmptyPosition)
                {
                    smallestEmptyPosition = i;
                    gotEmptyPosition = true;
                    fullData[i] = 1;
                }
            }
        }
        if (gotEmptyPosition)
        {
            if (smallestEmptyPosition < 5)
            {
                basesInside[0].GetComponent<BulletCase>().AddPowerBullet(newPower);
            }
            else if (smallestEmptyPosition >= 5 && smallestEmptyPosition < 10)
            {
                basesInside[1].GetComponent<BulletCase>().AddPowerBullet(newPower);
            }
            else if (smallestEmptyPosition >= 10)
            {
                basesInside[2].GetComponent<BulletCase>().AddPowerBullet(newPower);
            }
        }
        else
        {
            if (SmallestPoweredBullet() < newPower)
            {
                SmallestBullet().SetBullet(newPower, Skills.BiggerBullets,0);
            }
            else
            {

            }
        }
    }
    public ShowBullet SmallestBullet()
    {
        float smallestPower = Mathf.Infinity;
        int smallestI = 0;
        for (int i = 0; i < partCount; i++)
        {
            if (!bulletsInside[i].skillBullet)
            {
                if (bulletsInside[i]._power <= smallestPower)
                {
                    smallestPower = bulletsInside[i]._power;
                    smallestI = i;
                }
            }
        }
        return bulletsInside[smallestI];
    }
    public float SmallestPoweredBullet()
    {
        float smallestPower = Mathf.Infinity;
        int smallestI = 0;
        for (int i = 0; i < partCount; i++)
        {
            if (!bulletsInside[i].skillBullet)
            {
                if (bulletsInside[i]._power <= smallestPower)
                {
                    smallestPower = bulletsInside[i]._power;
                    smallestI = i;
                }
            }
        }
        return smallestPower;
    }
    public void AddPart()
    {
        if (partCount < _partsInside.Count)
        {
            partCount++;
        }
        for (int i = 0; i < _partsInside.Count; i++)
        {
            if (i < partCount)
            {
                if (!_partsInside[i].gameObject.activeInHierarchy)
                {
                    Vector3 scale = _partsInside[i].transform.localScale;
                    _partsInside[i].gameObject.SetActive(true);
                    _partsInside[i].transform.localScale = Vector3.zero;
                    _partsInside[i].transform.DOScale(scale, .2f).SetEase(rotateAnimCurve);
                    if (!_unlockedParts.Contains(_partsInside[i]))
                    {
                        _unlockedParts.Add(_partsInside[i]);
                    }
                }
            }
            else
            {
                _partsInside[i].gameObject.SetActive(false);
            }
        }
        PlaceBases();
    }
    public void PlaceBases()
    {
        basePositions.Clear();

        if (partCount <= 5)
        {
            foreach (Transform bb in _singlePosition)
            {
                basePositions.Add(bb);
            }
            openBaseCount = 1;
        }
        else if (partCount > 5 && partCount <= 10)
        {
            foreach (Transform bb in _doublePosition)
            {
                basePositions.Add(bb);
            }
            openBaseCount = 2;
        }
        else if (partCount > 10)
        {
            foreach (Transform bb in _triplePosition)
            {
                basePositions.Add(bb);
            }
            openBaseCount = 3;
        }
        for (int i = 0; i < _weaponsInside.Count; i++)
        {
            if (i == openBaseCount - 1)
            {
                _weaponsInside[i].SetActive(true);
            }
            else
            {
                _weaponsInside[i].SetActive(false);
            }
        }
        for (int i = 0; i < basesInside.Count; i++)
        {
            if (i < openBaseCount)
            {
                if (!basesInside[i].gameObject.activeInHierarchy)
                {
                    basesInside[i].gameObject.SetActive(true);
                    basesInside[i].transform.localScale = Vector3.zero;
                }
            }
            else
            {
                basesInside[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < openBaseCount; i++)
        {
            Transform refPos = basePositions[i];
            BulletCase _caseScript = basesInside[i].GetComponent<BulletCase>();
            _caseScript.transform.parent = refPos;
            _caseScript.transform.DOScale(Vector3.one, .2f).SetEase(rotateAnimCurve);
            //_caseScript.transform.localPosition = Vector3.zero;
            if (_caseScript.transform.localPosition != Vector3.zero)
            {
                _caseScript.transform.DOLocalMove(Vector3.zero, .2f).SetEase(rotateAnimCurve);
            }
            _caseScript.transform.localEulerAngles = Vector3.zero;
        }
    }
    public IEnumerator RotateToNext()
    {
        for (int i = 0; i < rotatePositions.Count; i++)
        {
            rotatePositions[i].transform.DOLocalRotate(new Vector3(angleAddAmount, 0, 0), .5f / fireRate, RotateMode.LocalAxisAdd).SetEase(rotateAnimCurve);
        }
        yield return new WaitForSeconds(.5f / fireRate);
        Debug.Log(GetActiveBullets().Count +"ActiveCounter");
        Shoot(GetActivePower(), GetActiveSkills(),GetActiveBullets());
        rotAmounter++;
        if (rotAmounter < 5)
        {

            yield return new WaitForSeconds(.5f / fireRate);
        }
        else
        {
            rotAmounter = 0;
            yield return new WaitForSeconds(waitTime);
        }
        StartCoroutine(RotateToNext());
    }
    public List<Skills> GetActiveSkills()
    {
        List<Skills> skillsToGive = new List<Skills>();
        
        return skillsToGive;
    }
    public List<ShowBullet> currentBullets()
    {
        List<ShowBullet> bullets = new List<ShowBullet>();
        return bullets;
    }
    public Skills SecondBaseSkills()
    {
        Skills _skillToGive = Skills.BiggerBullets;
        int partCount = openBaseCount;
        float totalPower = 0;
        List<RevolverParts> _parts = new List<RevolverParts>();
        for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().revolverParts.Count; i++)
        {
            _parts.Add(basesInside[0].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
        }
        if (partCount == 1)
        {
            _parts = _parts.OrderBy(part => part.transform.position.y).ToList();
            if (_parts[^1].GetComponent<ShowBullet>().skillBullet)
            {
                _skillToGive = _parts[^1].GetComponent<ShowBullet>().skill;
                ShakeBullet(_parts[^1]._bulletInside);
            }
        }
        else
        {
            _parts = _parts.OrderBy(part => part.transform.position.x).ToList();
            if (_parts[^1].GetComponent<ShowBullet>().skillBullet)
            {
                _skillToGive = _parts[^1].GetComponent<ShowBullet>().skill;

                ShakeBullet(_parts[^1]._bulletInside);
            }
        }
        return _skillToGive;
    }
    public Skills ThirdBaseSkills()
    {
        Skills _skillToGive = Skills.BiggerBullets;
        return _skillToGive;
    }
    private void ShakeBullet(ShowBullet _ss)
    {
        _ss._powerText.color = Color.green;
        _ss._powerText.DOColor(Color.white, .1f).SetDelay(.1f);
        _ss._powerText.transform.DOScale(Vector3.one * 1.4f, .2f);
        _ss._powerText.transform.DOScale(Vector3.one, .1f).SetDelay(.2f);
    }
    public float GetActivePower()
    {
        float totPower = 0;

        totPower += GetFirstBasePower() + GetSecondBasePower() + GetThirdBasePower();
        return totPower;
    }
    public List<ShowBullet> GetActiveBullets()
    {
        List<ShowBullet> _bulletsToReturn = new List<ShowBullet>();
        Debug.Log("Bcount" + bulletsInside.Count);
        _bulletsToReturn.Add(FirstBullet());
        Debug.Log("Fcount" + bulletsInside.Count);
        if (SecondBullet() != null)
        {
            Debug.Log("SecondNotNuller");
            Debug.Log(SecondBullet().name+"GotSecondBullet");
            ShowBullet _bulletToAdder = SecondBullet();
            _bulletsToReturn.Add(_bulletToAdder);
        }
        Debug.Log("Scount" + bulletsInside.Count);
        if (ThirdBullet() != null)
        {
            _bulletsToReturn.Add(ThirdBullet());
        }
        Debug.Log("Tcount" + bulletsInside.Count);
        return _bulletsToReturn;
    }
    public ShowBullet FirstBullet()
    {
        int partCount = openBaseCount;

        ShowBullet bullet = null;
        List<RevolverParts> _parts = new List<RevolverParts>();
        for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().revolverParts.Count; i++)
        {
            _parts.Add(basesInside[0].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
        }
        if (partCount == 1)
        {
            _parts = _parts.OrderBy(part => part.transform.position.y).ToList();
            ShakeBullet(_parts[^1]._bulletInside);
        }
        else
        {
            _parts = _parts.OrderBy(part => part.transform.position.x).ToList();
           
            ShakeBullet(_parts[^1]._bulletInside);
        }
        bullet = _parts[^1]._bulletInside;
        return bullet;
    }
    public float GetFirstBasePower()
    {
        int partCount = openBaseCount;
        float totalPower = 0;
        List<RevolverParts> _parts = new List<RevolverParts>();
        for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().revolverParts.Count; i++)
        {
            _parts.Add(basesInside[0].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
        }
        if (partCount == 1)
        {
            _parts = _parts.OrderBy(part => part.transform.position.y).ToList();
            totalPower += _parts[^1]._bulletInside._power;
            ShakeBullet(_parts[^1]._bulletInside);
        }
        else
        {
            _parts = _parts.OrderBy(part => part.transform.position.x).ToList();
            totalPower += _parts[^1]._bulletInside._power;
            ShakeBullet(_parts[^1]._bulletInside);
        }
        return totalPower;
    }
    public ShowBullet SecondBullet()
    {
        ShowBullet bullet = null;
        List<RevolverParts> _parts = new List<RevolverParts>();
        for (int i = 0; i < basesInside[1].GetComponent<BulletCase>().revolverParts.Count; i++)
        {
            _parts.Add(basesInside[1].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
        }
        _parts = _parts.OrderBy(part => part.transform.position.x).ToList();
        if (_parts[0].gameObject.activeInHierarchy && _parts[0]._bulletInside != null)
        {
            Debug.Log("GotSecondBulleter");
            bullet = _parts[0]._bulletInside;
        }
        return bullet;
    }
    public float GetSecondBasePower()
    {
        float totalPower = 0;
        List<RevolverParts> _parts = new List<RevolverParts>();
        for (int i = 0; i < basesInside[1].GetComponent<BulletCase>().revolverParts.Count; i++)
        {
            _parts.Add(basesInside[1].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
        }
        _parts = _parts.OrderBy(part => part.transform.position.x).ToList();
        if (_parts[0].gameObject.activeInHierarchy && _parts[0]._bulletInside != null)
        {
            totalPower += _parts[0]._bulletInside._power;
            ShakeBullet(_parts[0]._bulletInside);
        }
        return totalPower;
    }
    public ShowBullet ThirdBullet()
    {
        ShowBullet _bullet = null;
        if (openBaseCount >= 3)
        {
            List<RevolverParts> _parts = new List<RevolverParts>();
            for (int i = 0; i < basesInside[2].GetComponent<BulletCase>().revolverParts.Count; i++)
            {
                _parts.Add(basesInside[2].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
            }
            _parts = _parts.OrderBy(part => part.transform.position.y).ToList();
            if (_parts[0].gameObject.activeInHierarchy && _parts[0]._bulletInside != null)
            {
                _bullet = _parts[0]._bulletInside;
            }
        }
        return _bullet;
    }
    public float GetThirdBasePower()
    {
        float totalPower = 0;
        if (openBaseCount >= 3)
        {
            List<RevolverParts> _parts = new List<RevolverParts>();
            for (int i = 0; i < basesInside[2].GetComponent<BulletCase>().revolverParts.Count; i++)
            {
                _parts.Add(basesInside[2].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
            }
            _parts = _parts.OrderBy(part => part.transform.position.y).ToList();
            if (_parts[0].gameObject.activeInHierarchy && _parts[0]._bulletInside != null)
            {
                totalPower += _parts[0]._bulletInside._power;
                ShakeBullet(_parts[0]._bulletInside);
            }
        }
        return totalPower;
    }
    public void Shoot(float shootPower, List<Skills> _skillsInside,List<ShowBullet> _bulletsToUser)
    {
        if (canShoot && GameManager.instance.started)
        {
            for(int p= 0; p< shootPositions.Count; p++)
            {
                shootPosition = shootPositions[p];
                int shootAmount = 1;
                List<Skills> _skillers = new List<Skills>();
                for (int i = 0; i < _bulletsToUser.Count; i++)
                {
                    if (_bulletsToUser[i].skillBullet)
                    {
                        _skillers.Add(_bulletsToUser[i].skill);
                    }
                }

                for (int i = 0; i < _bulletsToUser.Count; i++)
                {
                    if (_bulletsToUser[i].skill == Skills.Multi)
                    {
                        int level = _bulletsToUser[i]._skillLevel;
                        shootAmount = level + 2;
                    }
                }
                /*
                if (_skillers.Contains(Skills.Multi))
                {
                    int indexNumber = _skillers.IndexOf(Skills.Multi);
                    int multiLeveler = _bulletsToUser[indexNumber]._skillLevel + 2;
                    Debug.Log("GotMultier"+multiLeveler+ "SkilLeveler"+ _bulletsToUser[indexNumber]._skillLevel);
                    shootAmount = multiLeveler;
                }
                */
                if (shootAmount == 0)
                {
                    shootAmount = 1;
                }
                GetComponent<Animator>().Play("ShootAnimation");
                Taptic.Light();
                if (shootAmount == 1)
                {
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                    for (int i = 0; i < muzzleParticles.Count; i++)
                    {
                        muzzleParticles[i].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, _bulletsToUser);
                    bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                    bullet.transform.position = shootPosition.position;
                    Vector3 shootVector = Vector3.forward * shootForce;
                    bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                }
                else if (shootAmount == 2)
                {
                    for (int s = 0; s < 2; s++)
                    {
                        GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                        bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        for (int i = 0; i < muzzleParticles.Count; i++)
                        {
                            muzzleParticles[i].Play();
                        }
                        bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, _bulletsToUser);
                        bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                        bullet.transform.position = shootPosition.position;
                        Vector3 shootVector = Vector3.forward * shootForce;
                        if (s == 0)
                        {
                            shootVector.x = -.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                        }
                        else if (s == 1)
                        {
                            shootVector.x = +.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, +3, 0);
                        }
                        bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                    }
                }
                else if (shootAmount == 3)
                {
                    for (int s = 0; s < 3; s++)
                    {
                        GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                        bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        for (int i = 0; i < muzzleParticles.Count; i++)
                        {
                            muzzleParticles[i].Play();
                        }
                        bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, _bulletsToUser);
                        bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                        bullet.transform.position = shootPosition.position;
                        Vector3 shootVector = Vector3.forward * shootForce;
                        if (s == 0)
                        {
                            shootVector.x = -.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                        }
                        else if (s == 1)
                        {
                            shootVector.x = +.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, +3, 0);
                        }
                        else if (s == 3)
                        {
                            bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        }
                        bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                    }
                }
                else if (shootAmount == 4)
                {
                    for (int s = 0; s < 4; s++)
                    {
                        GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                        bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        for (int i = 0; i < muzzleParticles.Count; i++)
                        {
                            muzzleParticles[i].Play();
                        }
                        bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, _bulletsToUser);
                        bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                        bullet.transform.position = shootPosition.position;
                        Vector3 shootVector = Vector3.forward * shootForce;
                        if (s == 0)
                        {
                            shootVector.x = -.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                        }
                        else if (s == 1)
                        {
                            shootVector.x = +.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, +3, 0);
                        }
                        else if (s == 2)
                        {
                            shootVector.x = +.1f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, +6, 0);
                        }
                        else if (s == 4)
                        {
                            shootVector.x = -.1f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, -6, 0);
                        }
                        bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                    }
                }
                else
                {
                    for (int s = 0; s < 5; s++)
                    {
                        GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                        bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        for (int i = 0; i < muzzleParticles.Count; i++)
                        {
                            muzzleParticles[i].Play();
                        }
                        bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, _bulletsToUser);
                        bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                        bullet.transform.position = shootPosition.position;
                        Vector3 shootVector = Vector3.forward * shootForce;
                        if (s == 0)
                        {
                            shootVector.x = -.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                        }
                        else if (s == 1)
                        {
                            shootVector.x = +.05f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, +3, 0);
                        }
                        else if (s == 2)
                        {
                            shootVector.x = +.1f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, +6, 0);
                        }
                        else if (s == 4)
                        {
                            shootVector.x = -.1f * shootForce;
                            bullet.transform.eulerAngles = new Vector3(0, -6, 0);
                        }
                        else if (s == 5)
                        {
                            bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                        }
                        bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                    }
                }

            }

        }
    }
}
