using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMover : MonoBehaviour
{
    public Vector2 moveSpeed;
    Renderer rr;
    Vector2 currentOffset;
    private void Awake()
    {
        rr = GetComponent<Renderer>();
    }
    private void Update()
    {
        currentOffset += moveSpeed * Time.deltaTime;
        rr.material.SetTextureOffset("_MainTex", currentOffset);
    }
}
