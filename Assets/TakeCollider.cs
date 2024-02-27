using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TakeCollider : MonoBehaviour
{
    public List<Transform> _piecePositions = new List<Transform>();
    public List<Transform> _getParticles = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SinglePiece"))
        {
            other.GetComponent<SingleCollectablePiecer>().move = false;
            int randomInt = Random.Range(0, _piecePositions.Count);
            ParticleSystem _getParticle = _getParticles[randomInt].GetComponent<ParticleSystem>();
            other.transform.DOLocalJump(_piecePositions[randomInt].transform.position,2,1,.3f).OnComplete(delegate {
                _getParticle.Play();
	        });
            other.transform.DOScale(Vector3.zero, .1f).SetDelay(.25f);
        }
    }
}
