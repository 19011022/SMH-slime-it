using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
//using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class Karakter : MonoBehaviour
{
    public static Karakter karakter;

    //temel degiskenler
    public int guc;
    public float can;
    public int hareketHizi;
    public int saldiriHizi;
    public int sans;
    public float skillHasar = 2;
    public float petGuc;

    //max degerler
    public int maxCan;

    //gui
    public Image canBarImage;
    public Image manaBarImage;
    public Text canText;
    public RectTransform canvas;

    public ChunkBilgi mevcutChunk;


    //envanter
    public OzellikDegis ozellik;
    public List<Item> items;
    public GameObject itemEtkilesimPanel;
    public Image itemEtkilesimImage;


    [Header("Materyal")]
    public Animation matAnim;

    [Header("HUD")]
    public Transform pusula;

    private void Awake()
    {
        karakter = this;
    }

    void Start()
    {
        maxCan = DefaultDegerler.DEFAULT_CAN;
        sans = DefaultDegerler.DEFAULT_SANS;
        hareketHizi = DefaultDegerler.DEFAULT_HAREKETHIZI;
        saldiriHizi = DefaultDegerler.DEFAULT_SALDIRIHIZI;
        guc = DefaultDegerler.DEFAULT_GUC;
        petGuc = DefaultDegerler.DEFAULT_GUC / 10;

        can = maxCan;

        GUIGuncelle();
    }

    private void Update()
    {
        canvas.rotation = KameraTakip.kt.nickAci;
        canvas.localScale = Vector3.one * 0.005f * KameraTakip.kt.canCancasScale / transform.localScale.x;

        pusula.localEulerAngles = Vector3.forward * -transform.localEulerAngles.y;// Mathf.Atan2(transform.forward.z, transform.forward.x) * Mathf.Rad2Deg;
    }

    public void CanDegis(float degisim)
    {
        can += degisim;

        if (can > maxCan)
            can = maxCan;

        if (can <= 0)
        {
            can = 0;
            OyuncuOlum();
        }

        GUIGuncelle();
    }

    void OyuncuOlum()
    {
        BaseDon();
        can = maxCan;
        GUIGuncelle();
        OyuncuSaldiri.saldiri.OyuncuOlum();
    }

    public void GUIGuncelle()
    {
        canBarImage.fillAmount = can / maxCan;
        canBarImage.fillAmount = can / maxCan;
        canText.text = (int)can + "";
    }

    public void SlimeSaldirdi(float hasar)
    {
        KameraTakip.kt.Shake();
        CanDegis(-hasar);
        matAnim.AnimasyonuKesinBaslat("OyuncuMatCanAzalma");
    }

    //item
    public void ChangeItem()
    {
        GameObject itemObje = acikItem.gameObject;
        Item item = itemObje.GetComponent<DroppedItem>().item;
        //items[(int)newItem.itemType] = newItem;

        if (item.itemType == ItemType.Weapon) ozellik.SilahAc((OzellikSeviye)item.level - 1);
        else if (item.itemType == ItemType.Armour) ozellik.ZirhAc((OzellikSeviye)item.level - 1);
        else if (item.itemType == ItemType.Pet) ozellik.PetAc((OzellikSeviye)item.level - 1);
        else if (item.itemType == ItemType.Book) ozellik.SkillAc((OzellikSeviye)item.level - 1);

        print("item degisti");
        //yeni itemi ele al, eldekini yere at

        itemObje.ObjeSil();
        itemEtkilesimPanel.ObjeKapa();
        acikItem = null;
    }

    // Burns the currently equipped item
    public void BurnItem()
    {
        GameObject itemObje = acikItem.gameObject;
        Item newItem = itemObje.GetComponent<DroppedItem>().item;
        GUICorePanel.gUICorePanel.CoreMiktar((Element)newItem.itemType, UnityEngine.Random.Range(10, 16) * newItem.level);
        itemObje.ObjeSil();

        itemEtkilesimPanel.ObjeKapa();
        acikItem = null;
    }

    Transform acikItem;

    void ItemPanelAc()
    {
        print(acikItem.GetComponent<DroppedItem>().item.level);
        itemEtkilesimPanel.ObjeAc();
        itemEtkilesimImage.sprite = acikItem.GetComponent<DroppedItem>().item.sprite;
    }
    void ItemPanelKapa()
    {
        itemEtkilesimPanel.ObjeKapa();
        acikItem = null;
    }


    //trigger fn
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            mevcutChunk = other.transform.parent.GetComponent<ChunkBilgi>();
            mevcutChunk.ChunkaGirildi();
        }
        else if (other.CompareTag("Item"))
        {
            if (acikItem == null)
            {
                acikItem = other.transform;
                ItemPanelAc();
            }
        }
        else if (other.CompareTag("Deniz"))
        {
            BaseDon();
        }
        else if (other.CompareTag("Forge"))
        {
            OyunKontrol.ok.forgeTable.PanelAc();
        }
        else if (other.CompareTag("Info"))
        {
            OyunKontrol.ok.infoPanel.ObjeAc();
        }
        else if (other.CompareTag("CoreDrop"))
        {
            int seviye = other.GetComponent<DroppedItem>().item.level;
            Element itemTur = other.GetComponent<DroppedItem>().element;

            GUICorePanel.gUICorePanel.CoreMiktar(itemTur, seviye * seviye * 10);
            other.tag = "Untagged";
            other.gameObject.ObjeSil();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            mevcutChunk.ChunktanCik();
            mevcutChunk = null;
        }
        else if (other.CompareTag("Forge"))
        {
            OyunKontrol.ok.forgeTable.PanelKapa();
        }
        else if (other.CompareTag("Info"))
        {
            OyunKontrol.ok.infoPanel.ObjeKapa();
        }
        else if (other.CompareTag("Item"))
        {
            if (acikItem == other.transform)
                ItemPanelKapa();
        }
    }

    public void SlimeCarpti()
    {
        matAnim.AnimasyonuKesinBaslat("OyuncuMatCanAzalma");
        CanDegis(-maxCan * .1f);
    }

    public void BaseDon()
    {
        transform.position = TileGenerator.tg.baseChunk.position;
    }
}


public static class DefaultDegerler
{
    public const int DEFAULT_CAN = 100;
    public const int DEFAULT_SANS = 100;
    public const int DEFAULT_SALDIRIHIZI = 100;
    public const int DEFAULT_HAREKETHIZI = 100;
    public const int DEFAULT_GUC = 25;

    //public static readonly Item DEFAULT_WEAPON = new("Beginner Weapon", ItemType.Weapon, 0, 1);

    public static readonly List<string> PP_ELEMENT_ADLARI = new List<string> { "AtesCore", "SuCore", "ToprakCore", "HavaCore" };
}

public enum Element
{
    None,
    Ates,
    Su,
    Toprak,
    Hava,
}