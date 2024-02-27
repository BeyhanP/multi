using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using ElephantSDK;
public class ShootingScript : MonoBehaviour
{

    [Header("Shooting Parameters")]
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float shootForce;
    private float counter;
    private float shootInterval;

    public float currentRange;
    public float fireRate;

    public float power;
    public bool canShoot;
    public float bulletScale = 0.2f;

    [Header("IncreaseAmounts")]
    [SerializeField] private float rateIncreaseAmount;
    [SerializeField] private float rangeIncreaseAmount;
    [SerializeField] private float powerIncreaseAmount;
    [SerializeField] List<BulletCase> basesInside = new List<BulletCase>();
    public float angleAddAmount;
    [SerializeField] AnimationCurve rotateAnimCurve;
    public List<ParticleSystem> muzzleParticles;
    public static ShootingScript instance;
    [SerializeField] List<Transform> basePositions = new List<Transform>();
    [SerializeField] List<Transform> rotatePositions = new List<Transform>();
    [SerializeField] GameObject basePrefab;
    public bool stopShooting;
    public bool SpreadShotUnlocked;
    public GameObject upgradeParticle;


    public List<Transform> _singlePosition;
    public List<Transform> _doublePosition;
    public List<Transform> _triplePosition;


    public List<ShowBullet> _showBulletsInside = new List<ShowBullet>();

    public List<RevolverParts> _partsInside;

    public GameObject bulletprefab;
    public int partCount;
    public int openBaseCount;



    [SerializeField] List<int> fullData = new List<int>();
    private void Awake()
    {
        instance = this;

       for(int i = 0; i < 15; i++)
        {
            fullData.Add(0);
        }
        partCount = 5;
        for(int i = 0; i < partCount; i++)
        {
            BulletAdder(PlayerPrefs.GetInt("StartPower"));
        }
        for(int i = 0; i < partCount; i++)
        {
            fullData[i] = 1;
        }
        fireRate = RemoteConfig.GetInstance().GetFloat("StartFireRate", 1) + PlayerPrefs.GetInt("RateUpgradedAmount") * RemoteConfig.GetInstance().GetFloat("RateUpgradeIncAmount", .01f);
        currentRange = RemoteConfig.GetInstance().GetFloat("StartFireRange", 20) + PlayerPrefs.GetInt("RangeUpgradedAmount") * RemoteConfig.GetInstance().GetFloat("RangeUpgradeIncAmount", .25f);
        rateIncreaseAmount = RemoteConfig.GetInstance().GetFloat("RateIncAmount", .02f);
        rangeIncreaseAmount = RemoteConfig.GetInstance().GetFloat("RateIncAmount", .02f);
        if (PlayerPrefs.GetFloat("StartPower") == 0)
        {
            PlayerPrefs.SetFloat("StartPower", 1);
        }
        if (RemoteConfig.GetInstance().GetInt("EffectAmountByPower", 1) == 0)
        {

        }
        else
        {
            rateIncreaseAmount *= 1 / (PlayerPrefs.GetFloat("StartPower") * RemoteConfig.GetInstance().GetFloat("RateIncDecreaseAmount", 1));
        }
        canShoot = true;
        stopShooting = true;
        shootInterval = 1 / fireRate;
        counter = shootInterval;
        float singleAngle = 360f / 5;
        angleAddAmount = singleAngle;
        //SetMiniGun();
        List<float> powerList = new List<float>();
        for (int i = 0; i < 6; i++)
        {
            powerList.Add(1);
        }
    }
    public void BulletAdder(float power)
    {

    }
    private void Start()
    {
        PlaceBases();
        for (int i = 0; i < basesInside.Count; i++)
        {
            Debug.Log("iiii" + i.ToString());
            for(int b = 0; b < basesInside[i].bulletsInside.Count; b++)
            {
                basesInside[i].bulletsInside[b].GetComponent<ShowBullet>().SetBullet((i * 5)+b,Skills.BiggerBullets);
                Debug.Log(b * (i + 1) + "Power" + i.ToString()+"i");
            }
            foreach (GameObject _bullet in basesInside[i].bulletsInside)
            {
                //_bullet.GetComponent<BulletScript>().bulletPower = PlayerPrefs.GetFloat("StartPower");
                int b = 0;
                b++;
                
            }
        }
        for (int i = 0; i < _showBulletsInside.Count; i++)
        {
            //_showBulletsInside[i]._power = i;
            //_showBulletsInside[i]._powerText.text = i.ToString();
        }
    }
    public void GameStarted()
    {
        StartCoroutine(RotateToNext());
    }
    public GameObject SmallestEarliestBullet(float changePower)
    {
        List<GameObject> allBulletsInside = new List<GameObject>();
        for (int i = 0; i < basesInside.Count; i++)
        {
            for (int b = 0; b < basesInside[i].bulletsInside.Count; b++)
            {
                allBulletsInside.Add(basesInside[i].bulletsInside[b]);
            }
        }
        float smallestPower = changePower;
        int smallestI = 0;
        bool found = false;
        for (int i = 0; i < allBulletsInside.Count; i++)
        {
            if (allBulletsInside[i].GetComponent<BulletScript>().bulletPower < smallestPower)
            {
                smallestPower = allBulletsInside[i].GetComponent<BulletScript>().bulletPower;
                smallestI = i;
                found = true;
            }
        }
        if (found)
        {
            return allBulletsInside[smallestI];
        }
        else
        {
            return null;
        }
    }
    public IEnumerator GetSkillBullet(Transform bulletStartPosition, Skills _skill)
    {
        Debug.Log("SkillEnumEntered");
        bool gotEmptySpot = false;
        int smallestEmptySpot = 0;
        for(int i = 0; i < fullData.Count; i++)
        {
            if (fullData[i] == 0)
            {
                if (!gotEmptySpot)
                {
                    gotEmptySpot = true;
                    smallestEmptySpot = i;
                }
            }
        }
        Debug.Log("EmptySpot" + smallestEmptySpot);
        if (gotEmptySpot)
        {
            if (smallestEmptySpot < partCount)
            {
                fullData[smallestEmptySpot] = 1;
                GameObject newBullet = Instantiate(bulletprefab);
                Transform _oldBullet = _showBulletsInside[smallestEmptySpot].transform;
                Transform _parent = _oldBullet.transform.parent;
                Vector3 oldLocalPosition = _oldBullet.transform.localPosition;
                Vector3 oldLocalEuler = _oldBullet.transform.localEulerAngles;
                Vector3 oldLocalScale = _oldBullet.transform.localScale;
                _oldBullet.transform.parent = null;
                newBullet.transform.position = bulletStartPosition.transform.position;
                newBullet.transform.eulerAngles = bulletStartPosition.transform.eulerAngles;
                newBullet.transform.localScale = bulletStartPosition.transform.localScale;
                newBullet.GetComponent<ShowBullet>().SetBullet(0, _skill, true);
                yield return null;

            }
        }
        else
        {
        }
    }
    public IEnumerator GetBullets(List<GameObject> bulletsToGet)
    {
        /*
        stopShooting = true;
        yield return new WaitForSeconds(.6f);
        */
        for (int i = 0; i < bulletsToGet.Count; i++)
        {
            if (SmallestEarliestBullet(bulletsToGet[i].GetComponent<BulletScript>().bulletPower) != null)
            {
                GameObject ourBullet = SmallestEarliestBullet(bulletsToGet[i].GetComponent<BulletScript>().bulletPower);

                GameObject _newBullet = bulletsToGet[i];
                bulletsToGet[i].transform.DOComplete();
                bulletsToGet[i].transform.DOKill();

                Transform _oldParent = ourBullet.transform.parent;
                Vector3 oldScale = ourBullet.transform.localScale;
                Vector3 oldPosition = ourBullet.transform.localPosition;
                Vector3 oldEuler = ourBullet.transform.localEulerAngles;

                GameObject bulletToShow = Instantiate(ourBullet);
                Destroy(bulletToShow.GetComponent<BulletScript>());
                bulletToShow.transform.position = ourBullet.transform.position;
                bulletToShow.transform.localScale = ourBullet.transform.localScale;
                bulletToShow.transform.localEulerAngles = ourBullet.transform.localEulerAngles;
                bulletToShow.AddComponent<Rigidbody>().AddForce(new Vector3(0, -1, 2) * -200f);
                bulletToShow.GetComponent<Rigidbody>().AddTorque(new Vector3(1, -1, 2) * -200f);
                bulletToShow.AddComponent<BoxCollider>();
                ourBullet.transform.parent = _newBullet.transform;
                ourBullet.transform.localScale = _newBullet.transform.GetChild(0).localScale;
                ourBullet.transform.localEulerAngles = _newBullet.transform.GetChild(0).localEulerAngles;
                ourBullet.transform.localPosition = _newBullet.transform.GetChild(0).localPosition;
                _newBullet.transform.GetChild(0).gameObject.SetActive(false);

                ourBullet.GetComponent<BulletScript>().bulletPower = _newBullet.GetComponent<BulletScript>().bulletPower;
                ourBullet.GetComponent<BulletScript>().SetBullet(new List<Skills>());

                ourBullet.transform.parent = _oldParent;
                ourBullet.transform.DOLocalMove(oldPosition, .5f);
                ourBullet.transform.DOLocalRotate(oldEuler, .5f);
                ourBullet.transform.DOScale(oldScale, .5f).OnComplete(delegate {
                });
            }
            else
            {
                bulletsToGet[i].transform.DOComplete();
                bulletsToGet[i].transform.DOKill();
                bulletsToGet[i].GetComponent<Rigidbody>().useGravity = true;
                bulletsToGet[i].GetComponent<Collider>().enabled = true;
                bulletsToGet[i].GetComponent<Collider>().isTrigger = false;
            }
        }
        for (int i = 0; i < bulletsToGet.Count; i++)
        {
            Taptic.Light();
            yield return new WaitForSeconds(.05f);
        }
        yield return new WaitForSeconds(.1f);
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
    public void AddPart()
    {
        partCount++;
        for(int i = 0; i < _partsInside.Count; i++)
        {
            if (i < partCount)
            {
                if (!_partsInside[i].gameObject.activeInHierarchy)
                {
                    _partsInside[i].gameObject.SetActive(true);
                    Vector3 scale = _partsInside[i].gameObject.transform.localScale;
                    _partsInside[i].gameObject.transform.localScale = Vector3.zero;
                    _partsInside[i].gameObject.transform.DOScale(scale, .2f).SetEase(rotateAnimCurve);
                    //_partsInside[i].SetBulletSize();
                }
            }
            else
            {
                
                _partsInside[i].gameObject.SetActive(false);
            }
        }
        PlaceBases();
    }
    public void FireRateUpgrade(float amount)
    {
        fireRate += amount * rateIncreaseAmount;
        if (fireRate < .5f)
        {
            fireRate = .5f;
        }
        FireRateChanged();
    }
    public void FireRangeUpgrade(float amount)
    {
        currentRange += amount * rangeIncreaseAmount;
        if (currentRange < 2f)
        {
            currentRange = 2;
        }
    }
    public void BulletPowerUpgradeFromUI()
    {
        PlayUpgradeParticle();
        PlayerPrefs.SetFloat("StartPower", PlayerPrefs.GetFloat("StartPower") + RemoteConfig.GetInstance().GetFloat("PoweUpgradeAmountFromUI", .5f));
        for (int i = 0; i < basesInside.Count; i++)
        {
            foreach (GameObject _bullet in basesInside[i].bulletsInside)
            {
                _bullet.GetComponent<BulletScript>().bulletPower = PlayerPrefs.GetFloat("StartPower");
                _bullet.GetComponent<BulletScript>().SetBullet(new List<Skills>());
            }
        }
    }
    public void FireRangeUpgradeFromUI()
    {
        foreach (ParticleSystem ps in upgradeParticle.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
        //currentRange = RemoteConfig.GetInstance().GetFloat("StartFireRange", 1) + PlayerPrefs.GetInt("RangeUpgradedAmount") * RemoteConfig.GetInstance().GetFloat("RangeUpgradeIncAmount", .25f);
        currentRange += RemoteConfig.GetInstance().GetFloat("RangeUpgradeIncAmount", .25f);
        PlayerPrefs.SetInt("RangeUpgradedAmount", PlayerPrefs.GetInt("RangeUpgradedAmount") + 1);
        if (currentRange < 2f)
        {
            currentRange = 2;
        }
    }
    public void FirePowerUpgrade(float amount)
    {
        power += amount * powerIncreaseAmount;
        if (power < .5f)
        {
            power = .5f;
        }
    }
    public void FireRateUpgradeFromUI()
    {
        foreach (ParticleSystem ps in upgradeParticle.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
        fireRate += RemoteConfig.GetInstance().GetFloat("RateUpgradeIncAmount", .02f);
        PlayerPrefs.SetInt("RateUpgradedAmount", PlayerPrefs.GetInt("RateUpgradedAmount") + 1);
        FireRateChanged();
    }
    public void PlayUpgradeParticle()
    {
        foreach (ParticleSystem ps in upgradeParticle.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }
    public void FirePowerUpgradeFromUI()
    {
        foreach (ParticleSystem ps in upgradeParticle.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
        power += .1f;
        PlayerPrefs.SetFloat("FirePower", power);
    }
    private void FireRateChanged()
    {
        shootInterval = 1 / fireRate;
        if (counter > shootInterval)
        {
            counter = shootInterval;
        }
    }
    public IEnumerator RotateToNext()
    {
        for (int i = 0; i < rotatePositions.Count; i++)
        {
            rotatePositions[i].transform.DOLocalRotate(new Vector3(angleAddAmount, 0, 0), .5f / fireRate, RotateMode.LocalAxisAdd).SetEase(rotateAnimCurve);
        }
        yield return new WaitForSeconds(.5f / fireRate);
        List<Skills> skillList = new List<Skills>();
        float totalPower = 0;
        switch (openBaseCount)
        {
            case 1:
                List<GameObject> bullets = new List<GameObject>();
                for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().bulletsInside.Count; i++)
                {
                    bullets.Add(basesInside[0].GetComponent<BulletCase>().bulletsInside[i]);
                }
                bullets = bullets.OrderBy(bullet => bullet.transform.position.y).ToList();
                Debug.Log(bullets[^1].GetComponent<ShowBullet>()._power);
                totalPower += bullets[^1].GetComponent<ShowBullet>()._power;
                break;
            case 2:
                List<GameObject> _firstBullets = new List<GameObject>();
                for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().bulletsInside.Count; i++)
                {
                    _firstBullets.Add(basesInside[0].GetComponent<BulletCase>().bulletsInside[i]);
                }
                _firstBullets = _firstBullets.OrderBy(bullet => bullet.transform.position.x).ToList();

                totalPower += _firstBullets[^1].GetComponent<ShowBullet>()._power;
                //

                List<GameObject> _secondBullets = new List<GameObject>();
                for (int i = 0; i < basesInside[1].GetComponent<BulletCase>().bulletsInside.Count; i++)
                {
                    _secondBullets.Add(basesInside[1].GetComponent<BulletCase>().bulletsInside[i]);
                }
                _secondBullets = _secondBullets.OrderBy(bullet => bullet.transform.position.x).ToList();
                if (_secondBullets.Count > 0)
                {

                    if (_secondBullets[0].activeInHierarchy)
                    {
                        if (!_secondBullets[0].GetComponent<ShowBullet>().skillBullet)
                        {
                            totalPower += _secondBullets[0].GetComponent<ShowBullet>()._power;
                        }
                        else
                        {
                            skillList.Add(_secondBullets[0].GetComponent<ShowBullet>().skill);
                        }
                    }
                }
                break;
            case 3:
                List<GameObject> _bulleters = new List<GameObject>();
                for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().bulletsInside.Count; i++)
                {
                    _bulleters.Add(basesInside[0].GetComponent<BulletCase>().bulletsInside[i]);
                }
                _bulleters = _bulleters.OrderBy(bullet => bullet.transform.position.x).ToList();
                totalPower += _bulleters[^1].GetComponent<ShowBullet>()._power;


                //

                List<GameObject> _secondBulleters = new List<GameObject>();
                for (int i = 0; i < basesInside[1].GetComponent<BulletCase>().bulletsInside.Count; i++)
                {
                    _secondBulleters.Add(basesInside[1].GetComponent<BulletCase>().bulletsInside[i]);
                }
                _secondBulleters = _secondBulleters.OrderBy(bullet => bullet.transform.position.x).ToList();
                if (_secondBulleters.Count > 0)
                {

                    if (_secondBulleters[0].activeInHierarchy)
                    {
                        if (!_secondBulleters[0].GetComponent<ShowBullet>().skillBullet)
                        {
                            totalPower += _secondBulleters[0].GetComponent<ShowBullet>()._power;
                        }
                        else
                        {
                            skillList.Add(_secondBulleters[0].GetComponent<ShowBullet>().skill);
                        }
                    }
                }
                List<GameObject> _thirdBulleters = new List<GameObject>();
                for (int i = 0; i < basesInside[1].GetComponent<BulletCase>().bulletsInside.Count; i++)
                {
                    _thirdBulleters.Add(basesInside[1].GetComponent<BulletCase>().bulletsInside[i]);
                }
                _thirdBulleters = _thirdBulleters.OrderBy(bullet => bullet.transform.position.y).ToList();

                float smallestX = Mathf.Infinity;
                int iNumber = 0;
                if (_thirdBulleters.Count > 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (_thirdBulleters[i].transform.position.x < smallestX)
                        {
                            smallestX = _thirdBulleters[i].transform.position.x;
                            iNumber = i;
                        }
                    }
                }
                else { iNumber = 0; }
                if (_thirdBulleters.Count > 0)
                {
                    if (_thirdBulleters[iNumber].activeInHierarchy)
                    {
                        if (!_thirdBulleters[iNumber].GetComponent<ShowBullet>().skillBullet)
                        {
                            totalPower += _thirdBulleters[iNumber].GetComponent<ShowBullet>()._power;
                        }
                        else
                        {
                            skillList.Add(_thirdBulleters[iNumber].GetComponent<ShowBullet>().skill);
                        }
                    }
                }

                break;
        }
        Shoot(totalPower, skillList);
        yield return new WaitForSeconds(.5f / fireRate);
        StartCoroutine(RotateToNext());
    }
    private void Update()
    {
        if (GameManager.instance.started)
        {
            if (canShoot)
            {
                counter -= Time.deltaTime;
                if (counter < 0)
                {
                    //Shoot();
                    counter = shootInterval;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddPart();
        }
    }
    public void Shoot(float shootPower,List<Skills> _skillsInside)
    {
        if (canShoot && GameManager.instance.started)
        {
            GetComponent<Animator>().Play("ShootAnimation");
            if (!SpreadShotUnlocked)
            {
                
                Taptic.Light();
                GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                for (int i = 0; i < muzzleParticles.Count; i++)
                {
                    muzzleParticles[i].Play();
                }
                /*
                    muzzleParticles[0].GetComponent<AudioSource>().Play();
                    muzzleParticles[0].GetComponent<AudioSource>().pitch *= 1 + Random.Range(-5 / 100, 5 / 100);
                    power = 0;
                    for(int i = 0; i < currentPowers.Count; i++)
                    {
                        power += currentPowers[i];
                    }
                    */
                bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside);
                bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                bullet.transform.position = shootPosition.position;
                Vector3 shootVector = Vector3.forward * shootForce;
                bullet.GetComponent<Rigidbody>().AddForce(shootVector);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Taptic.Light();
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside);

                    bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                    bullet.transform.position = shootPosition.position;

                    Vector3 shootVector = Vector3.forward * shootForce;
                    if (i == 0)
                    {
                        shootVector.x = -.05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                    }
                    else
                    {
                        shootVector.x = .05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 3, 0);
                    }
                    bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                }
            }
        }
    }
}
