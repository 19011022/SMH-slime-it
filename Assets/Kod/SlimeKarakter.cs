using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SlimeKarakter : MonoBehaviour
{
    public SuratDurumEnum suratDurum;
    public SlimeHareket slimeHareket;
    public Rigidbody kafaRigid;

    //temel degiskenler
    //max degerler
    [Header("Degerler")]
    public int maxCan;
    public float can;
    public int guc;
    public float canDolmaHiz;

    int hareketHizi;
    int saldiriHizi;

    //gui
    [Header("Canvas")]
    public Image canBarImage;
    public RectTransform canvas;

    Coroutine canYenileCor;

    [Header("YuzObjeler")]
    public List<GameObject> mutlu;
    public List<GameObject> ofkeli;
    public List<GameObject> uzgun;

    List<GameObject> acikYuz;

    public ParticleSystem olumEfekt;
    bool hayatta = true;

    [Header("Slime Bilgi")]
    public Element slimeTur;
    public int seviye;

    void Start()
    {
        maxCan = DefaultDegerler.DEFAULT_CAN;
        hareketHizi = DefaultDegerler.DEFAULT_HAREKETHIZI;
        saldiriHizi = DefaultDegerler.DEFAULT_SALDIRIHIZI;

        can = maxCan;
        acikYuz = mutlu;
    }

    private void Update()
    {
        canvas.rotation = KameraTakip.kt.nickAci;
        canvas.localScale = Vector3.one * 0.005f * KameraTakip.kt.canCancasScale / transform.localScale.x;
    }

    public void OyuncuyaVur()
    {
        Karakter.karakter.SlimeSaldirdi(guc);
    }

    public void CanDegis(float degisim)
    {
        if (!hayatta)
            return;


        can += degisim;

        if (canYenileCor != null)
        {
            StopCoroutine(canYenileCor);
        }


        if (can > maxCan)
            can = maxCan;

        if (can <= 0)
        {
            can = 0;
            GUIGuncelle();
            SlimeOlme();
        }
        else
        {
            canYenileCor = StartCoroutine(CanYenile());
            GUIGuncelle();
        }

        if (!slimeHareket.saldiriyor)
            SuratDegis(SuratDurumEnum.Uzgun);
    }

    public void GUIGuncelle()
    {
        canvas.gameObject.ObjeAc();
        canBarImage.fillAmount = can / maxCan;
    }

    void SlimeOlme()
    {
        kafaRigid.isKinematic = true;
        slimeHareket.animation.AnimasyonuKesinBaslat("SlimeOlme");
        SuratDegis(SuratDurumEnum.Uzgun);

        tag = "Untagged";
        OyuncuSaldiri.saldiri.SlimeOlum();
        slimeHareket.SlimeOlme();
        //slimeHareket.animation.transform.localEulerAngles = new Vector3(-90, 0, 0);

        olumEfekt.Play();
        canvas.gameObject.ObjeKapa();

        ItemUret();
    }

    void ItemUret()
    {
        Transform itemParent = transform.GetChild(transform.childCount - 1);
        
        for (int i = itemParent.childCount - 1; i >= 0; i--)
        {
            Transform altParent = new GameObject().transform;
            altParent.parent = itemParent;
            altParent.position = transform.position;
            altParent.eulerAngles = Vector3.up * Random.Range(0, 360);


            Transform itemT = itemParent.GetChild(0);
            itemT.parent = altParent;
            itemT.gameObject.ObjeAc();
            itemT.GetComponent<DroppedItem>().ItemDrop();
        }
    }

    IEnumerator CanYenile()
    {
        yield return new WaitForSeconds(2);

        while (can < maxCan)
        {
            can += Time.deltaTime * canDolmaHiz;
            GUIGuncelle();
            yield return null;
        }

        can = maxCan;
        GUIGuncelle();
        SuratDegis(SuratDurumEnum.Mutlu);

        yield return new WaitForSeconds(2);
        canvas.gameObject.ObjeKapa();
    }

    public void SuratDegis(SuratDurumEnum yeniDurum)
    {
        acikYuz.ForEach(obje => obje.ObjeKapa());

        switch (yeniDurum)
        {
            case SuratDurumEnum.Mutlu:
                acikYuz = mutlu;
                break;

            case SuratDurumEnum.Uzgun:
                acikYuz = uzgun;
                break;

            case SuratDurumEnum.Ofkeli:
                acikYuz = ofkeli;
                break;
        }


        acikYuz.ForEach(obje => obje.ObjeAc());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SkillTopu"))
        {
            CanDegis(-Karakter.karakter.skillHasar);
        }
    }
}

public enum SuratDurumEnum
{
    Mutlu,
    Uzgun,
    Ofkeli
}