using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEditor.VersionControl;

public class KameraTakip : MonoBehaviour
{
    public Transform takipEdilen;

    public float canCancasScale;

    public static Transform yedekTakip;
    public Vector3 oyundaUzaklik;
    public Vector3 uzaklik;
    public Vector3 bakilanKonum;

    Vector3 kameraKonum;
    Quaternion quaternion;

    public float hiz = 3;
    public float mesafe = 1;

    CameraShaker degerler;
    bool oyunBasla = false;
    bool yumusakBak = false;
    public static KameraTakip kt;

    public Quaternion nickAci;

    private void Awake()
    {
        kt = this;
        degerler = GetComponent<CameraShaker>();
    }

    public void OyunBasladi()
    {
        oyunBasla = true;
        uzaklik = oyundaUzaklik;
    }

    void FixedUpdate()
    {
        //Vector3 aciliUzaklik = Vector3.up * uzaklik.y + takipEdilen.forward * uzaklik.z;
        kameraKonum = takipEdilen.position + uzaklik * mesafe;
        quaternion = Quaternion.LookRotation(-uzaklik * mesafe + bakilanKonum);
        nickAci = Quaternion.LookRotation(-uzaklik * mesafe + bakilanKonum);

        degerler.RestPositionOffset = Vector3.Lerp(transform.position, kameraKonum, Time.deltaTime * hiz);

        if (oyunBasla)
        {
            if (hiz < 20)
            {
                hiz += Time.deltaTime * 5;
                degerler.RestRotationOffset = Quaternion.Lerp(transform.rotation, quaternion, Time.deltaTime * hiz * hiz / 10).eulerAngles;
            }
            else if (yumusakBak)
            {
                degerler.RestRotationOffset = Quaternion.Lerp(transform.rotation, quaternion, Time.deltaTime * 3).eulerAngles;
            }
            else
            {
                degerler.RestRotationOffset = quaternion.eulerAngles;
            }
        }
        else
        {
            degerler.RestRotationOffset = quaternion.eulerAngles;
        }

        //transform.position = Vector3.Lerp(transform.position, kameraKonum, Time.deltaTime * hiz);
        //transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Time.deltaTime * 5);
    }


    public void Shake()
    {
        degerler.ShakeOnce(3, 3, .1f, .5f);
    }
}


//public IEnumerator OldurenAta(Transform oldurenAdam, float sure)
//{
//    mesafe = .6f;
//    olduren = oldurenAdam;
//    yield return new WaitForSeconds(sure);
//    takipEdilen = olduren;
//    mesafe = 1;
//    hiz = 10;
//    oyunBasla = false;
//}