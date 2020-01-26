using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
[CustomEditor(typeof(GenerateHexTower))]
public class HexTowerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateHexTower hexTower = (GenerateHexTower)target;
        if (GUILayout.Button("Generate Mesh"))
        {
            hexTower.GenerateMesh();
        }
        if (GUILayout.Button("Clear")) {
            hexTower.ClearMesh();
        }
    }
}
