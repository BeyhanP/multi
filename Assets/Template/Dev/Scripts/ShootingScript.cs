using System.Collections.Generic;
using System.Collections;
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

    public List<float> anglesToRotate = new List<float>();
    [SerializeField] List<BulletCase> basesInside = new List<BulletCase>();
    int currentAngleNumber;
    public float angleAddAmount;
    float currentAngle;
    [SerializeField] AnimationCurve rotateAnimCurve;
    [SerializeField] Transform bulletDescendPosition;
    [SerializeField] float bulletPunchAmount;
    public List<ParticleSystem> muzzleParticles;
    List<float> currentPowers = new List<float>();
    public static ShootingScript instance;

    List<int> fullData = new List<int>();

    [SerializeField] List<Transform> basePositions = new List<Transform>();
    [SerializeField] private List<Transform> shootPositions = new List<Transform>();


    [SerializeField] Transform caseThrowPosition;
    [SerializeField] GameObject basePrefab;
    [SerializeField] Vector3 caseThrowForce;
    public bool stopShooting;
    public bool SpreadShotUnlocked;
    public GameObject upgradeParticle;



    [SerializeField] AnimationCurve partCurve;
    int partNumber;
    public void SetMiniGun()
    {
        int baseCount = basesInside.Count;
        muzzleParticles[0].transform.localPosition = shootPosition.localPosition;
    }
    private void Awake()
    {
        instance = this;


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
        for (int i = 0; i < 5; i++)
        {
            anglesToRotate.Add(i * singleAngle);
        }
        anglesToRotate.RemoveAt(0);
        anglesToRotate.Add(360);
        angleAddAmount = singleAngle;
        //SetMiniGun();
        List<float> powerList = new List<float>();
        for (int i = 0; i < 6; i++)
        {
            powerList.Add(1);
        }
        StartCoroutine(GetBase(powerList, false, new List<BulletCase>()));
    }
    private void Start()
    {

        for (int i = 0; i < basesInside.Count; i++)
        {
            foreach (GameObject _bullet in basesInside[i].bulletsInside)
            {
                //_bullet.GetComponent<BulletScript>().bulletPower = PlayerPrefs.GetFloat("StartPower");
                _bullet.GetComponent<ShowBullet>().SetBullet(PlayerPrefs.GetFloat("StartPower"));
            }
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
                bulletToShow.GetComponent<BulletScript>().throwerBulleter = true;
                Destroy(bulletToShow.GetComponent<BulletScript>());
                bulletToShow.transform.position = ourBullet.transform.position;
                bulletToShow.transform.localScale = ourBullet.transform.localScale;
                bulletToShow.transform.localEulerAngles = ourBullet.transform.localEulerAngles;
                bulletToShow.AddComponent<Rigidbody>().AddForce(new Vector3(0, -1, 2) * -200f);
                bulletToShow.GetComponent<Rigidbody>().AddTorque(new Vector3(1, -1, 2) * -200f);
                bulletToShow.AddComponent<BoxCollider>();
                ourBullet.GetComponent<BulletScript>().throwerBulleter = true;
                ourBullet.transform.parent = _newBullet.transform;
                ourBullet.transform.localScale = _newBullet.transform.GetChild(0).localScale;
                ourBullet.transform.localEulerAngles = _newBullet.transform.GetChild(0).localEulerAngles;
                ourBullet.transform.localPosition = _newBullet.transform.GetChild(0).localPosition;
                _newBullet.transform.GetChild(0).gameObject.SetActive(false);

                ourBullet.GetComponent<BulletScript>().bulletPower = _newBullet.GetComponent<BulletScript>().bulletPower;
                ourBullet.GetComponent<BulletScript>().SetBullet();

                ourBullet.transform.parent = _oldParent;
                ourBullet.transform.DOLocalMove(oldPosition, .5f);
                ourBullet.transform.DOLocalRotate(oldEuler, .5f);
                ourBullet.transform.DOScale(oldScale, .5f).OnComplete(delegate {
                    ourBullet.GetComponent<BulletScript>().throwerBulleter = false;
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
    public IEnumerator GetBase(List<float> basePowers, bool fromMiniGame, List<BulletCase> _basesToGet)
    {
        stopShooting = true;


        if (!fromMiniGame)
        {
            if (basesInside.Count < 6)
            {
                GameObject _basePrefab = Instantiate(basePrefab);
                BulletCase _caseScript = _basePrefab.GetComponent<BulletCase>();
                _caseScript.miniGameCase = false;
                for (int i = 0; i < _caseScript.bulletsInside.Count; i++)
                {
                    _caseScript.bulletsInside[i].GetComponent<ShowBullet>().SetBullet(basePowers[i]);
                }
                basesInside.Add(_caseScript);
                Transform refPos = basePositions[basesInside.Count - 1];
                _caseScript.transform.parent = refPos.parent;
                _caseScript.transform.localScale = refPos.transform.localScale;
                _caseScript.transform.localPosition = refPos.transform.localPosition;
                _caseScript.transform.localEulerAngles = refPos.transform.localEulerAngles;
                int multipilier = 0;
                if (basesInside.Count % 2 == 0)
                {
                    multipilier = 1;
                }
                else
                {
                    multipilier = -1;
                }
                _caseScript.transform.localEulerAngles = refPos.transform.localEulerAngles;

                SetMiniGun();
            }
            yield return new WaitForSeconds(.1f);
        }
        else
        {
            Debug.Log(_basesToGet.Count + "GetCounter" + basesInside.Count + "BaseInsideCounter");
            if (basesInside.Count + _basesToGet.Count <= 6)
            {
                Debug.Log("Smaller");
                Taptic.Medium();
                for (int i = 0; i < _basesToGet.Count; i++)
                {
                    BulletCase _baseToGet = _basesToGet[i];
                    _baseToGet.transform.DOComplete();
                    _baseToGet.transform.DOKill();
                    _baseToGet.GetComponent<BulletCase>().miniGameCase = false;
                    _baseToGet.transform.parent = basePositions[basesInside.Count].transform.parent;
                    _baseToGet.transform.DOScale(basePositions[basesInside.Count].transform.localScale, .5f);
                    _baseToGet.transform.DOLocalRotate(basePositions[basesInside.Count].transform.localEulerAngles, .5f);
                    _baseToGet.transform.DOLocalMove(basePositions[basesInside.Count].transform.localPosition, .5f);
                    basesInside.Add(_baseToGet);
                    SetMiniGun();
                }
                yield return new WaitForSeconds(Time.deltaTime + .5f);
                stopShooting = false;
            }
            else
            {
                Debug.Log("Bigger");
                float singleNumber = 0;
                float totalNumber = 0;
                for (int i = 0; i < basesInside.Count; i++)
                {
                    foreach (GameObject _bullet in basesInside[i].bulletsInside)
                    {
                        totalNumber += _bullet.GetComponent<BulletScript>().bulletPower;
                    }
                }
                for (int i = 0; i < _basesToGet.Count; i++)
                {
                    foreach (GameObject _bullet in _basesToGet[i].bulletsInside)
                    {
                        totalNumber += _bullet.GetComponent<BulletScript>().bulletPower;
                    }
                }
                singleNumber = (int)(totalNumber / 6f);
                for (int i = 0; i < basesInside.Count; i++)
                {
                    //basesInside[i].transform.DOScale(Vector3.zero, .2f);
                }

                List<float> newBasePowers = new List<float>();
                for (int i = 0; i < 6; i++)
                {
                    newBasePowers.Add(singleNumber);
                }
                for (int i = 0; i < _basesToGet.Count; i++)
                {
                    _basesToGet[i].transform.DOScale(Vector3.zero, .2f);
                }
                for (int i = 0; i < basesInside.Count; i++)
                {
                    basesInside[i].transform.DOLocalMove(basePositions[0].localPosition, .2f);
                    basesInside[i].transform.DOScale(Vector3.zero, .05f).SetDelay(.15f);
                    yield return new WaitForSeconds(.05f);
                }
                foreach (ParticleSystem ps in upgradeParticle.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
                basesInside.Clear();
                StartCoroutine(GetBase(newBasePowers, false, null));
            }
        }
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
                _bullet.GetComponent<BulletScript>().SetBullet();
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
        if (stopShooting)
        {
            yield return null;
        }
        currentAngleNumber++;
        currentAngle += angleAddAmount;
        if (currentAngleNumber >= 6)
        {
            currentAngleNumber = 0;
        }
        int currentBulletNumber = currentAngleNumber;
        yield return new WaitForSeconds(.8f / fireRate);
        currentPowers.Clear();
        for (int i = 0; i < basesInside.Count; i++)
        {
            int bulletNumber = 0;
            if (i % 2 == 0)
            {
                bulletNumber = currentBulletNumber;
                Debug.Log("PlusNumber" + bulletNumber);
            }
            else
            {
                bulletNumber = 5 - currentBulletNumber + 1;
                if (bulletNumber == 6)
                {
                    bulletNumber = 0;
                }
                Debug.Log("MinusNumber" + bulletNumber);
            }
            int multiplier = 0;
            if (i % 2 == 0)
            {
                multiplier = 1;
            }
            else
            {
                multiplier = -1;
            }
            basesInside[i].transform.DOLocalRotate(new Vector3(angleAddAmount * multiplier, 0, 0), .8f / fireRate, RotateMode.LocalAxisAdd);
            Debug.Log(.8f / fireRate);
            /*
            GameObject _bullet = basesInside[i].GetBullet(bulletNumber);
            basesInside[i].GetBullet(bulletNumber).transform.DOPunchScale(Vector3.one * bulletPunchAmount, .2f, 10, 1);
            _bullet.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.red;
            _bullet.GetComponentInChildren<TMPro.TextMeshPro>().DOColor(Color.white, .1f).SetDelay(.1f);
            */
            currentPowers.Add(1);
        }
        for (int i = 0; i < currentPowers.Count; i++)
        {
            Transform _baseToMove = basesInside[i].transform;
            Transform _currentReference = basePositions[i];
            //_baseToMove.transform.DOComplete();
            //_baseToMove.transform.DOKill();
            //_baseToMove.transform.DOLocalMoveX(_currentReference.transform.localPosition.x + .004f, .1f);
            //_baseToMove.transform.DOLocalMoveX(_currentReference.transform.localPosition.x, .1f).SetDelay(.1f);
            
            Shoot(currentPowers[i]);
            Taptic.Light();
            yield return new WaitForSeconds(.15f / fireRate);
        }
        
        ShootingScript.instance.gameObject.GetComponent<Animator>().SetFloat("FireRate", fireRate);
        yield return new WaitForSeconds(.2f / fireRate);
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
            List<float> powerList = new List<float>();
            for (int i = 0; i < 6; i++)
            {
                powerList.Add(1);
            }
            StartCoroutine(GetBase(powerList, false, new List<BulletCase>()));
            //StartCoroutine(GetBase(powerList, false),new BulletCase());
        }
    }
    public void Shoot(float shootPower)
    {
        if (canShoot && GameManager.instance.started)
        {
            GetComponent<Animator>().Play("ShootAnimation");
            if (!SpreadShotUnlocked)
            {
                
                Taptic.Light();
                GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                /*
//                GameObject _case = ObjectPooler.instance.SpawnFromPool("ThrowCase", caseThrowPosition.position, Quaternion.identity);
                _case.transform.eulerAngles = caseThrowPosition.eulerAngles;
                _case.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _case.GetComponent<Rigidbody>().isKinematic = true;
                _case.GetComponent<Rigidbody>().isKinematic = false;
                _case.GetComponent<Rigidbody>().AddForce(caseThrowForce);
                _case.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, 1) * caseThrowForce.x);
                */
                bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                bullet.GetComponent<BulletScript>().ShowBullet = false;

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
                bullet.GetComponent<BulletScript>().ActivateBullet(shootPower);
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
                    bullet.GetComponent<BulletScript>().ShowBullet = false;
                    if (i == 0)
                    {
                        GameObject _case = ObjectPooler.instance.SpawnFromPool("ThrowCase", caseThrowPosition.position, Quaternion.identity);
                        _case.transform.eulerAngles = caseThrowPosition.eulerAngles;
                        _case.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        _case.GetComponent<Rigidbody>().isKinematic = true;
                        _case.GetComponent<Rigidbody>().isKinematic = false;
                        _case.GetComponent<Rigidbody>().AddForce(caseThrowForce);
                        _case.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, 1) * caseThrowForce.x);
                    }

                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    /*
                    power = 0;
                    for(int i = 0; i < currentPowers.Count; i++)
                    {
                        power += currentPowers[i];
                    }
                    */
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower);

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
