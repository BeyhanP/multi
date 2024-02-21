using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class FinalObject : MonoBehaviour
{
    public float power;
    [SerializeField]private GameObject varilInside;
    private TextMeshPro powerText;
    [SerializeField]private GameObject moneyObject;
    private bool shaking;
    Vector3 startScale;
    [SerializeField]private Transform moneyFallPosition;
    private Vector3 moneyStartPos;
    public int zNum;
    public bool inGameVaril;
    private void Awake()
    {
        moneyObject.GetComponent<Collider>().enabled = false;
        powerText = GetComponentInChildren<TextMeshPro>();
        startScale = varilInside.transform.localScale;
        moneyStartPos = moneyObject.transform.position;
        if (inGameVaril)
        {
            SetPower();
        }
    }
    private IEnumerator ShakeVaril()
    {
        varilInside.transform.DOScaleX(startScale.x * 1.1f,.2f);
        yield return new WaitForSeconds(.2f+Time.deltaTime);
        varilInside.transform.DOScaleX(startScale.x, .1f);
        yield return new WaitForSeconds(.1f + Time.deltaTime);
        shaking = false;
    }
    public void SetPower()
    {
        powerText.text = power.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Taptic.Light();
            other.transform.DOKill();
            power -= other.GetComponent<BulletScript>().bulletPower;
            powerText.text = power.ToString("0");
            if (!shaking)
            {
                StartCoroutine(ShakeVaril());
                shaking = true;
            }
            /*
            GameObject varilParticle = ObjectPooler.instance.SpawnFromPool("BulletHitEffect", other.transform.position, Quaternion.identity);
            foreach (ParticleSystem ps in varilParticle.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
            */
            if (power <= 0)
            {
                powerText.text = "0";
                GetComponent<Collider>().enabled = false;
                transform.DOScale(Vector3.zero,.2f);
                moneyObject.transform.DOKill();
                moneyObject.GetComponent<Collider>().enabled = true;
                moneyObject.transform.parent = null;
                moneyFallPosition.transform.parent = null;
                moneyObject.transform.DOJump(moneyFallPosition.position+new Vector3(0,0,2.5f), 1.3f, 1, .5f);
                moneyObject.transform.DORotate(moneyObject.transform.eulerAngles+ new Vector3(0, 180, 0), .2f);
                moneyObject.transform.DOScale(moneyObject.transform.localScale * 1.1f, .2f).OnComplete(delegate {
                    moneyObject.transform.DOScale(moneyObject.transform.localScale / 1.1f, .2f);
                });
                /*
                GameObject varilBreakParticle = ObjectPooler.instance.SpawnFromPool("VarilBreakParticle", transform.position, Quaternion.identity);
                
                foreach(ParticleSystem ps in varilBreakParticle.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
                */
            }
            else
            {
                moneyObject.transform.DOJump(moneyStartPos, .2f, 1, .2f);
                
            }
        }else if (other.CompareTag("Player"))
        {
            FinishManager.instance.CheckHighScore(zNum);
            if (!inGameVaril)
            {
                Taptic.Success();
                other.enabled = false;
                GetComponent<Collider>().enabled = false;
                GameManager.instance.WinGame();
            }
            else
            {
                if (!PlayerMovement.instance.jumping)
                {
                    Taptic.Failure();
                    StartCoroutine(PlayerMovement.instance.Jump());
                    PlayerMovement.instance.jumping = true;
                }
            }
        }
    }
}
