using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIInventoryPanel : MonoBehaviour
{
    public Animation corePanelAnim;
    Animation anim;
    public bool inventoryAcik = false;

    private void Start()
    {
        anim = GetComponent<Animation>();
    }

    public void GUIAc()
    {
        anim.AnimasyonuKesinBaslat("InventoryPanelAc");
        corePanelAnim.AnimasyonuKesinBaslat("CorePanelAc");
    }

    public void GUIKapat()
    {
        anim.AnimasyonuKesinBaslat("InventoryPanelKapa");
        corePanelAnim.AnimasyonuKesinBaslat("CorePanelKapa");
    }

    public void InventoryButtonEtkilesim()
    {
        inventoryAcik = !inventoryAcik;

        if (inventoryAcik)
            GUIAc();
        else
            GUIKapat();
    }
}
