using System;
using UnityEditor;
using UnityEngine;

namespace Terrain.Editor {
[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorInspector : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        var generator = (TerrainGenerator) target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate")) {
            generator.RenderMesh();
        }
    }
}
}