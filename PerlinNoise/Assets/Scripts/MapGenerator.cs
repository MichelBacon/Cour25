using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public enum GenerationType
    {
        RANDOM, PERLINNOISE
    }

    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;

    public TerrainType[] regions;
    public TerrainType[] regions2;

    public void GenerateMap()
    {
        if (generationType == GenerationType.PERLINNOISE)
        {
            GenerateMapWithNoise();
        }
        else if (generationType == GenerationType.RANDOM)
        {
            GenerateMapWithRandom();
        }
    }

    private void GenerateMapWithNoise()
    {
        float[,] noiseMapGround = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapGisement = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + 1, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapContenuGisement = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + 2, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (noiseMapGisement[x, y] >= 0.8f)
                {
                    customTileMap[y * mapWidth + x] = FindTileFromGisement(noiseMapContenuGisement[x, y]);
                }
                else
                {
                    customTileMap[y * mapWidth + x] = FindTileFromRegion(noiseMapGround[x, y]);
                }
            }
        }
        SetTileMap(customTileMap);
    }

    private void GenerateMapWithRandom()
    {
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = UnityEngine.Random.Range(0f, 1f);
                customTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customTileMap);
    }

    private void SetTileMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTileMap[y * mapWidth + x]);
            }
        }
    }

    private TileBase FindTileFromRegion(float rnd)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (rnd <= regions[i].height)
            {
                return regions[i].tile;
            }

        }
        return regions[0].tile;
    }

    private TileBase FindTileFromGisement(float rnd)
    {
        for (int i = 0; i < regions2.Length; i++)
        {
            if (rnd <= regions2[i].height)
            {
                return regions2[i].tile;
            }
        }
        return regions2[0].tile;
    }

    private void OnValidate()
    {
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public TileBase tile;
}
