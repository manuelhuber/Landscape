using Grimity.Data;
using UnityEngine;

namespace Terrain.Settings {
[CreateAssetMenu()]
public class TerrainSettings : UpdatableData {
    [Range(1, 10)] public int octaves = 2;
    [Range(0, 1)] public float persistence = 0.3f;
    [Min(1)] public float lacunarity = 1.5f;
    [Min(1)] public float scale = 20;
    public AnimationCurve heightCurve;
    public float HeightAmplifier;

    public int chunkSize;
    public Material material;
    public float distanceBetweenVertices = 1;
    public LayerMask layers;

    public float WorldLength() {
        return (chunkSize - 1) * distanceBetweenVertices;
    }
}
}