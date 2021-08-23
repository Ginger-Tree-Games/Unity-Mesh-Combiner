using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    private void OnSceneGUI()
    {
        MeshCombiner mc = target as MeshCombiner;
        if (Handles.Button(mc.transform.position + Vector3.up * 5, Quaternion.LookRotation(Vector3.up), 3, 3, Handles.SphereHandleCap))
        {
            mc.CombineMeshesComplex();
        }

        
    }

    public override void OnInspectorGUI()
    {
        MeshCombiner mc = target as MeshCombiner;
        if (GUILayout.Button("Create Mesh"))
        {
            mc.CombineMeshesComplex();
        }

        if (GUILayout.Button("Turn Off Children"))
        {
            mc.SetChildrenActive(false);
        }

        if (GUILayout.Button("Turn On Children"))
        {
            mc.SetChildrenActive(true);
        }
    }
}
