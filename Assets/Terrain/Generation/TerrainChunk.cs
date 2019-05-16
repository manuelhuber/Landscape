using System;
using Grimity.Mesh;
using Terrain.Settings;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Terrain.Generation {
public class TerrainChunk : MonoBehaviour {
    public Mesh[] _lodMeshes = new Mesh[10];

    private MeshFilter _meshFilter;
    private TerrainSettings _settings;
    private float[,] _heightMap;
    private MeshCollider _collider;

    public static TerrainChunk Create(Vector2 position,
                                      TerrainSettings settings,
                                      Transform parent,
                                      float[,] heightMap) {
        var terrainChunk = new GameObject().AddComponent<TerrainChunk>();
        terrainChunk.InitiateChunk(position, settings, parent, heightMap);
        return terrainChunk;
    }

    public void InitiateChunk(Vector2 position,
                              TerrainSettings settings,
                              Transform parent,
                              float[,] heightMap) {
        _settings = settings;
        _heightMap = heightMap;

        var chunk = gameObject;
        chunk.name = $"Chunk ({position.x}, {position.y})";
        chunk.transform.position = new Vector3(
            position.x * settings.WorldLength(),
            0f,
            position.y * settings.WorldLength());
        chunk.transform.parent = parent;
        chunk.layer = 9;

        _meshFilter = chunk.AddComponent<MeshFilter>();
        _collider = chunk.AddComponent<MeshCollider>();
        chunk.AddComponent<MeshRenderer>().material = _settings.material;
        chunk.AddComponent<Rigidbody>().isKinematic = true;
    }

    public void setLodMesh(int lod) {
        var mesh = MeshGenerator.GenerateMesh(
            _heightMap,
            _settings.HeightAmplifier,
            _settings.heightCurve,
            lod,
            _settings.distanceBetweenVertices).generateMesh();
        _meshFilter.sharedMesh = mesh;
        _collider.sharedMesh = mesh;
    }

    private void requestMeshForLoD(int lod, Action<Mesh> callback) {
        if (_lodMeshes[lod] == null) {
            var result = new NativeArray<MeshData>(1, Allocator.TempJob);
            UnsafeUtility.IsBlittable(result.GetType());


            Action cacheAndCallback = () => {
                var mesh = result[0].generateMesh();
                mesh.RecalculateNormals();
                _lodMeshes[lod] = mesh;
                callback(mesh);
            };
            var meshJob = new MeshJob(_heightMap,
                _settings,
                lod,
                cacheAndCallback,
                result);
            meshJob.Schedule();
        } else {
            callback(_lodMeshes[lod]);
        }
    }
}

public struct MeshJob : IJob {
    private float[,] _heightMap;
    private TerrainSettings _settings;
    private int _lod;
    private Action _callback;
    public NativeArray<MeshData> _result;

    public MeshJob(float[,] heightMap,
                   TerrainSettings settings,
                   int lod,
                   Action callback,
                   NativeArray<MeshData> result
    ) {
        _result = result;
        _callback = callback;
        _heightMap = heightMap;
        _settings = settings;
        _lod = lod;
    }

    public void Execute() {
        var meshData = MeshGenerator.GenerateMesh(_heightMap,
            _settings.HeightAmplifier,
            _settings.heightCurve,
            _lod,
            _settings.distanceBetweenVertices);
        _result[0] = meshData;
        _callback();
    }
}

[Serializable]
public struct MeshData {
    public NativeArray<Vector3> Vertices;
    public NativeArray<Vector2> Uvs;
    public NativeArray<int> Triangles;

    public Mesh generateMesh() {
        var mesh = new Mesh {vertices = Vertices.ToArray(), triangles = Triangles.ToArray(), uv = Uvs.ToArray()};
        mesh.RecalculateNormals();
        return mesh;
    }
}
}