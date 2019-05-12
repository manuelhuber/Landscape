using Grimity.Loops;
using Grimity.Rng;
using UnityEngine;

namespace Terrain {
public class TerrainGenerator : MonoBehaviour {
    public int Height;
    public int Width;

    public bool AutoUpdate;
    [Range(1, 10)] public int Octaves;
    [Range(0, 1)] public float Persistence;
    public float Lacunarity;
    public float Scale;
    public int Seed;

    public AnimationCurve heightCurve;
    public float HeightAmplifier;
    public bool SetTexture;

    public void Start() {
        RenderMesh();
    }


    public void RenderMesh() {
        var meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();
        var heightMap = Perlin.GeneratePerlinArray(Width, Height, Octaves, Scale, Persistence, Lacunarity, Seed);
        meshFilter.sharedMesh = GenerateMesh(Height, Width, heightMap);
        if (SetTexture) {
            meshRenderer.sharedMaterial.mainTexture = GenerateTexture(Height, Width, heightMap);
        }
    }

    public Mesh GenerateMesh(int height, int width, float[,] heightMap) {
        // VERTICES ------------
        var vertices = new Vector3[height * width];
        var uvs = new Vector2[height * width];
        var index = 0;


        new Loop2D(width, height).loopByColumn((x, y) => {
            var elevation = heightCurve.Evaluate(heightMap[x, y]) * HeightAmplifier;
            vertices[index] = new Vector3(x, elevation, y);
            uvs[index] = new Vector2(x / (float) width, y / (float) height);
            index++;
        });

        // TRIANGLES ------------
        var triangles = new int[3 * 2 * (width - 1) * (height - 1)];
        index = 0;
        new Loop2D(width - 1, height - 1).loopByRow((x, y) => {
            var topLeft = height * x + y;
            var topRight = topLeft + 1;
            var bottomRight = topRight + height;
            var bottomLeft = topLeft + height;
            triangles[index++] = topLeft;
            triangles[index++] = bottomRight;
            triangles[index++] = bottomLeft;

            triangles[index++] = topLeft;
            triangles[index++] = topRight;
            triangles[index++] = bottomRight;
        });


        var mesh = new Mesh {vertices = vertices, triangles = triangles, uv = uvs};
        mesh.RecalculateNormals();
        return mesh;
    }

    private Texture2D GenerateTexture(int height, int width, float[,] heightMap) {
        var texture = new Texture2D(width,height);


        var colors = new Color[height * width];
        new Loop2D(width, height).loopByColumn((x, y) => {
            colors[y * width + x] = new Color(heightMap[x, y], heightMap[x, y], heightMap[x, y]);
        });
        texture.SetPixels(colors);

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }


    private void OnValidate() {
        if (AutoUpdate) {
            RenderMesh();
        }
    }
}
}