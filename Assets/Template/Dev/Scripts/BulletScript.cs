using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class BulletScript : MonoBehaviour
{
    public float zMax;
    public float bulletPower;
    public bool ShowBullet;
    public bool throwerBulleter;
    public List<Material> _bulletTipMaterials;
    public bool decisionGateBullet;
    private void Awake()
    {
        SetBullet();
    }
    public void SetBullet()
    {
        if (bulletPower % 1 == 0)
        {
            GetComponentInChildren<TMPro.TextMeshPro>().text = bulletPower.ToString("0");
        }
        else
        {
            GetComponentInChildren<TMPro.TextMeshPro>().text = bulletPower.ToString("0.0");
        }
        GetComponentInChildren<TMPro.TextMeshPro>().color = Color.green;
        GetComponentInChildren<TMPro.TextMeshPro>().DOColor(Color.white, .1f).SetDelay(.2f);
    }
    public void BulletDeActivate(bool showPower, bool particledHit = false)
    {
        if (particledHit)
        {
            GameObject hitParticle = ObjectPooler.instance.SpawnFromPool("BulletHit", transform.position, Quaternion.identity);
            foreach (ParticleSystem ps in hitParticle.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }
        if (showPower)
        {
            GameObject hitPowerTexter = ObjectPooler.instance.SpawnFromPool("HitPowerTexter", transform.position - new Vector3(0, 0, 1), Quaternion.identity);
            hitPowerTexter.transform.DOKill();
            hitPowerTexter.transform.DOComplete();
            Vector3 startPosition = transform.position - new Vector3(Random.Range(-.2f, .2f), 0, 1);
            hitPowerTexter.transform.position = startPosition;
            hitPowerTexter.transform.DOMove(startPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(1, 1.5f), 0), .3f);
            if (bulletPower % 1 == 0)
            {
                hitPowerTexter.GetComponent<TextMeshPro>().text = bulletPower.ToString("0");
            }
            else
            {
                hitPowerTexter.GetComponent<TextMeshPro>().text = bulletPower.ToString("0.0");
            }
            hitPowerTexter.GetComponent<TextMeshPro>().color = Color.white;
            hitPowerTexter.GetComponent<TextMeshPro>().DOColor(new Color(), .2f).SetDelay(.5f);
        }
        transform.DOComplete();
        transform.DOKill();
        transform.DOScale(Vector3.zero, .1f);
        GetComponent<Collider>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;
        GetComponent<TrailRenderer>().Clear();
    }
    public void ActivateBullet(float _power)
    {
        bulletPower = _power;
        transform.DOComplete();
        transform.DOKill();
        GetComponent<TrailRenderer>().Clear();
        GetComponent<BulletScript>().SetBullet();
        transform.DOKill();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        transform.localScale = Vector3.one * .2f;
    }
    private void Update()
    {
        if (!throwerBulleter)
        {
            if (!ShowBullet)
            {
                if (transform.position.z > zMax)
                {
                    if (GetComponent<Collider>())
                    {
                        GetComponent<Collider>().enabled = false;
                        transform.DOScale(Vector3.zero, .2f);
                        GetComponent<TrailRenderer>().enabled = false;
                    }
                }
            }
            else
            {
                transform.eulerAngles = new Vector3(-90, 0, 90);
            }
        }
    }
}
