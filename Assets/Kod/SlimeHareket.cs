using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHareket : MonoBehaviour
{
    public LayerMask zeminLayer;
    public bool hareketAktif;

    public Renderer renderer;
    public Transform efektParent;

    public Animation animation;
    public Animation saldirAnimation;
    public Transform slimeChunk;

    [Header("Degerler")]
    public float yukariZiplaKuvvet;
    public float ileriZiplaKuvvet;
    public float donmeHiz;
    public float antiGravityForce;

    [Header("Dongu")]
    public float minSure;
    public float maxSure;

    Rigidbody rb;
    SlimeKarakter slimeKarakter;

    Vector3 sonHedefYon;
    bool yerdeMi;
    bool oncekiFrameYerdeMi;

    Coroutine kararCor;

    void Start()
    {
        hareketAktif = true;
        rb = GetComponent<Rigidbody>();
        slimeKarakter = GetComponent<SlimeKarakter>();

        rb.isKinematic = false;
        kararCor = StartCoroutine(KararVerme());
    }

    public void AlanaGirdi()
    {
        print("Girdi");
        rb.isKinematic = false;
        kararCor = StartCoroutine(KararVerme());
    }
    public void AlandanCikti()
    {
        rb.isKinematic = true;
        StopCoroutine(kararCor);
    }

    void Update()
    {
        if (saldiriyor)
        {
            Vector3 yon = Karakter.karakter.transform.position - transform.position;
            yon.y = 0;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(yon), Time.deltaTime * donmeHiz * 3);
        }
        else
        {
            Vector3 yon = sonHedefYon;
            yon.y = 0;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(yon), Time.deltaTime * donmeHiz);

            rb.AddForce(Vector3.up * antiGravityForce, ForceMode.Force);

            Ray ray = new Ray(transform.position + Vector3.up * .2f, Vector3.down);

            yerdeMi = Physics.Raycast(ray, .5f, zeminLayer);

            if (yerdeMi && !oncekiFrameYerdeMi)
            {
                animation.Play("SlimeYereCarpma");
            }

            oncekiFrameYerdeMi = yerdeMi;
        }
    }

    IEnumerator KararVerme()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        while (hareketAktif)
        {
            animation.Play("SlimeZipla");
            yield return new WaitForSeconds(.25f);

            BirYoneZipla(ZiplamaYonTayin());
            yield return new WaitForSeconds(Random.Range(minSure, maxSure));
        }
    }

    void BirYoneZipla(Vector3 randomVec)
    {
        Vector3 hedefYon = Vector3.up * yukariZiplaKuvvet;
        Vector3 rastVec = randomVec * ileriZiplaKuvvet;
        hedefYon.x = rastVec.x; hedefYon.z = rastVec.z;

        sonHedefYon = hedefYon;
        rb.velocity = hedefYon;
    }

    Vector3 ZiplamaYonTayin()//Konumu duvara yakinsa iceriyi gosteriyor
    {
        Vector2 randomVec = Random.insideUnitCircle.normalized;
        Vector3 hedefYon = Vector3.zero;
        hedefYon.x = randomVec.x; hedefYon.z = randomVec.y;


        Vector3 fark = transform.position + hedefYon.normalized * 1.5f - slimeChunk.position;
        fark.y = 0;


        if (fark.magnitude > 4.5f)
        {
            fark = slimeChunk.position - transform.position;
            fark.y = 0;

            return fark.normalized;
        }
        else
        {
            return hedefYon.normalized;
        }
    }

    public void SlimeVuruldu()
    {
        rb.velocity = Vector3.down * 5;
        animation.Play("SlimeVurulma");
        slimeKarakter.CanDegis(-Karakter.karakter.guc);
    }

    public void OyuncuCarptiKac()
    {
        Vector3 fark = slimeChunk.position - transform.position;
        fark.y = 0;

        BirYoneZipla(fark.normalized);
    }

    public void SlimeOlme()
    {
        StopCoroutine(kararCor);
        enabled = false;
        hareketAktif = false;
        saldirAnimation.transform.localRotation = Quaternion.identity;

        slimeChunk.GetComponent<ChunkBilgi>().gorevler[0].GorevArttir();
    }

    public bool saldiriyor;

    public void OyuncuVar()//Yaninda oyuncu var ve saldirmaya basla
    {
        saldiriyor = true;
        slimeKarakter.SuratDegis(SuratDurumEnum.Ofkeli);

        StopCoroutine(kararCor);
        kararCor = StartCoroutine(SaldirmaCor());
    }
    public void OyuncuGitti()//Yaninda oyuncu var ve saldirmaya basla
    {
        saldiriyor = false;
        if (slimeKarakter.can < 100)
            slimeKarakter.SuratDegis(SuratDurumEnum.Uzgun);
        else
            slimeKarakter.SuratDegis(SuratDurumEnum.Mutlu);

        StopCoroutine(kararCor);
        kararCor = StartCoroutine(KararVerme());
    }

    IEnumerator SaldirmaCor()
    {
        yield return new WaitForSeconds(1);

        while (hareketAktif)
        {
            saldirAnimation.Play();
            yield return new WaitForSeconds(1.5f);
        }
    }

    ////item drop
    //public ItemElement element;

    //public Item DropItem()
    //{
    //    Item item = new Item();
    //    switch (element)
    //    {
    //        case ItemElement.Fire:
    //            item.itemType = Random.Range(0, 2) == 0 ? ItemType.Weapon : ItemType.Core;
    //            item.itemElement = ItemElement.Fire;
    //            break;
    //            // Handle other slime types similarly...
    //    }
    //    item.level = Random.Range(1, 4); // this will give 1, 2, or 3
    //    // Assign the stat value based on level and item type, e.g.
    //    item.statValue = 10 * item.level; // example calculation
    //    return item;
    //}
}
