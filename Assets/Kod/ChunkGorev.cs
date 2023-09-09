using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChunkGorev
{
    public bool gorevDurumu;
    public string gorevAciklama;

    public int mevcut = 0;
    public int hedef;

    [HideInInspector]
    public ChunkBilgi cb;

    [HideInInspector]
    public Text gorevText;

    public void GorevArttir()
    {
        mevcut++;
        TextGuncelle();

        if(mevcut >= hedef)
            GorevTamamlandi();
    }

    public void GorevTamamlandi()
    {
        gorevDurumu = true;
        gorevText.color = Color.green;
        gorevText.transform.GetChild(0).GetComponent<Image>().color = Color.green;
        //cb.GorevlerKontrol();

        cb.GorevlerTamamlandi();
    }

    public void TextGuncelle()
    {
        gorevText.text = gorevAciklama + " (" + mevcut +"/"+ hedef +")";
        gorevText.color = gorevDurumu ? Color.green : Color.white;
    }
}