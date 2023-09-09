using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGenerator : MonoBehaviour
{
    public static TileGenerator tg;

    public List<Material> materials;
    public Material baseMat;

    public int seed;
    public bool seedKullan;

    public Transform baseChunk;

    [Header("Boundary Adjustment")]
    public bool enableBoundaryDistortion = true;
    public DistortionMode distortionMode = DistortionMode.UnityRandom;
    public enum DistortionMode { UnityRandom, PerlinNoise };
    public float perlinScale = 0.1f;
    public float perlinStrength = 1;

    public float unityRandomStrength = 1.2f;
    public float maxDistortionPenetration = 5.0f;

    public bool enableSmoothening = true;
    public int smootheningIterations = 4;
    public int changeTypeWhenCountIs = 3;

    [Header("Biome Set Up")]
    public Transform cellPositionParent;
    public int biomeWidth;  // Width in number of cells
    public int biomeHeigth;  // Height in number of cells
    public float biomeSize;  // The expected edge length of a Voronoi cell
    [Range(min: 0f, max: .5f)] public float voronoiDistortionFactor = 0.5f;  // The range to shift the Voronoi points
    private Vector3[,] featurePoints;  // 2D array of Voronoi feature points


    [Header("Tile Grid Set Up")]
    public GameObject hexTilePrefab;
    public Transform tileParent;
    public int tileHeigth;
    public int tileWidth;
    public float tileSpacing;
    public float defaultTileProbability = .1f;
    [HideInInspector] public Transform baseTile;

    private void Awake()
    {
        tg = this;
    }

    [ContextMenu("Update Grid")]

    private void UpdateGrid()
    {
        GenerateGrid();

        if (enableBoundaryDistortion)
        {
            DistortBoundaries();
            // smoothen boundaries.
        }

        AssignTileBiomeAppearances();

    }

    private void Start()
    {
        if (tileWidth % 2 == 0)
        {
            tileWidth++;
        }

        if (tileHeigth % 2 == 0)
        {
            tileHeigth++;
        }

        if(seedKullan)
            Random.InitState(seed);

        GenerateVoronoiPoints();

        AssignInitialBiomes();

        GenerateGrid();


        // save Random state
        Random.State originalState = Random.state;

        var middlePartTypeAndCellLocationArray = MiddlePartTypeAndCellLocationArray(size: 4);// size is how many tiles are gonna be saved.

        if (enableBoundaryDistortion)
            // the distort boundaries uses the distortionMode option to decide to use perlinNoise or Random
            DistortBoundaries();
        // smoothen boundaries.

        // reset the Random state to previously saved one, because we may or may not have used Random class inside DistortBoundaries.
        Random.state = originalState;

        if (enableSmoothening)
            SmoothenBoundaries(smootheningIterations); // using conwey's game of life algo. to be implemented.

        ScatterRandomDefaultTiles();

        ApplyMiddlePartTypeAndCellLocation(size: 4, info: middlePartTypeAndCellLocationArray);

        baseTile = hexTiles[tileWidth / 2, tileHeigth / 2].transform;

        ChunkBilgi baseChunkBilgi = baseTile.GetComponent<ChunkBilgi>();
        baseChunkBilgi.element = Element.None;
        baseChunkBilgi.baseChunk = true;
        baseChunk = baseTile;
        baseTile.name = "BASE TILE";
        Karakter.karakter.transform.position = baseTile.position;


        AssignTileBiomeAppearances();

    }
    void GenerateVoronoiPoints()
    {
        featurePoints = new Vector3[biomeWidth, biomeHeigth];

        for (int x = 0; x < biomeWidth; x++)
        {
            for (int y = 0; y < biomeHeigth; y++)
            {
                // Calculate the base position for each point (center of each cell)
                Vector3 basePos = new Vector3(x * biomeSize, 0, y * biomeSize) - new Vector3(biomeWidth * biomeSize * 0.5f, 0, biomeHeigth * biomeSize * 0.5f) + new Vector3(biomeSize * .5f, 0, biomeSize * .5f); ;


                // If it's the four center points, don't distort them
                if ((x == biomeWidth / 2 || x == biomeWidth / 2 - 1) && (y == biomeHeigth / 2 || y == biomeHeigth / 2 - 1))
                {
                    featurePoints[x, y] = basePos;
                }
                else
                {
                    // Shift the position based on the distortion factor
                    Vector3 randomOffset = new Vector3(Random.Range(-voronoiDistortionFactor, voronoiDistortionFactor), 0, Random.Range(-voronoiDistortionFactor, voronoiDistortionFactor)) * biomeSize;
                    featurePoints[x, y] = basePos + randomOffset;
                }
            }
        }
    }


    private Vector2Int DetermineVoronoiCell(Vector3 position)
    {
        //!!!! --->> clamp the position inside the area allocated for featurepoints here.

        // Define the boundaries of the shifted area
        float minX = -biomeWidth * biomeSize * 0.5f;
        float maxX = biomeWidth * biomeSize * 0.5f;
        float minZ = -biomeHeigth * biomeSize * 0.5f;
        float maxZ = biomeHeigth * biomeSize * 0.5f;

        // Clamp the position inside the shifted area allocated for feature points
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);


        // Convert the continuous space into grid indices
        int x = Mathf.FloorToInt(position.x / biomeSize) + biomeWidth / 2; // Using size because it's the expected edge length of a Voronoi cell
        int y = Mathf.FloorToInt(position.z / biomeSize) + biomeHeigth / 2;

        // Find the neighboring voronoi points in the grid
        Vector2Int[] surroundingPoints = new Vector2Int[]
        {
        new Vector2Int(x, y),
        new Vector2Int(x+1, y),
        new Vector2Int(x-1, y),
        new Vector2Int(x, y+1),
        new Vector2Int(x, y-1),
        new Vector2Int(x+1, y+1),
        new Vector2Int(x-1, y-1),
        new Vector2Int(x-1, y+1),
        new Vector2Int(x+1, y-1)
        };

        float minDistance = float.MaxValue;
        Vector2Int closestCell = new Vector2Int(-1, -1);

        // Determine the closest voronoi point
        foreach (var point in surroundingPoints)
        {
            if (point.x >= 0 && point.x < biomeWidth && point.y >= 0 && point.y < biomeHeigth)
            {
                float dist = Vector3.Distance(position, featurePoints[point.x, point.y]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestCell = point;
                }
            }
        }
        return closestCell;
    }



    private Element[,] initialLocationBiomes;

    void AssignInitialBiomes()
    {
        initialLocationBiomes = new Element[biomeWidth, biomeHeigth];
        List<Element> centerBiomes = new List<Element> { Element.Ates, Element.Su, Element.Toprak, Element.Hava };

        for (int x = 0; x < biomeWidth; x++)
        {
            for (int y = 0; y < biomeHeigth; y++)
            {
                // If it's the four center points, assign them a unique biome
                if ((x == biomeWidth / 2 || x == biomeWidth / 2 - 1) && (y == biomeHeigth / 2 || y == biomeHeigth / 2 - 1))
                {
                    int randomIndex = Random.Range(0, centerBiomes.Count);
                    initialLocationBiomes[x, y] = centerBiomes[randomIndex];
                    centerBiomes.RemoveAt(randomIndex);  // Remove the biome to ensure uniqueness
                }
                else
                {
                    // For other points, assign a random biome (excluding the Default biome)
                    initialLocationBiomes[x, y] = (Element)Random.Range(1, Enum.GetValues(typeof(Element)).Length);
                }
            }
        }
    }


    ChunkBilgi[,] hexTiles;

    void GenerateGrid()
    {
        hexTiles = new ChunkBilgi[tileWidth, tileHeigth];

        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeigth; y++)
            {
                // Calculate the position for the hex tile
                float xPos = x * tileSpacing * 1.5f;
                float zPos = y * tileSpacing * Mathf.Sqrt(3);

                // If it's an odd column, offset it to fit hexagonal pattern
                if (x % 2 == 1)
                {
                    zPos += tileSpacing * Mathf.Sqrt(3) * 0.5f;
                }

                Vector3 pos = new Vector3(xPos, 0, zPos);
                pos -= new Vector3(tileWidth * tileSpacing * 0.75f, 0, tileHeigth * tileSpacing * Mathf.Sqrt(3) * 0.5f);

                var obj = Instantiate(hexTilePrefab, pos, Quaternion.identity, this.transform);
                obj.gameObject.name = "tile" + x + " " + y;

                var tileInfo = obj.GetComponent<ChunkBilgi>();
                tileInfo.tileIndex = new Vector2Int(x, y);
                tileInfo.hexTilesReference = hexTiles;

                Vector2Int cellLocation = DetermineVoronoiCell(pos); // !!!! --->> this part may return outside of range index for now. 

                tileInfo.cellLocation = cellLocation;
                tileInfo.GetComponent<ChunkBilgi>().element = initialLocationBiomes[cellLocation.x, cellLocation.y];
                tileInfo.transform.parent = tileParent;
                hexTiles[x, y] = tileInfo;
            }
        }

        // Extract neighbors for each tile
        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeigth; y++)
            {
                hexTiles[x, y].ExtractNeighbors();
            }
        }
    }

    private void DistortBoundaries()
    {
        // I moved the scale definition to be a class field 
        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeigth; y++)
            {
                ChunkBilgi tile = hexTiles[x, y];

                // Generate a Perlin noise value for the tile position
                float noiseValue = distortionMode == DistortionMode.PerlinNoise ?
                    perlinStrength * Mathf.PerlinNoise(tile.transform.position.x * perlinScale, tile.transform.position.z * perlinScale)
                    : unityRandomStrength * Random.value;

                Vector2Int secondClosestCell = FindSecondClosestCell(tile.transform.position);

                float distanceToClosestCell = Vector3.Distance(featurePoints[tile.cellLocation.x, tile.cellLocation.y], tile.transform.position);
                float distanceToSecondClosestCell = Vector3.Distance(featurePoints[secondClosestCell.x, secondClosestCell.y], tile.transform.position);

                if (distanceToSecondClosestCell - distanceToClosestCell > maxDistortionPenetration)
                    continue;
                // If the noise value exceeds a threshold, we change the tile's type
                if (noiseValue > 0.5f)
                {
                    if (secondClosestCell != new Vector2Int(-1, -1)) // ensure we found a valid second closest cell
                    {
                        tile.element = initialLocationBiomes[secondClosestCell.x, secondClosestCell.y];
                        tile.cellLocation = secondClosestCell;
                    }
                }
            }
        }
    }


    private Vector2Int FindSecondClosestCell(Vector3 position)
    {
        Vector2Int closestCell = DetermineVoronoiCell(position);
        Vector2Int secondClosestCell = new Vector2Int(-1, -1);
        float minDistance = float.MaxValue;

        // Find the neighboring voronoi points in the grid, similar to before
        int x = closestCell.x;
        int y = closestCell.y;

        Vector2Int[] surroundingPoints = new Vector2Int[]
        {
        new Vector2Int(x, y),      // Current cell
        new Vector2Int(x+1, y),
        new Vector2Int(x-1, y),
        new Vector2Int(x, y+1),
        new Vector2Int(x, y-1),
        new Vector2Int(x+1, y+1),
        new Vector2Int(x-1, y-1),
        new Vector2Int(x-1, y+1),
        new Vector2Int(x+1, y-1)
        };

        // Skip the first point since it's the current cell and find the second closest Voronoi point
        for (int i = 1; i < surroundingPoints.Length; i++)
        {
            var point = surroundingPoints[i];
            if (point.x >= 0 && point.x < biomeWidth && point.y >= 0 && point.y < biomeHeigth)
            {
                float dist = Vector3.Distance(position, featurePoints[point.x, point.y]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    secondClosestCell = point;
                }
            }
        }

        return secondClosestCell;
    }

    private void SmoothenBoundaries(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            // Phase 1: Decide the next type for each tile
            foreach (ChunkBilgi tile in hexTiles)
            {
                Element dominantType = GetDominantNeighborType(tile);
                if (dominantType != Element.None) // assuming you have an "None" or "Undefined" type
                {
                    tile.nextElement = dominantType;
                }
                else
                {
                    tile.nextElement = tile.element; // retains its current type if no dominant type found
                }
            }

            // Phase 2: Update the tiles
            foreach (ChunkBilgi tile in hexTiles)
            {
                tile.element = tile.nextElement;
            }
        }
    }

    private Element GetDominantNeighborType(ChunkBilgi tile)
    {
        Dictionary<Element, int> typeCounts = new Dictionary<Element, int>();
        foreach (ChunkBilgi neighbor in tile.neighboringTiles)
        {
            if (!typeCounts.ContainsKey(neighbor.element))
            {
                typeCounts[neighbor.element] = 0;
            }
            typeCounts[neighbor.element]++;
        }

        // This will filter types seen more than 3 times and then select the one with the highest count.
        return typeCounts.Where(kvp => kvp.Value > changeTypeWhenCountIs).OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
    }

    private (Element, Vector2Int)[,] MiddlePartTypeAndCellLocationArray(int size)
    {
        (Element, Vector2Int)[,] selectedTiles = new (Element, Vector2Int)[size, size];

        int startX = tileWidth / 2 - size / 2;
        int startY = tileHeigth / 2 - size / 2;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                selectedTiles[i, j].Item1 = hexTiles[startX + i, startY + j].element;
                selectedTiles[i, j].Item2 = hexTiles[startX + i, startY + j].cellLocation;
            }
        }

        return selectedTiles;
    }

    private void ApplyMiddlePartTypeAndCellLocation(int size, (Element, Vector2Int)[,] info)
    {
        int startX = tileWidth / 2 - size / 2;
        int startY = tileHeigth / 2 - size / 2;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                hexTiles[startX + i, startY + j].element = info[i, j].Item1;
                hexTiles[startX + i, startY + j].cellLocation = info[i, j].Item2;
            }
        }
    }

    public float level2StartingRadius = 40f; // Adjust these values to determine when each level starts.
    public float level3StartingRadius = 80f;
    public float fadeWidth = 15f; // The width of the fading region. Adjust this for a broader/narrower fade.

    int GetMaterialsBasedOnDistance(float distance)
    {
        float distanceToLevel2Boundary = Math.Abs(distance - level2StartingRadius);
        float distanceToLevel3Boundary = Math.Abs(distance - level3StartingRadius);

        if (distanceToLevel2Boundary < fadeWidth)
        {
            // Probabilistic decision based on how close we are to the boundary
            float distanceToFadeBeginning = distance - (level2StartingRadius - fadeWidth);
            float fadeFactor = distanceToFadeBeginning / (fadeWidth * 2);
            return UnityEngine.Random.value > fadeFactor ? 1 : 2;
        }
        else if (distanceToLevel3Boundary < fadeWidth)
        {
            float distanceToFadeBeginning = distance - (level3StartingRadius - fadeWidth);
            float fadeFactor = distanceToFadeBeginning / (fadeWidth * 2);
            return UnityEngine.Random.value > fadeFactor ? 2 : 3;
        }
        else if (distance < level2StartingRadius)
        {
            return 1;
        }
        else if (distance < level3StartingRadius)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    void ScatterRandomDefaultTiles()
    {
        // 1. Scattering Default Type Tiles:
        foreach (var tile in hexTiles)
        {
            // Check if the tile should be converted to Default
            if (Random.value < defaultTileProbability)
            {
                tile.GetComponent<ChunkBilgi>().element = Element.None;
            }
        }
    }

    void AssignTileBiomeAppearances()
    {
        Vector3 center = Vector3.zero;

        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeigth; y++)
            {
                ChunkBilgi tile = hexTiles[x, y];

                // Calculate the distance from the tile to the center
                float distance = Vector3.Distance(tile.transform.position, center);

                // Get the materials based on distance
                int level = GetMaterialsBasedOnDistance(distance);
                tile.seviye = level;
            }
        }
    }
}