using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;
[System.Serializable]
public class CollectableLayers
{
    public List<GameObject> _cubesInside = new List<GameObject>();
    public Transform _mainParent;
    public float _throwAmountPerPower;
    public float _currentThrowAmount;
    public float _singlePiecePowerAmount;
    public float _layerPower;
}

public class CollectableMain : MonoBehaviour
{
    [SerializeField] List<GameObject> _referenceCubes = new List<GameObject>();
    [SerializeField] List<GameObject> cubesInside = new List<GameObject>();
    [SerializeField] List<CollectableLayers> _collectableLayersInside = new List<CollectableLayers>();

    [SerializeField] bool getLayers;
    int currentLayerNumber =0;
    public float explodeForce;
    bool nonking;



    [SerializeField] TextMeshPro _powerText;
    [SerializeField] float rotShakeAmount;
    private void Awake()
    {
        for(int i = 0; i < _collectableLayersInside.Count; i++)
        {
            float dropAmountPerPower = (float)_collectableLayersInside[i]._cubesInside.Count / (float)_collectableLayersInside[i]._layerPower;
            _collectableLayersInside[i]._throwAmountPerPower = dropAmountPerPower;
        }
        SetPowerTexter();
    }
    public void SetPowerTexter()
    {
        float totalPower = 0;
        for(int i = 0; i < _collectableLayersInside.Count; i++)
        {
            totalPower += _collectableLayersInside[i]._layerPower;
        }
        _powerText.text = totalPower.ToString("0");
    }
    private void Update()
    {
        /*
        if (getLayers)
        {
            GetCubesInside();
            getLayers = false;
        }
        */
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetHit(1);
        }
    }
    public void GetCubesInside()
    {
        _referenceCubes = _referenceCubes.OrderBy(_singleCube => _singleCube.transform.position.y).ToList();
        for(int i = 0; i < _referenceCubes.Count; i++)
        {
            GameObject _layerParent = new GameObject();
            _layerParent.transform.parent = transform;
            _layerParent.name = "LayerParent" +" "+ i.ToString();   
            _collectableLayersInside.Add(new CollectableLayers());
            foreach (GameObject cube in cubesInside)
            {
                if(cube.transform.position.y == _referenceCubes[i].transform.position.y)
                {
                    _collectableLayersInside[i]._cubesInside.Add(cube);
                    cube.transform.parent = _layerParent.transform;
                }
            }
        }
    }
    public void GetHit(float _addAmount)
    {
        List<GameObject> _sendPieceLister = new List<GameObject>();
        _collectableLayersInside[currentLayerNumber]._layerPower -= _addAmount;
        _collectableLayersInside[currentLayerNumber]._currentThrowAmount += _collectableLayersInside[currentLayerNumber]._throwAmountPerPower * _addAmount;
        int startLayerNumberer = currentLayerNumber;
        if (_collectableLayersInside[currentLayerNumber]._layerPower <= 0)
        {
            for (int i = 0; i < _collectableLayersInside[currentLayerNumber]._cubesInside.Count; i++)
            {
                if (_collectableLayersInside[currentLayerNumber]._cubesInside[i].GetComponent<Rigidbody>() == null)
                {
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.DOComplete();
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.DOKill();
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].AddComponent<Rigidbody>().AddExplosionForce(explodeForce, new Vector3(transform.position.x, _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.position.y, transform.position.z), 100);
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].AddComponent<BoxCollider>();
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.parent = null;
                    _sendPieceLister.Add(_collectableLayersInside[currentLayerNumber]._cubesInside[i]);
                }
            }
            currentLayerNumber++;
            if (currentLayerNumber >= _collectableLayersInside.Count)
            {
                transform.DOScale(Vector3.zero, .2f);
            }
        }
        else
        {
            for (int i = 0; i < _collectableLayersInside[currentLayerNumber]._currentThrowAmount; i++)
            {
                if (_collectableLayersInside[currentLayerNumber]._cubesInside[i].GetComponent<Rigidbody>() == null)
                {
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.DOComplete();
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.DOKill();
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].AddComponent<Rigidbody>().AddExplosionForce(explodeForce, new Vector3(transform.position.x, _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.position.y, transform.position.z), 100);
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].AddComponent<BoxCollider>();
                    _collectableLayersInside[currentLayerNumber]._cubesInside[i].transform.parent = null;
                    _sendPieceLister.Add(_collectableLayersInside[currentLayerNumber]._cubesInside[i]);
                }
            }
            if (!nonking)
            {
                StartCoroutine(Noink());
                _powerText.transform.DOPunchScale(Vector3.one * rotShakeAmount, .25f, 10, 10);
                nonking = true;
            }
        }
        StartCoroutine(SendPieces(_sendPieceLister, _collectableLayersInside[startLayerNumberer]._singlePiecePowerAmount)); 
        SetPowerTexter();
    }
    private IEnumerator Noink()
    {
        for (int i = currentLayerNumber+1; i < _collectableLayersInside.Count; i++)
        {
            _collectableLayersInside[i]._mainParent.transform.DOLocalRotate(new Vector3(0, 360, 0), .25f, RotateMode.LocalAxisAdd);
            foreach (GameObject cube in _collectableLayersInside[i]._cubesInside)
            {
                Vector3 localPosition = cube.transform.localPosition;
                cube.transform.DOLocalMove(new Vector3(localPosition.x * 1.3f, localPosition.y, localPosition.z * 1.3f), .15f);
                cube.transform.DOLocalMove(localPosition, .1f).SetDelay(.15f);
            }
            yield return new WaitForSeconds(.03f);
        }
        nonking = false;
    }
    private IEnumerator SendPieces(List<GameObject> _piecesToSend,float piecePowerer)
    {
        yield return new WaitForSeconds(.8f);
        for(int i = 0; i < _piecesToSend.Count; i++)
        {
            GameObject _singlePiece = _piecesToSend[i];
            _singlePiece.gameObject.tag = "SinglePiece";
            _piecesToSend[i].transform.DOComplete();
            _piecesToSend[i].transform.DOKill();
            _piecesToSend[i].GetComponent<Rigidbody>().isKinematic = true;
            Vector3 _jumpPosition = GameManager.instance._collectablePosition.position;
            _jumpPosition.z = _piecesToSend[i].transform.position.z;
            _piecesToSend[i].transform.DOJump(_jumpPosition, 1, 1, .3f).OnComplete(delegate {
                _singlePiece.AddComponent<SingleCollectablePiecer>().move = true;
                _singlePiece.GetComponent<SingleCollectablePiecer>()._power = piecePowerer;
            });
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            GetHit(other.GetComponent<BulletScript>().bulletPower);
            other.GetComponent<BulletScript>().BulletDeActivate(true,true);
        }
    }
}
