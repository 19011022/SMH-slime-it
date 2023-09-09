using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzellikDegis : MonoBehaviour
{
    OyuncuStat oyuncuStat;

    [Header("Referanslar")]
    public OyuncuSaldiri saldiri;
    public GameObject petObje;
    public GameObject skillObje;

    [Header("Listeler")]
    public List<GameObject> zirhlar;
    public List<GameObject> silahlar;
    public List<GameObject> petler;
    public List<GameObject> skiller;
    public List<GameObject> kitaplar;

    GameObject acikZirh, acikSilah, acikPet, acikSkill, acikKitap;

    private void Start()
    {
        oyuncuStat = GetComponent<OyuncuStat>();
        acikZirh = zirhlar[0];
        acikSilah = silahlar[0];
    }

    private void Update()
    {
        KeyCode.Q.MetotKontrol(() => ZirhAc(OzellikSeviye.Seviye2));
        KeyCode.E.MetotKontrol(() => SilahAc(OzellikSeviye.Seviye2));
        KeyCode.Space.MetotKontrol(() => PetAc(OzellikSeviye.Seviye2));
        KeyCode.T.MetotKontrol(() => SkillAc(OzellikSeviye.Seviye2));
    }

    public void ZirhAc(OzellikSeviye seviye)
    {
        acikZirh?.ObjeKapa();
        acikZirh = zirhlar[(int)seviye];
        acikZirh?.ObjeAc();
        oyuncuStat.ZirhSeviyeAta((int)seviye + 1);
    }

    public void SilahAc(OzellikSeviye seviye)
    {
        acikSilah?.ObjeKapa();
        acikSilah = silahlar[(int)seviye];
        acikSilah?.ObjeAc();
        oyuncuStat.SilahSeviyeAta((int)seviye + 1);
    }

    public void PetAc(OzellikSeviye seviye)
    {
        petObje.ObjeAc();

        acikPet?.ObjeKapa();
        acikPet = petler[(int)seviye];
        acikPet?.ObjeAc();
        oyuncuStat.PetSeviyeAta((int)seviye + 1);

        saldiri.petAktif = true;
    }

    public void SkillAc(OzellikSeviye seviye)
    {
        skillObje.ObjeAc();

        acikSkill?.ObjeKapa();
        acikSkill = skiller[(int)seviye];
        acikSkill?.ObjeAc();
        //oyuncuStat.s((int)seviye);


        acikKitap?.ObjeKapa();
        acikKitap = kitaplar[(int)seviye];
        acikKitap?.ObjeAc();
    }

}

public enum OzellikSeviye
{
    Seviye1,
    Seviye2,
    Seviye3
}
