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

    [ContextMenu("Item Drop")]
    public void ItemDrop()
    {
        coll = GetComponent<Collider>();
        anim = GetComponent<Animation>();

        item = new Item(item.itemName, item.itemType, item.level, item.stat);

        //itemObjeMesh.mesh = item.mesh;

        anim.AnimasyonuKesinBaslat("ItemDrop");

        UzantiMetotlar.GecikmeliEylem(1, ()=> 
        { 
            anim.AnimasyonuKesinBaslat("ItemObjeIdle");  
            coll.enabled = true; 
        });
        
        ps.Play();
    }
}
