using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class MovementValues
{
    public string MovementType;
    public float Sensitivity;
    public float Speed;
    public Vector2 clamp;
}
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float runnerSpeedSlow;
    [SerializeField] private float runnerSpeedFast;

    [Header("Movement")]
    public Vector3 forwardVector;
    private float forwardMovementSpeed;
    private float sensitivity;
    private Vector2 xClamp;
    private float xPosition;
    private Vector2 firstMousePos;
    private Vector2 secondMousePos;
    private Camera mainCam;
    private bool clicked;

    private bool finishLinePassed;
    public bool jumping;

    public List<MovementValues> _movementList;

    public static PlayerMovement instance;
    public float startSpeed;

    private void Awake()
    {
        instance = this ;
        _movementList[0].Sensitivity = ElephantSDK.RemoteConfig.GetInstance().GetFloat("Sensitivity", 5);
        _movementList[0].Speed= ElephantSDK.RemoteConfig.GetInstance().GetFloat("Speed", 4);
        mainCam = Camera.main;
        SetMovementValues(0);
        startSpeed = _movementList[0].Speed;
    }
    private void Update()
    {
        if (GameManager.instance.started)
        {
            HorizontalMovement();
            if (!jumping)
            {
                ForwardMovement();
            }
        }
    }
    public void SetMovementValues(int movementState)
    {
        MovementValues mv = _movementList[movementState];
        forwardMovementSpeed = mv.Speed;
        sensitivity = mv.Sensitivity;
        xClamp = mv.clamp;
    }
    private void ForwardMovement()
    {
        transform.Translate(forwardVector * forwardMovementSpeed*Time.deltaTime);
    }
    private void HorizontalMovement()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            firstMousePos = mainCam.ScreenToViewportPoint(Input.mousePosition);
            xPosition = transform.position.x;
            clicked = true;
        }
        else if (Input.GetMouseButton(0)&&clicked)
        {
            secondMousePos = mainCam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 difference = (secondMousePos - firstMousePos)*sensitivity;
            xPosition += difference.x;
            xPosition = Mathf.Clamp(xPosition, xClamp.x, xClamp.y);
            transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
            firstMousePos = secondMousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            clicked = false;
        }
        #else


        if (Input.touchCount > 0)
        {
            Touch _touch = Input.GetTouch(0);
            Vector3 difference = _touch.deltaPosition;
            xPosition += (difference.x)*sensitivity;
            xPosition = Mathf.Clamp(xPosition, xClamp.x, xClamp.y);
            transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        }
        #endif
    }
    public IEnumerator Jump()
    {
        transform.DOMoveY(transform.position.y + 1, .3f).SetEase(Ease.Linear);
        transform.DOMoveZ(transform.position.z - 3, .45f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(.3f);
        transform.DOMoveY(transform.position.y - 1, .15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(.15f);
        jumping = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            finishLinePassed = true;
        }
        else if (other.CompareTag("Engel"))
        {
            if (!jumping)
            {
                StartCoroutine(Jump());
                jumping = true;
            }
        }
        else if (other.CompareTag("Money"))
        {
            other.enabled = false;
            other.transform.DOScale(Vector3.zero, .2f);
            StartCoroutine(GameManager.instance.InstNewMoney(other.transform.position,1,10));
        }
    }
}
