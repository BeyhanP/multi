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
            //BulletAdder(PlayerPrefs.GetInt("StartPower"));
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
    private void Start()
    {
        for (int i = 0; i < basesInside.Count; i++)
        {
            Debug.Log("iiii" + i.ToString());
            for(int b = 0; b < basesInside[i].bulletsInside.Count; b++)
            {
                basesInside[i].bulletsInside[b].GetComponent<ShowBullet>().SetBullet((i * 5)+b,Skills.BiggerBullets,0);
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
            case 0:
                for (int i = 0; i < basesInside[0].revolverParts.Count; i++)
                {
                    if (basesInside[0].revolverParts[i].transform.localPosition.z == 0.001866712f)
                    {
                        basesInside[0].revolverParts[i].GetComponent<RevolverParts>()._bulletInside._powerText.color = Color.green;
                        basesInside[0].revolverParts[i].GetComponent<RevolverParts>()._bulletInside._powerText.DOColor(Color.white, .1f).SetDelay(.1f);
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
                //bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside);
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
                    //bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside);

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
