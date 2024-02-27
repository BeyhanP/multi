using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElephantSDK;
public class SinglePlatform : MonoBehaviour
{
    public GameObject platformObject;
    public GameObject conveyorObject;
    public Vector3 defaultScalePlatform;
    public Vector3 defaultScaleConveyor;
    public Vector2 defaultTilingPlatform;
    public Vector2 defaultTilingConveyor;
    public Color originalColor, whiteColor;
    private void Awake()
    {
        SetTilings();
    }
    public void SetTilings()
    {
        //conveyorObject.transform.localPosition = new Vector3(-6.8f, conveyorObject.transform.localPosition.y, conveyorObject.transform.localPosition.z);
        //conveyorObject.transform.localScale= new Vector3(conveyorObject.transform.localScale.x, 130.6f, conveyorObject.transform.localScale.z);
        Vector3 currentPlatformScale = platformObject.transform.localScale;
        float difference = currentPlatformScale.x / defaultScalePlatform.x;
        Vector2 tilingPlatform = defaultTilingPlatform * difference;
        Vector2 tilingConveyor = defaultTilingConveyor * difference;
        platformObject.GetComponent<Renderer>().materials[0].SetTextureScale("_MainTex", new Vector2(tilingPlatform.x, defaultTilingPlatform.y));
        platformObject.GetComponent<Renderer>().materials[1].SetTextureScale("_MainTex", new Vector2(tilingPlatform.x, defaultTilingPlatform.y));
        conveyorObject.GetComponent<Renderer>().materials[0].SetTextureScale("_MainTex", tilingConveyor);
        if (RemoteConfig.GetInstance().GetFloat("WhiteGround",1) == 0)
        {
            
        }
        else
        {
            //platformObject.GetComponent<Renderer>().materials[1].color = whiteColor;
        }
    }
}
