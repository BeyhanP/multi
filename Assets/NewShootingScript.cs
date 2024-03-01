using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
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



    int partCount;
    [SerializeField] List<RevolverParts> _partsInside = new List<RevolverParts>();
    [SerializeField] List<RevolverParts> _unlockedParts = new List<RevolverParts>();


    public Skills skillToPut;
    List<int> fullData = new List<int>();

    [SerializeField] private List<GameObject> _weaponsInside = new List<GameObject>();


    [Header("ShootPart")]
    [SerializeField] float currentRange;
    public float shootForce;
    [SerializeField] Transform shootPosition;
    private bool spreadShotUnlocked;
    bool canShoot;
    [SerializeField] List<ParticleSystem> muzzleParticles = new List<ParticleSystem>();
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
            PowerBulletAdd(PlayerPrefs.GetInt("StartPower", 1));
            fullData[i] = 1;
        }
        PlaceBases();
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
            GetSkillBullet(skillToPut);
        }
    }
    public void GetSkillBullet(Skills _bulletSkill)
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
                basesInside[0].GetComponent<BulletCase>().AddSkillBullet(_bulletSkill);
            }
            else if (smallestEmptyPosition >= 5 && smallestEmptyPosition < 10)
            {
                basesInside[1].GetComponent<BulletCase>().AddSkillBullet(_bulletSkill);
            }
            else if (smallestEmptyPosition >= 10)
            {
                basesInside[1].GetComponent<BulletCase>().AddSkillBullet(_bulletSkill);
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
                basesInside[1].GetComponent<BulletCase>().AddPowerBullet(newPower);
            }
        }
        else
        {
            if (SmallestPoweredBullet() < newPower)
            {
                SmallestBullet().SetBullet(newPower, Skills.BiggerBullets);
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
        Shoot(GetActivePower(), GetActiveSkills());
        yield return new WaitForSeconds(.5f / fireRate);
        StartCoroutine(RotateToNext());
    }
    public List<Skills> GetActiveSkills()
    {
        List<Skills> skillsToGive = new List<Skills>();
        switch (openBaseCount)
        {
            case 0:
                break;
            case 2:
                //
                List<GameObject> _secondBullets = new List<GameObject>();
                for (int i = 0; i < basesInside[1].GetComponent<BulletCase>().revolverParts.Count; i++)
                {
                    _secondBullets.Add(basesInside[1].GetComponent<BulletCase>().revolverParts[i].gameObject);
                }
                Debug.Log(_secondBullets.Count + "SecondBulletCounter");
                _secondBullets = _secondBullets.OrderBy(bullet => bullet.transform.position.x).ToList();
                if (_secondBullets.Count > 0)
                {
                    if (_secondBullets[0].activeInHierarchy)
                    {
                        if (_secondBullets[0].GetComponent<RevolverParts>()._bulletInside != null)
                        {
                            if (_secondBullets[0].GetComponent<RevolverParts>()._bulletInside.GetComponent<ShowBullet>().skillBullet)
                            {
                                skillsToGive.Add(_secondBullets[0].GetComponent<RevolverParts>()._bulletInside.GetComponent<ShowBullet>().skill);
                            }
                        }
                    }
                }
                break;
            case 3:


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
                        if (_secondBulleters[0].GetComponent<ShowBullet>().skillBullet)
                        {
                            skillsToGive.Add(_secondBulleters[0].GetComponent<ShowBullet>().skill);
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
                        if (_thirdBulleters[iNumber].GetComponent<ShowBullet>().skillBullet)
                        {
                            skillsToGive.Add(_thirdBulleters[iNumber].GetComponent<ShowBullet>().skill);
                        }
                    }
                }

                break;
        }
        return skillsToGive;
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
    public float GetFirstBasePower()
    {
        int partCount = openBaseCount;
        float totalPower = 0;
        List<RevolverParts> _parts = new List<RevolverParts>();
        for (int i = 0; i < basesInside[0].GetComponent<BulletCase>().revolverParts.Count; i++)
        {
            _parts.Add(basesInside[0].GetComponent<BulletCase>().revolverParts[i].GetComponent<RevolverParts>());
        }
        if(partCount == 1)
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
    public void Shoot(float shootPower, List<Skills> _skillsInside)
    {
        if (canShoot && GameManager.instance.started)
        {
            GetComponent<Animator>().Play("ShootAnimation");
            if (!spreadShotUnlocked)
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
