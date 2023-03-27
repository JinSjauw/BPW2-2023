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

        /*DungeonGenerator generator = (DungeonGenerator)target;
        if (GUILayout.Button("Generate Dungeon"))
        {
            generator.Generate();
        }
        
        if (GUILayout.Button("Generate Rooms"))
        {
            generator.GenerateRooms();
        }

        if (GUILayout.Button("Generate Delaunay"))
        {
            generator.GenerateTriangulation();
        }
        
        if (GUILayout.Button("Generate Hallways"))
        {
            generator.GenerateHallways();
        }*/
    }
}
