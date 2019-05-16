using System.Collections.Generic;
using Grimity.Collections;
using Grimity.Rng;
using Terrain.Settings;
using UnityEngine;

namespace Terrain.Generation {
public class ChunkSpawner {
    private TerrainSettings _settings;
    private readonly int _seed;
    private Dictionary<Vector2Int, float[,]> heightMaps = new Dictionary<Vector2Int, float[,]>();

    public ChunkSpawner(TerrainSettings settings, int seed) {
        _settings = settings;
        _seed = seed;
    }

    public TerrainChunk SpawnChunk(Vector2Int position, Transform parent, int lod) {
        var heightMap = heightMaps.GetOrCompute(position,
            _ => Perlin.GeneratePerlinArray(_settings.chunkSize,
                _settings.chunkSize,
                _settings.octaves,
                _settings.scale,
                _settings.persistence,
                _settings.lacunarity,
                _seed,
                position.x * (_settings.chunkSize - 1),
                position.y * (_settings.chunkSize - 1)));
        var terrainChunk = TerrainChunk.Create(position, _settings, parent, heightMap);
        terrainChunk.setLodMesh(lod);
        return terrainChunk;
    }
}
}