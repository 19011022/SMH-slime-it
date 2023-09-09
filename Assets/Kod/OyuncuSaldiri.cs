using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OyuncuSaldiri : MonoBehaviour
{
    public static OyuncuSaldiri saldiri;

    public OyuncuHareket hareket;
    public Transform saldiriSlime;
    public List<Transform> yakinSlimelar;

    [Header("Inverse K.")]
    public Transform spineT;


    [Header("Pet")]
    public bool petAktif;
    public Transform pet;
    public Animator petAnim;

    private void Awake()
    {
        saldiri = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slime"))
        {
            yakinSlimelar.Add(other.transform);

            if (saldiriSlime == null)
            {
                PetSaldiriBasla();
                saldiriSlime = other.transform;
                saldiriSlime.GetComponent<SlimeHareket>().OyuncuVar();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Slime"))
        {
            yakinSlimelar.Remove(other.transform);

            if (saldiriSlime == other.transform)
            {
                saldiriSlime.GetComponent<SlimeHareket>().OyuncuGitti();

                if (yakinSlimelar.Count == 0)
                {
                    saldiriSlime = null;
                    PetSaldiriBitir();
                }
                else
                {
                    saldiriSlime = yakinSlimelar[Random.Range(0, yakinSlimelar.Count)];
                    saldiriSlime.GetComponent<SlimeHareket>().OyuncuVar();
                }
            }
        }
    }

    public void SlimeVuruldu()
    {
        if (saldiriSlime)
            saldiriSlime.GetComponent<SlimeHareket>().SlimeVuruldu();
    }

    private void LateUpdate()
    {
        hareket.vuruyor = saldiriSlime != null;

        if (saldiriSlime)
        {
            Vector3 yon = saldiriSlime.position - spineT.position;
            yon.y = 0;

            spineT.rotation = Quaternion.LookRotation(yon);
        }

        if (petAktif)
        {
            if (saldiriSlime)
                pet.LookAt(saldiriSlime);
            else
                pet.localRotation = Quaternion.identity;
        }
    }

    public void SlimeOlum()
    {
        yakinSlimelar.Remove(saldiriSlime);

        if (yakinSlimelar.Count == 0)
        {
            saldiriSlime = null;
            PetSaldiriBitir();
        }
        else
        {
            saldiriSlime = yakinSlimelar[Random.Range(0, yakinSlimelar.Count)];
            saldiriSlime.GetComponent<SlimeHareket>().OyuncuVar();
        }
    }

    Coroutine petCor;

    public void PetSaldiriBasla()
    {
        if (petAktif)
            petCor = StartCoroutine(PetSaldirCor());
    }
    public void PetSaldiriBitir()
    {
        if (petAktif)
            StopCoroutine(petCor);
    }

    IEnumerator PetSaldirCor()
    {
        while (true)
        {
            petAnim.SetTrigger("Saldir");
            yield return new WaitForSeconds(0.17f);

            saldiriSlime.GetComponent<SlimeKarakter>().CanDegis(-Karakter.karakter.petGuc);
            yield return new WaitForSeconds(1);
        }
    }
}
