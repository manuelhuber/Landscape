using Terrain.Generation;
using UnityEditor;
using UnityEngine;

namespace Terrain.Editor {
[CustomEditor(typeof(EndlessTerrainGenerator))]
public class EndlessTerrainGeneratorInspector : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        var generator = (EndlessTerrainGenerator) target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate")) {
            generator.GenerateInitialTerrain();
        }
    }
}
}