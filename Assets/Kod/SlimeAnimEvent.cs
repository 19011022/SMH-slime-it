using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimEvent : MonoBehaviour
{
    public void SlimeSaldirdir()
    {
        GetComponentInParent<SlimeKarakter>().OyuncuyaVur();
    }
}
