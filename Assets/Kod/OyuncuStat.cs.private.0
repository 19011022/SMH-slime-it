using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OyuncuStat : MonoBehaviour
{
    public void ZirhSeviyeAta(int seviye)
    {
        Karakter.karakter.maxCan = DefaultDegerler.DEFAULT_CAN * seviye * seviye;

    }
    public void SilahSeviyeAta(int seviye)
    {
        Karakter.karakter.guc = DefaultDegerler.DEFAULT_GUC * seviye * seviye;
    }

    public void PetSeviyeAta(int seviye)
    {
        Karakter.karakter.petGuc = (float)DefaultDegerler.DEFAULT_GUC * seviye * seviye / 10;
    }
}
