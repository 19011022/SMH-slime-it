using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EfektKontrol : MonoBehaviour
{
    public static EfektKontrol ek;

    [Header("Prefablar")]
    public GameObject splashBoyaPrefab;
    public GameObject puffEfektPrefab;
    public GameObject pariltiEfektPrefab;
    public GameObject kesmeEfektPrefab;

    [Header("Referanslar")]
    public Animation rewardWordAnimation;
    public Animation animatifPara;
    public Animation sizeUpAnim;

    [Header("Degerler")]
    public List<string> rewardWordList;

    private void Awake()
    {
        ek = this;
    }

    public void SplashBoyaOlustur(Vector3 konum, Color renk)
    {
        Transform boya = splashBoyaPrefab.ObjeUret(new Vector3(konum.x, 0, konum.z), Random.Range(0, 360), transform).transform.GetChild(0).transform;
        boya.GetComponent<SpriteRenderer>().color = renk;
        boya.localScale *= Random.Range(.8f, 1.2f);
    }

    public void PuffOlustur(Vector3 konum, Color renk)
    {
        puffEfektPrefab.ObjeUret(konum, 0, transform).GetComponent<ParticleSystem>().startColor = renk;
    }

    public void PuffOlustur(Vector3 konum)
    {
        puffEfektPrefab.ObjeUret(konum, 0, transform);
    }

    public void RewardWordCagir()
    {
        Text t = rewardWordAnimation.GetComponent<Text>();
        t.text = rewardWordList.RastgeleElemanSec();
        t.color = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        if (Random.Range(0, 2) == 0) rewardWordAnimation.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, rewardWordAnimation.GetComponent<RectTransform>().eulerAngles.z * -1);
        rewardWordAnimation.AnimasyonuKesinBaslat("RewardWord");
    }    
    
    public void SizeUpCagir()
    {
        sizeUpAnim.AnimasyonuKesinBaslat("ArtiBir");
    }

    public void PariltiOlustur(Vector3 konum)
    {
        pariltiEfektPrefab.ObjeUret(konum, 0, transform);
    }

    public void KesmeEfektOlustur(Vector3 konum)
    {
        kesmeEfektPrefab.ObjeUret(konum, 0, transform);
    }

    IEnumerator UI_ParaToplamaAnim()
    {
        animatifPara.AnimasyonuKesinBaslat("UI_ParaToplama");
        yield return new WaitForSeconds(.5f);
        OyunKontrol.ok.ParaYazdir(1);
    }
    public void TekliParaToplama()
    {
        StartCoroutine(UI_ParaToplamaAnim());
    }
}
