using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUICorePanel : MonoBehaviour
{
    public static GUICorePanel gUICorePanel;
    GridLayoutGroup glr;
    Animation anim;
    public List<Text> coreTextList;
    Coroutine mevcutCor;

    private void Awake()
    {
        gUICorePanel = this;
        glr = GetComponent<GridLayoutGroup>();
        anim = GetComponent<Animation>();
    }

    public List<Transform> barTransformList;

    public void BasChildAta(Element element)
    {
        barTransformList[(int)element].SetAsLastSibling();
    }

    IEnumerator GUIKapat()
    {
        yield return new WaitForSeconds(5);
        //glr.spacing = new Vector2(glr.spacing.x, -glr.cellSize.y);
        anim.AnimasyonuKesinBaslat("CorePanelKapa");
        mevcutCor = null;
    }

    public void GUIAc()
    {
        //glr.spacing = new Vector2(glr.spacing.x, 0);
        anim.AnimasyonuKesinBaslat("CorePanelAc");
        mevcutCor = StartCoroutine(GUIKapat());
    }

    public void CoreMiktar(Element e, int miktar)
    {
        int mevcut = PlayerPrefs.GetInt(DefaultDegerler.PP_ELEMENT_ADLARI[(int)e-1]);
        PlayerPrefs.SetInt(DefaultDegerler.PP_ELEMENT_ADLARI[(int)e-1], mevcut + miktar);
        coreTextList[(int)e-1].text = "" + (mevcut + miktar);

        BasChildAta(e-1);

        if (mevcutCor == null)
            GUIAc();
    }
}
