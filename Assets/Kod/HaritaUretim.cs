using UnityEngine;


public class HaritaUretim : MonoBehaviour
{
    public GameObject hexPrefab; // Eşkenar altıgen prefabı
    public float hexRadius = 1.0f; // Altıgenin yarıçapı
    public float hexModelScale = 7.0f; // Altıgenin model scale çarpanı
    private float xOffset; // X ekseni aralığı (altıgenin genişliği)
    private float yOffset; // Y ekseni aralığı (altıgenin yüksekliği)
    public int mapWidth = 10; // Harita genişliği
    public int mapHeight = 10; // Harita yüksekliği

    void Start()
    {
        xOffset = 1.5f * hexModelScale;
        yOffset = Mathf.Sqrt(3) * hexModelScale;
        GenerateHexMap();
    }

    void GenerateHexMap()
    {
        GameObject biome = new GameObject("biome");
        biome.transform.parent = transform;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Altıgenin pozisyonunu hesapla
                float xPos = x * xOffset;
                float yPos;

                if (x % 2 == 0)
                    yPos = y * yOffset;
                else
                    yPos = y * yOffset + yOffset / 2;

                // Altıgeni oluştur ve pozisyonunu ayarla
                GameObject hex = hexPrefab.ObjeUret(new Vector3(xPos, 0, yPos), 0, biome.transform);

                // Altıgenin çapını ayarla
                hex.transform.localScale = new Vector3(hexRadius, hexRadius, hexRadius);
            }
        }
    }
}
