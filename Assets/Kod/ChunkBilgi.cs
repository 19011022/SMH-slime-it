using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBilgi : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Element element;
    public int seviye;
    public bool baseChunk = false;

    //gorev/buff
    public int buffSure;
    public int gorevAdet;
    public bool gorevAtanabilir;
    [HideInInspector] public List<ChunkGorev> gorevler;
    [HideInInspector] public ChunkBuff chunkBuff;

    //spawn
    public int mevcutEntityAdet;
    public int maxEntityAdet;
    public int spawnCooldown;
    public GameObject entityPrefab;
    public Renderer mapMesh;
    
    private void Start()
    {
        var mat = TileGenerator.tg.materials[(int)element];
        var mats = new List<Material>() { mat, mat };
        GetComponent<MeshRenderer>().materials = mats.ToArray();
        mapMesh.materials = mats.ToArray();

        //spawn
        if (!baseChunk)
        {
        if (element == Element.None)
        {
            gorevAtanabilir = false;
            maxEntityAdet = Random.Range(1, 3);
            entityPrefab = OyunKontrol.ok.entityPrefabList.RastgeleElemanSec();
        }
        else
        {
            maxEntityAdet = 3;//Random.Range(4, 8);
            entityPrefab = OyunKontrol.ok.slimePrefab;
            entityPrefab.GetComponent<SlimeHareket>().slimeChunk = transform;

            //item
            //Item slimeItem = OyunKontrol.ok.itemsList.
            //DroppedItem droppedItem = new() { item = slimeItem };

            //slime seviye ve elementi belirlenip spawnlanacak
            
        }

        for (int i = 0; i < maxEntityAdet; i++)
            SpawnEntity();

        if (gorevAtanabilir)
            GorevAta();

            chunkBuff = (ChunkBuff)Random.Range(0, System.Enum.GetValues(typeof(ChunkBuff)).Length);
        }
        else
        {
            var mat2 = TileGenerator.tg.baseMat;
            var mats2 = new List<Material>() { mat2, mat2 };
            GetComponent<MeshRenderer>().materials = mats2.ToArray();
            mapMesh.materials = mats2.ToArray();
            mapMesh.transform.GetChild(0).gameObject.ObjeAc();
            //base islemleri
        }
    }

    public void SpawnEntity()
    {
        float r = OyunKontrol.ok.chunkCap;
        Vector3 spawnPos = new Vector3(Random.Range(0, r), 0, Random.Range(0, r));
        Transform entity = entityPrefab.ObjeUret(Vector3.zero, Random.Range(0,360), transform).transform;
        entity.localPosition = spawnPos;

        if (entity.GetComponent<SlimeHareket>())
        {
            SlimeHareket sh = entity.GetComponent<SlimeHareket>();
            sh.renderer.material = OyunKontrol.ok.slimeMaterials[(int)element-1];
            sh.efektParent.GetChild((int)element-1).gameObject.ObjeAc();

            SlimeKarakter karakter = entity.GetComponent<SlimeKarakter>();
            karakter.seviye = seviye;
            karakter.slimeTur = element;


            int chanceItem = 0;// Random.Range(0, 2);
            if(chanceItem == 0)
            {
                GameObject _dropItem = OyunKontrol.ok.itemPrefab.ObjeUret(entity.position);
                _dropItem.transform.parent = entity.GetChild(entity.childCount - 1);

                DroppedItem __dropItem = _dropItem.GetComponent<DroppedItem>();
                __dropItem.item = OyunKontrol.ok.itemsList[(int)element - 1].list[seviye];
            }

            int chanceCore = 0;// Random.Range(0, 2);
            if (chanceCore == 0)
            {
                //  5,10 arası * seviye kadar core dusur 

                GameObject _dropItem = OyunKontrol.ok.itemPrefab.ObjeUret(entity.position);
                _dropItem.transform.parent = entity.GetChild(entity.childCount - 1);

                DroppedItem __dropItem = _dropItem.GetComponent<DroppedItem>();
                __dropItem.item = new Item(element.ToString() + "Core", ItemType.Core, seviye, 0);
            }
        }
    }

    //her kill'den sonra cagiriyoruz, mob spawn oalcak mı olmayacak mı vs kontrolünü sağlıyor
    public void EntityKillAndSpawnKontrol()
    {
        mevcutEntityAdet--;

        if(mevcutEntityAdet < maxEntityAdet)
        {
            UzantiMetotlar.GecikmeliEylem(spawnCooldown, ()=> SpawnEntity());
        }
    }

    public void GorevAta()
    {
        List<ChunkGorev> gorevCopy = new List<ChunkGorev>(OyunKontrol.ok.chunkGorevList);

        for (int i = 0; i < gorevAdet; i++)
        {
            ChunkGorev gorev = gorevCopy.RastgeleElemanSecVeSil();
            gorev.cb = this;
            gorev.gorevText = OyunKontrol.ok.chunkGorevTextList[i];
            gorevler.Add(gorev);
        }
    }

    public void GorevlerKontrol()
    {
        int i = 0;
        while (gorevler[i].gorevDurumu && i < gorevler.Count)
            i++;

        if(i == gorevler.Count)
        {
            GorevlerTamamlandi();
        }
    }

    [ContextMenu("Gorevleri ARttir Test")]
    public void GorevleriArttirTest()
    {
        gorevler.ForEach((ChunkGorev obj) => obj.GorevArttir());
    }

    [ContextMenu("Gorev Tamamlandi")]
    public void GorevlerTamamlandi()
    {
        gorevAtanabilir = false;

        print("Görevler Bitti!");

        UzantiMetotlar.GecikmeliEylem(30, () => gorevAtanabilir = false);

        particleSystem.Play();
        GetComponent<Animation>().AnimasyonuKesinBaslat("ChunkParlama");
        BuffVer();
    }

    [ContextMenu("Chunk gir")]
    public void ChunkaGirildi()
    {
        if (!gorevAtanabilir)
            return;

        int i = 0;
        gorevler.ForEach((ChunkGorev obj) => {OyunKontrol.ok.chunkGorevTextList[i++].gameObject.ObjeAc(); ; obj.TextGuncelle(); });
        GetComponent<Animation>().Play();

        //GUICorePanel.gUICorePanel.BasChildAta(element);
    }

    [ContextMenu("Chunk cik")]
    public void ChunktanCik()
    {
        if (!gorevAtanabilir)
            return;

        int i = 0;
        gorevler.ForEach((ChunkGorev obj) => { OyunKontrol.ok.chunkGorevTextList[i++].gameObject.ObjeKapa();});
    }

    public void BuffVer()
    {
        switch (chunkBuff)
        {
            case ChunkBuff.SaldiriHizi:
                Karakter.karakter.saldiriHizi *= 2;
                UzantiMetotlar.GecikmeliEylem(buffSure, ()=>
                {
                    Karakter.karakter.saldiriHizi /= 2;
                    GetComponent<Animation>().AnimasyonuKesinBaslat("ChunkParlama");
                });
                break;

            case ChunkBuff.HareketHizi:
                Karakter.karakter.hareketHizi *= 2;
                UzantiMetotlar.GecikmeliEylem(buffSure, () =>
                {
                    Karakter.karakter.hareketHizi /= 2;
                    GetComponent<Animation>().AnimasyonuKesinBaslat("ChunkParlama");
                });
                break;

            //case ChunkBuff.GucArtisi:
            //    Karakter.karakter.guc *= 2;
            //    print("GucArtisi artisi!");
            //    break;

            //case ChunkBuff.SansArtisi:
            //    Karakter.karakter.saldiriHizi *= 2;
            //    print("SansArtisi artisi!");
            //    break;
        }
    }


    //Tile info
    public Vector2Int cellLocation; // Biome location in 2D array
    public Vector2Int tileIndex;    // Tile location in HexTiles 2D array
    public ChunkBilgi[,] hexTilesReference;
    public List<ChunkBilgi> neighboringTiles;

    public Element nextElement;
    public Vector2Int nextCellLocation;

    public void ExtractNeighbors()
    {
        neighboringTiles = new List<ChunkBilgi>();

        // Top and Bottom in the same column
        AddNeighbor(tileIndex.x, tileIndex.y - 1);
        AddNeighbor(tileIndex.x, tileIndex.y + 1);

        // Depending on if the current column is odd or even
        if (tileIndex.x % 2 == 0)
        {
            AddNeighbor(tileIndex.x - 1, tileIndex.y);
            AddNeighbor(tileIndex.x + 1, tileIndex.y);
            AddNeighbor(tileIndex.x - 1, tileIndex.y - 1);
            AddNeighbor(tileIndex.x + 1, tileIndex.y - 1);
        }
        else
        {
            AddNeighbor(tileIndex.x - 1, tileIndex.y);
            AddNeighbor(tileIndex.x + 1, tileIndex.y);
            AddNeighbor(tileIndex.x - 1, tileIndex.y + 1);
            AddNeighbor(tileIndex.x + 1, tileIndex.y + 1);
        }
    }

    private void AddNeighbor(int x, int y)
    {
        if (x >= 0 && x < hexTilesReference.GetLength(0) && y >= 0 && y < hexTilesReference.GetLength(1))
        {
            neighboringTiles.Add(hexTilesReference[x, y]);
        }
    }
}

public enum ChunkBuff
{
    SaldiriHizi,
    HareketHizi,
    GucArtisi,
    SansArtisi
}
