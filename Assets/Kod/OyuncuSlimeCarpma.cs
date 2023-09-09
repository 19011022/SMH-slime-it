using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OyuncuSlimeCarpma : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slime"))
        {
            Karakter.karakter.SlimeCarpti();
            other.GetComponent<SlimeHareket>().OyuncuCarptiKac();
        }
    }
}
