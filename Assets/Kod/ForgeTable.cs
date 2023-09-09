using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeTable : MonoBehaviour
{
    public int panelNo = 0;
    public List<GameObject> forgeList;

    public void PanelDegis(bool sag)
    {
        forgeList[panelNo].ObjeKapa();
        panelNo = sag ? ((panelNo + 1) % 4) : ((panelNo + 3) % 4);
        forgeList[panelNo].ObjeAc();
    }

    public void ForgeYap()
    {
        if (PlayerPrefs.GetInt(DefaultDegerler.PP_ELEMENT_ADLARI[panelNo]) <= 0)
            return;

        GUICorePanel.gUICorePanel.CoreMiktar((Element)(panelNo+1), -1);
        Karakter.karakter.items[panelNo].StatAtla();
    }

    public void PanelAc()
    {
        transform.gameObject.ObjeAc();
    }

    public void PanelKapa()
    {
        transform.gameObject.ObjeKapa();
    }
}
