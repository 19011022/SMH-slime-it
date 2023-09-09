using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    Animation anim;
    Collider coll;

    public ParticleSystem ps;
    public MeshFilter itemObjeMesh;
    public Item item;

    [HideInInspector] public Element element;

    [ContextMenu("Item Drop")]
    public void ItemDrop(Element el)
    {
        element = el;

        Atama();
        Animasyon();
        ObjeAcma();
    }

    void ObjeAcma()
    {
        Transform objeParent = transform.GetChild(0).GetChild((int)item.itemType);

        if (item.itemType == ItemType.Core)
        {
            Transform obje = objeParent.GetChild((int)element - 1);

            obje.gameObject.ObjeAc();
            transform.tag = "CoreDrop";
        }
        else
        {
            Transform obje = objeParent.GetChild(item.level - 1);

            obje.gameObject.ObjeAc();
        }
    }

    void Atama()
    {
        coll = GetComponent<Collider>();
        anim = GetComponent<Animation>();

        //item = new Item(item.itemName, item.itemType, item.level, item.stat);
    }

    void Animasyon()
    {
        anim.AnimasyonuKesinBaslat("ItemDrop");

        UzantiMetotlar.GecikmeliEylem(1, () =>
        {
            anim.AnimasyonuKesinBaslat("ItemObjeIdle");
            coll.enabled = true;
        });

        ps.Play();
    }
}
