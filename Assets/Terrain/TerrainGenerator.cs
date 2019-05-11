using System;
using Grimity.Loops;
using UnityEngine;
using Random = System.Random;

namespace Terrain {
public class TerrainGenerator : MonoBehaviour {
    public int Height;
    public int Width;

    public bool AutoUpdate;

    public void Start() {
        RenderMesh();
    }


    public void RenderMesh() {
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = GenerateMesh(Height + 1, Width + 1);
    }

    public Mesh GenerateMesh(int height, int width) {
        
        // VERTICES ------------
        var vertices = new Vector3[height * width];
        var index = 0;
        new Loop2D(width, height).loopByColumn((x, y) => vertices[index++] = new Vector3(x, 0, y));

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

        return new Mesh {vertices = vertices, triangles = triangles};
    }


    private void OnValidate() {
        if (AutoUpdate) {
            RenderMesh();
        }
    }
}
}