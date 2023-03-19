using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class NewBehaviourScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGenerator generator = (DungeonGenerator)target;
        if (GUILayout.Button("Generate Rooms"))
        {
            generator.SpawnRooms();
        }

        if (GUILayout.Button("Generate Delaunay"))
        {
            generator.GenerateTriangulation();
        }
    }
}
