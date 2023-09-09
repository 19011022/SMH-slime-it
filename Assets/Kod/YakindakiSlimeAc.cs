using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YakindakiSlimeAc : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<SlimeHareket>().AlanaGirdi();
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<SlimeHareket>().AlandanCikti();
    }
}
