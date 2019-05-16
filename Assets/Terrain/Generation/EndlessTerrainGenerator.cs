using Grimity.Collections;
using Grimity.Loops;
using Terrain.Settings;
using UnityEngine;

namespace Terrain.Generation {
public class EndlessTerrainGenerator : MonoBehaviour {
    public TerrainSettings settings;
    public int seed;

    private ChunkSpawner _spawner;
    private BiDictionary<Vector2Int, TerrainChunk> chunks = new BiDictionary<Vector2Int, TerrainChunk>();


    private void Start() {
        GenerateInitialTerrain();
    }

    public void GenerateInitialTerrain() {
        chunks = new BiDictionary<Vector2Int, TerrainChunk>();
        _spawner = new ChunkSpawner(settings, seed);
        foreach (var child in GetComponentsInChildren<Transform>()) {
            if (child.gameObject == gameObject) continue;
            if (Application.isEditor) DestroyImmediate(child.gameObject);
            else Destroy(child.gameObject);
        }

        new Loop2D(3, 3).loopX((x, y) => { generateChunk(new Vector2Int(x - 1, y - 1), 0); });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            generateChunk(new Vector2Int(-1, 2), 0);
        }
    }

    private void generateChunk(Vector2Int pos, int lod) {
        chunks.Add(pos, _spawner.SpawnChunk(pos, transform, lod));
    }

    void OnValidate() {
        if (settings != null) {
            settings.OnValuesUpdated -= GenerateInitialTerrain;
            settings.OnValuesUpdated += GenerateInitialTerrain;
        }
    }

    public void SpawnNeighbours(TerrainChunk terrainChunk) {
        var center = chunks[terrainChunk];
        new Loop2D(3).loopX((x, y) => {
            x -= 1;
            y -= 1;
            var pos = new Vector2Int(center.x + x, center.y + y);
            var chunk = chunks[pos];
            if (chunk == null) {
                generateChunk(pos, 0);
            }
        });
    }
}
}