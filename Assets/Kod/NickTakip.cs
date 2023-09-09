using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NickTakip : MonoBehaviour
{
    public Text mTextOverHead;
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        mTextOverHead.transform.position = cam.WorldToScreenPoint(this.transform.position);
    }
}

