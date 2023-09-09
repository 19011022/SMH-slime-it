using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class ObjeListesi<T>
{
    public List<T> list;
}

public class OyunKontrol : MonoBehaviour
{
    public static OyunKontrol ok;

    public static bool oyunda;
    public static bool ilkOturum;

    public bool yenidenDogum;

    public GameObject oyuncuCember;
    public GameObject hedefYardim;
    public GameObject gosterge;
    public GameObject oyuncu;
    public GameObject oyuncuPref;

    [Header("Canvas Elemanlar�")]
    public GameObject startObje;
    public GameObject navigasyon;
    public GameObject titresimObje;

    [Header("Win/Lose D�zeni")]
    public GameObject butonBloklayici;

    [Header("Genel De�erler")]
    public bool titresimAciklik;
    public Sprite titresimAcikResim;
    public Sprite titresimKapaliResim;

    [Header("Ekran Ayar� De�erleri")]
    public static float ekranOrani = 1;
    float yukseklik = 2160;

    [Header("Slime it")]
    public List<ChunkGorev> chunkGorevList;
    public List<Text> chunkGorevTextList;
    public GameObject slimePrefab;
    public List<GameObject> entityPrefabList;
    public int chunkCap;
    public Animation minimapAnim;
    public List<Material> slimeMaterials;

    public ForgeTable forgeTable;
    public GameObject infoPanel;

    [Header("Items")]
    public List<ObjeListesi<Item>> itemsList;
    public GameObject itemPrefab;

    private void Update()
    {
        KeyCode.Alpha0.MetotKontrol(() => GUICorePanel.gUICorePanel.CoreMiktar(Element.Ates, 1));
        KeyCode.Alpha1.MetotKontrol(() => GUICorePanel.gUICorePanel.CoreMiktar(Element.Su, 1));
        KeyCode.Alpha2.MetotKontrol(() => GUICorePanel.gUICorePanel.CoreMiktar(Element.Hava, 1));
        KeyCode.Alpha3.MetotKontrol(() => GUICorePanel.gUICorePanel.CoreMiktar(Element.Toprak, 1));
    }

    private void Awake()
    {
        ok = this;

        oyunda = false;

        if (PlayerPrefs.HasKey("Level"))
        {
            titresimAciklik = PlayerPrefs.GetInt("Titresim") == 0 ? true : false;
            TitresimAyar();

            ilkOturum = false;
        }
        else
        {   //�LK OTURUM
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("Para", 0);

            PlayerPrefs.SetString("Nick", "Player");

            PlayerPrefs.SetInt("Titresim", 0);

            ilkOturum = true;
        }

        TumOgelerinBoyutunuDuzenle();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        BolumStart();
    }
    void TumOgelerinBoyutunuDuzenle()
    {
        ekranOrani = Screen.height / yukseklik;

        foreach (Transform oge in transform)
        {
            oge.GetComponent<RectTransform>().anchoredPosition *= ekranOrani;
            oge.GetComponent<RectTransform>().localScale *= ekranOrani;
        }
    }

    void GecikmeliOyundaTrueYap()
    {
        oyunda = true;
    }

    public void BolumStart()
    {
        titresimObje.ObjeKapa();

        oyuncu.GetComponent<OyuncuHareket>().OyunBasladi();

        Invoke("GecikmeliOyundaTrueYap", 0.01f);
    }


    public void BolumLose()
    {
        BolumRestart();
    }

    public void BolumWin()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);

        BolumRestart();
    }

    public void BolumRestart()
    {
        SceneManager.LoadScene(0);
    }

    public void ParaYazdir(int artisMiktari)
    {
        int mevcutPara = PlayerPrefs.GetInt("Para");
        PlayerPrefs.SetInt("Para", mevcutPara + artisMiktari);
    }

    public void TitresimButon()
    {
        titresimAciklik = !titresimAciklik;

        TitresimAyar();
    }

    public void TitresimAyar()
    {
        if (titresimAciklik)
        {
            titresimObje.GetComponent<Image>().sprite = titresimAcikResim;
            PlayerPrefs.SetInt("Titresim", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Titresim", 1);
            titresimObje.GetComponent<Image>().sprite = titresimKapaliResim;
        }
    }

    public static void Titrestir(int i)
    {
        if (ok.titresimAciklik)
        {
            switch (i)
            {
                case 0:
                    //RegularPresetsDemoManager.LightButton();
                    break;
                case 1:
                    //RegularPresetsDemoManager.SuccessButton();
                    break;
                case 2:
                    //RegularPresetsDemoManager.FailureButton();
                    break;
            }
        }
    }

    bool minimapBuyuk = true;
    public void MinimapEtkilesim()
    {
        minimapBuyuk = !minimapBuyuk;

        if(minimapBuyuk)
            minimapAnim.AnimasyonuKesinBaslat("MinimapKucult");
        else
            minimapAnim.AnimasyonuKesinBaslat("MinimapBuyult");
    }

    //public IEnumerator ParaYazdirAnim(int paraArtisMiktari)
    //{
    //    int iterasyonAdet = (int)(paraArtisMiktari / 10f);
    //    float animArasiSure = 1f / iterasyonAdet;

    //    int mevcutPara = PlayerPrefs.GetInt("Para");

    //    PlayerPrefs.SetInt("Para", PlayerPrefs.GetInt("Para") + paraArtisMiktari);

    //    for (int i = 0; i < iterasyonAdet; i++)
    //    {
    //        paraText.text = "" + (mevcutPara + (i + 1) * 10);
    //        yield return new WaitForSeconds(animArasiSure);
    //    }
    //}

    //public IEnumerator EksiParaYazdirAnim(int paraArtisMiktari)
    //{
    //    int iterasyonAdet = (int)(paraArtisMiktari / 10f);
    //    float animArasiSure = 1f / iterasyonAdet;

    //    int mevcutPara = PlayerPrefs.GetInt("Para");

    //    PlayerPrefs.SetInt("Para", PlayerPrefs.GetInt("Para") - paraArtisMiktari); //g�venlik �nlemi i�in ba�ta kaydediyoruz paray�

    //    for (int i = 0; i < iterasyonAdet; i++)
    //    {
    //        paraText.text = "" + (mevcutPara + -(i + 1) * 10);
    //        yield return new WaitForSeconds(animArasiSure);
    //    }
    //}
}



public static class UzantiMetotlar
{
    public static float Pow(this float floor, float pow) => Mathf.Pow(floor, pow);
    public static void print(this object a) => MonoBehaviour.print(a);
    public static void print(this object ilk, params object[] list)
    {
        string a = ilk.ToString();
        for (int i = 0; i < list.Length; i++)
            a += list[i] + ((i == list.Length - 1) ? "" : " ");
        a += "";
        MonoBehaviour.print(a);
    }
    public static void print(params object[] list)
    {
        string a = "";
        for (int i = 0; i < list.Length; i++)
            a += list[i] + ((i == list.Length - 1) ? "" : " ");
        a += "";
        MonoBehaviour.print(a);
    }
    public static void ListeYazdir<T>(this IEnumerable<T> liste, Func<T, string> nesiniYazdir = null, string mesaj = "")
    {
        if (liste == null)
        {
            MonoBehaviour.print("liste null");
            return;
        }
        IEnumerator<T> e1 = liste.GetEnumerator();
        IEnumerator<T> e2 = liste.GetEnumerator();
        e2.MoveNext();

        string a = mesaj + " ";
        if (nesiniYazdir == null)
        {
            while (e1.MoveNext())
            {
                a += e1.Current.ToString();
                a += e2.MoveNext() ? ", " : "";
            }
        }
        else
        {
            while (e1.MoveNext())
            {
                a += nesiniYazdir(e1.Current);
                a += e2.MoveNext() ? ", " : "";
            }
        }
        e1.Dispose();
        e2.Dispose();
        MonoBehaviour.print(a);
    }

    public static void Log(this object log)
    {
        print(log);
    }

    public static void ObjeAc(this GameObject obje)
    {
        obje.SetActive(true);
    }

    public static void ObjeKapa(this GameObject obje)
    {
        obje.SetActive(false);
    }

    /// <summary>
    /// Obje a��ksa kapat�r, kapal�ysa a�ar.
    /// </summary>
    /// <param name="obje"></param>
    public static void ObjeDegis(this GameObject obje)
    {
        obje.SetActive(!obje.activeSelf);
    }

    public static T RastgeleElemanSec<T>(this List<T> kaynakListesi)
    {
        return kaynakListesi[UnityEngine.Random.Range(0, kaynakListesi.Count)];
    }

    public static T RastgeleElemanSecVeSil<T>(this List<T> kaynakListesi)
    {
        T eleman = kaynakListesi[UnityEngine.Random.Range(0, kaynakListesi.Count)]; ;
        kaynakListesi.Remove(eleman);
        return eleman;
    }

    public static void MetotKontrol(this KeyCode key, System.Action metot)
    {
        if (Input.GetKeyDown(key)) metot();
    }
    public static void MetotKontrol2(this KeyCode key, System.Action metot)
    {
        if (Input.GetKey(key)) metot();
    }

    public static GameObject ObjeUret(this GameObject obje, Vector3 konum, float aci, Transform parent)
    {
        GameObject clon = GameObject.Instantiate(obje, konum, Quaternion.Euler(0, aci, 0), parent);

        return clon;
    }

    public static GameObject ObjeUret(this GameObject obje, Vector3 konum, Transform parent)
    {
        GameObject clon = GameObject.Instantiate(obje, konum, Quaternion.identity, parent);

        return clon;
    }

    public static GameObject ObjeUret(this GameObject obje, Vector3 konum)
    {
        GameObject clon = GameObject.Instantiate(obje, konum, Quaternion.identity, null);

        return clon;
    }

    public static GameObject ObjeUret(this GameObject obje, Vector3 konum, float aci, Transform parent, string isim)
    {
        GameObject clon = GameObject.Instantiate(obje, konum, Quaternion.Euler(0, aci, 0), parent);
        clon.transform.name = isim;

        return clon;
    }

    public static void ObjeSil(this GameObject obje)
    {
        MonoBehaviour.Destroy(obje);
    }

    public static void ObjeSil(this GameObject obje, float sure)
    {
        MonoBehaviour.Destroy(obje, sure);
    }

    public static void AnimasyonuKesinBaslat(this Animation animation, string clip)
    {
        if (!animation.isPlaying) animation.Play(clip);
        else
        {
            animation.Stop();
            animation.Play(clip);
        }
    }

    public static void KlibeKonumEkle(this AnimationClip ac, Vector3 baslangicBagilKonum, Vector3 hedefBagilKonum, float sure, KonumOtelemeModu mod = KonumOtelemeModu.lerp)
    {
        Vector3 p = baslangicBagilKonum;
        float rsure = 1 / sure,
            ztan = (hedefBagilKonum.z - p.z) * rsure,
            ytan = (hedefBagilKonum.y - p.y) * rsure,
            xtan = (hedefBagilKonum.x - p.x) * rsure;
        switch (mod)
        {
            case KonumOtelemeModu.def:
                ac.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(new Keyframe(0, p.x), new Keyframe(sure, hedefBagilKonum.x)));
                ac.SetCurve("", typeof(Transform), "localPosition.y", new AnimationCurve(new Keyframe(0, p.y), new Keyframe(sure, hedefBagilKonum.y)));
                ac.SetCurve("", typeof(Transform), "localPosition.z", new AnimationCurve(new Keyframe(0, p.z), new Keyframe(sure, hedefBagilKonum.z)));
                return;
            case KonumOtelemeModu.lerp:
                xtan *= 3;
                ytan *= 3;
                ztan *= 3;

                ac.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(new Keyframe(0, p.x, xtan, xtan, .1f, .1f), new Keyframe(sure, hedefBagilKonum.x, 0, 0, .1f, .1f)));
                ac.SetCurve("", typeof(Transform), "localPosition.y", new AnimationCurve(new Keyframe(0, p.y, ytan, ytan, .1f, .1f), new Keyframe(sure, hedefBagilKonum.y, 0, 0, .1f, .1f)));
                ac.SetCurve("", typeof(Transform), "localPosition.z", new AnimationCurve(new Keyframe(0, p.z, ztan, ztan, .1f, .1f), new Keyframe(sure, hedefBagilKonum.z, 0, 0, .1f, .1f)));
                return;
            case KonumOtelemeModu.lineer:
                ac.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(new Keyframe(0, p.x, xtan, xtan), new Keyframe(sure, hedefBagilKonum.x, xtan, xtan)));
                ac.SetCurve("", typeof(Transform), "localPosition.y", new AnimationCurve(new Keyframe(0, p.y, ytan, ytan), new Keyframe(sure, hedefBagilKonum.y, ytan, ytan)));
                ac.SetCurve("", typeof(Transform), "localPosition.z", new AnimationCurve(new Keyframe(0, p.z, ztan, ztan), new Keyframe(sure, hedefBagilKonum.z, ztan, ztan)));
                return;
            case KonumOtelemeModu.hizlanan:
                xtan *= 5;
                ytan *= 5;
                ztan *= 5;
                ac.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(new Keyframe(0, p.x, 0, 0), new Keyframe(sure, hedefBagilKonum.x, xtan, xtan)));
                ac.SetCurve("", typeof(Transform), "localPosition.y", new AnimationCurve(new Keyframe(0, p.y, 0, 0), new Keyframe(sure, hedefBagilKonum.y, ytan, ytan)));
                ac.SetCurve("", typeof(Transform), "localPosition.z", new AnimationCurve(new Keyframe(0, p.z, 0, 0), new Keyframe(sure, hedefBagilKonum.z, ztan, ztan)));
                return;
        }
    }
    public enum KonumOtelemeModu
    {
        def,
        lerp,
        lineer,
        hizlanan
    }
    public static void KlibeRotasyonEkle(this AnimationClip ac, Vector3 basEuler, Vector3 sonEuler, float sure)
    {
        ac.KlibeRotasyonEkle(basEuler.x, sonEuler.x, sure, "x");
        ac.KlibeRotasyonEkle(basEuler.y, sonEuler.y, sure, "y");
        ac.KlibeRotasyonEkle(basEuler.z, sonEuler.z, sure, "z");
    }
    public static void KlibeRotasyonEkle(this AnimationClip ac, float basEuler, float sonEuler, float sure, string xyz = "z")
    {
        if (basEuler > sonEuler)
        {
            if (basEuler - sonEuler > 180)
                basEuler -= 360;
        }
        else
        {
            if (sonEuler - basEuler > 180)
                sonEuler -= 360;
        }
        ac.SetCurve("", typeof(Transform), "localEulerAngles." + xyz, new AnimationCurve(new Keyframe(0, basEuler), new Keyframe(sure, sonEuler)));
    }

    public static void AnimasyonEkleveBaslat(this GameObject obje, AnimationClip klip)
    {
        obje.AddComponent<Animation>();
        obje.GetComponent<Animation>().AddClip(klip, klip.name);
        obje.GetComponent<Animation>().Play(klip.name);
    }

    public static int testSayisi = 0;
    public static Transform NoktaKoy(Vector3 pos, Transform parent = null, string ad = "", float scale = .1f, float yokOlmaSuresi = -1)
    {
        Transform test = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        test.localScale *= scale;
        test.parent = parent;
        test.localPosition = pos;
        test.name = ad == "" ? ("test " + testSayisi++) : ad;
        test.GetComponent<Collider>().enabled = false;
        if (yokOlmaSuresi != -1)
        {
            UnityEngine.Object.Destroy(test.gameObject, yokOlmaSuresi);
        }
        return test;
    }
    public static Transform NoktaKoy(Vector3 pos, Color renk, Transform parent = null, string ad = "", float scale = .1f, float yokOlmaSuresi = -1)
    {
        Transform test = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        test.localScale *= scale;
        test.parent = parent;
        test.localPosition = pos;
        test.name = ad == "" ? ("test " + testSayisi++) : ad;
        Material m = new Material(test.GetComponent<Renderer>().sharedMaterial);
        test.GetComponent<Collider>().enabled = false;
        m.color = renk;
        test.GetComponent<Renderer>().sharedMaterial = m;
        if (yokOlmaSuresi != -1)
        {
            UnityEngine.Object.Destroy(test.gameObject, yokOlmaSuresi);
        }
        return test;
    }

    public static void DegiskenKonumAnimasyon(this Animation anim, string klipAdi, float sure, Vector3 baslangicKonum, Vector3 hedefKonum)
    {
        float basKonX = baslangicKonum.x;
        float basKonY = baslangicKonum.y;
        float basKonZ = baslangicKonum.z;

        float hedKonX = hedefKonum.x;
        float hedKonY = hedefKonum.y;
        float hedKonZ = hedefKonum.z;

        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, basKonX), new Keyframe(sure, hedKonX));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, basKonY), new Keyframe(sure, hedKonY));
        AnimationCurve curveZ = new AnimationCurve(new Keyframe(0, basKonZ), new Keyframe(sure, hedKonZ));

        anim.clip = anim.GetClip(klipAdi);
        AnimationClip klip = anim.clip;

        klip.ClearCurves();
        klip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        klip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        klip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

        anim.Play(klipAdi);
    }

    public static void DegiskenHacimAnimasyon(this Animation anim, string klipAdi, float sure, Vector3 baslangicScale, Vector3 hedefScale)
    {
        float basKonX = baslangicScale.x;
        float basKonY = baslangicScale.y;
        float basKonZ = baslangicScale.z;

        float hedKonX = hedefScale.x;
        float hedKonY = hedefScale.y;
        float hedKonZ = hedefScale.z;

        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, basKonX), new Keyframe(sure, hedKonX));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, basKonY), new Keyframe(sure, hedKonY));
        AnimationCurve curveZ = new AnimationCurve(new Keyframe(0, basKonZ), new Keyframe(sure, hedKonZ));

        anim.clip = anim.GetClip(klipAdi);
        AnimationClip klip = anim.clip;

        klip.ClearCurves();
        klip.SetCurve("", typeof(Transform), "localScale.x", curveX);
        klip.SetCurve("", typeof(Transform), "localScale.y", curveY);
        klip.SetCurve("", typeof(Transform), "localScale.z", curveZ);

        anim.Play(klipAdi);
    }

    public static void BarLoadAnim(this Animation anim, string klipAdi, float sure, float artisMiktari)
    {
        float baslangic = anim.GetComponent<RectTransform>().localScale.x;
        float bitis = baslangic + artisMiktari;

        AnimationCurve curveBar = new AnimationCurve(new Keyframe(0, baslangic), new Keyframe(sure, bitis));

        anim.clip = anim.GetClip(klipAdi);
        AnimationClip klip = anim.clip;

        klip.ClearCurves();
        klip.SetCurve("", typeof(RectTransform), "localScale.x", curveBar);

        anim.Play(klipAdi);
    }

    public static int IndisKontrol(string ad, int uzunluk)
    {
        int level = PlayerPrefs.GetInt("Level");

        if (PlayerPrefs.HasKey(ad + level)) return PlayerPrefs.GetInt(ad + level);
        else
        {
            List<int> indisList = new List<int>();
            for (int i = 0; i < uzunluk; i++) indisList.Add(i);
            if (PlayerPrefs.HasKey(ad + (level - 1))) indisList.Remove(PlayerPrefs.GetInt(ad + (level - 1)));

            int indis = indisList.RastgeleElemanSec();
            PlayerPrefs.SetInt(ad + level, indis);
            return indis;
        }
    }

    public static void GecikmeliEylem(float gecikme, Action eylem)
    {
        OyunKontrol.ok.StartCoroutine(GecikmeliEylemE(gecikme, eylem));
    }
    static IEnumerator GecikmeliEylemE(float gecikme, Action eylem)
    {
        yield return new WaitForSeconds(gecikme);
        eylem();
    }
    public static CultureInfo us;
    public static string KiloFormat(this int num)
    {
        if (num >= 100_000_000)
            return (num / 1_000_000f).ToString("F0", us) + "M";

        if (num >= 1_000_000)
            return (num / 1_000_000f).ToString("F1", us) + "M";

        if (num >= 100_000)
            return (num / 1000f).ToString("F0", us) + "K";

        if (num >= 1000)
            return (num / 1000f).ToString("F1", us) + "K";

        return num.ToString();
    }
}