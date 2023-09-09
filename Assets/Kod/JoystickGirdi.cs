using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickGirdi : MonoBehaviour
{
    public bool sabitJoy = true;
    public RectTransform bir;
    public RectTransform iki;

    public static Vector2 hareketDegeri;
    Vector2 ilkNokta;
    Vector2 ikincikNokta;
    Vector2 JoyVektoru;
    Vector2 eskiJoy;
    Vector2 fark;

    public float yariCap = 100;
    public static JoystickGirdi jy;

    private void Start()
    {
        hareketDegeri = Vector3.zero;
        eskiJoy = Vector2.down;
        JoyVektoru = Vector2.zero;
        ilkNokta = Vector2.zero;
        ikincikNokta = Vector2.zero;
        fark = new Vector2(Screen.width, Screen.height) / 2;
        
        //bir.localScale *= yariCap / 50;
        yariCap *= OyunKontrol.ekranOrani;

        //JoyAcKapa(false);
        jy = this;
    }

    public bool aktiflik;

    public void Basti()
    {
        ilkNokta = (Vector2)Input.mousePosition;
        ikincikNokta = (Vector2)Input.mousePosition;

        if (OyunKontrol.oyunda)
            JoyAcKapa(true);

        aktiflik = true;
    }
    public void Birakti()
    {
        aktiflik = false;
        bir.localPosition = Vector3.up * -540f / OyunKontrol.ekranOrani;
        iki.localPosition = Vector3.up * -540f / OyunKontrol.ekranOrani;
    }
    


    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    ilkNokta = (Vector2)Input.mousePosition;
        //    ikincikNokta = (Vector2)Input.mousePosition;

        //    if(OyunKontrol.oyunda)
        //        JoyAcKapa(true);
        //}
        if (aktiflik)
        {
            ikincikNokta = (Vector2)Input.mousePosition;
            if (sabitJoy)
            {
                if ((ikincikNokta - ilkNokta).magnitude > yariCap)
                {
                    ikincikNokta = ilkNokta + (ikincikNokta - ilkNokta).normalized * yariCap;
                }
            }
            else
            {
                if ((ilkNokta - ikincikNokta).magnitude > yariCap)
                {
                    Vector2 vektor = (ikincikNokta - ilkNokta).normalized;
                    Vector2 yeniKonum = ikincikNokta - vektor * yariCap;
                    ilkNokta = yeniKonum;
                }
            }

            if(OyunKontrol.oyunda)
                JoyVektoru = (ikincikNokta - ilkNokta) / yariCap;

            bir.localPosition = (ilkNokta - fark) / OyunKontrol.ekranOrani;
            iki.localPosition = (ikincikNokta - fark) / OyunKontrol.ekranOrani;
        }
        else
        {
            JoyVektoru = Vector2.zero;
            ikincikNokta = new Vector2(Screen.width / 2, Screen.height / 4);
            iki.localPosition = (ikincikNokta - fark) / OyunKontrol.ekranOrani;
            //JoyAcKapa(false);
        }
        if(JoyVektoru == Vector2.zero)
        {
            //hareketDegeri = Vector2.zero;
            hareketDegeri = eskiJoy;
        }
        else
        {
            hareketDegeri = JoyVektoru;
            eskiJoy = JoyVektoru;
        }
    }
    public void JoyAcKapa(bool durum)
    {
        bir.gameObject.SetActive(durum);
        iki.gameObject.SetActive(durum);
    }
}
