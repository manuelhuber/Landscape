using System.Collections.Generic;
using Grimity.Loops;
using Terrain.Settings;
using UnityEngine;

namespace Terrain.Generation {
public class EndlessTerrainGenerator : MonoBehaviour {
    public TerrainSettings settings;
    public int seed;

    private ChunkSpawner _spawner;
    private List<GameObject> chunks = new List<GameObject>();

    public void GenerateInitialTerrain() {
        _spawner = new ChunkSpawner(settings, seed);
        foreach (var chunk in chunks) {
            if (Application.isEditor) DestroyImmediate(chunk);
            else Destroy(chunk);
        }

        new Loop2D(3, 3).loopX((x, y) => { generateChunk(new Vector2Int(x - 1, y - 1)); });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            generateChunk(new Vector2Int(-1, 2));
        }
    }

    private void generateChunk(Vector2Int pos) {
        chunks.Add(_spawner.SpawnChunk(pos, transform));
    }

    void OnValidate() {
        if (settings != null) {
            settings.OnValuesUpdated -= GenerateInitialTerrain;
            settings.OnValuesUpdated += GenerateInitialTerrain;
        }
    }
}
}