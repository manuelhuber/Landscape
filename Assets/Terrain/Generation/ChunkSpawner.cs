using Grimity.Mesh;
using Grimity.Rng;
using Terrain.Settings;
using UnityEngine;

namespace Terrain.Generation {
public class ChunkSpawner {
    private TerrainSettings _settings;
    private readonly int _seed;

    public ChunkSpawner(TerrainSettings settings, int seed) {
        _settings = settings;
        _seed = seed;
    }

    public GameObject SpawnChunk(Vector2Int position, Transform parent) {
        var chunk = new GameObject($"Chunk ({position.x}, {position.y})");
        var meshFilter = chunk.AddComponent<MeshFilter>();

        chunk.transform.position = new Vector3(
            position.x * _settings.WorldLength(),
            0f,
            position.y * _settings.WorldLength());
        chunk.transform.parent = parent;

        chunk.AddComponent<MeshRenderer>().material = _settings.material;
        chunk.AddComponent<Rigidbody>().isKinematic = true;
        chunk.layer = 9;

        var heightMap = Perlin.GeneratePerlinArray(_settings.chunkSize,
            _settings.chunkSize,
            _settings.octaves,
            _settings.scale,
            _settings.persistence,
            _settings.lacunarity,
            _seed,
            position.x * (_settings.chunkSize - 1),
            position.y * (_settings.chunkSize - 1));
        var mesh = MeshGenerator.GenerateMesh(_settings.chunkSize,
            _settings.chunkSize,
            heightMap,
            _settings.HeightAmplifier,
            _settings.heightCurve,
            levelOfDetail: _settings.levelOfDetail,
            distanceBetweenVertices: _settings.distanceBetweenVertices);
        var collider = chunk.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        meshFilter.sharedMesh = mesh;
        return chunk;
    }
}
}