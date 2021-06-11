using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisionConeMeshGeneratorMono))]
public class MeshGeneratorEditor: Editor
{
    // void OnSceneGUI()
    // {
    //     VisionConeMeshGeneratorMono meshGenerator = (VisionConeMeshGeneratorMono)target;
    //     // meshGenerator.MakeMesh();
    // }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VisionConeMeshGeneratorMono meshGenerator = (VisionConeMeshGeneratorMono)target;
        if(GUILayout.Button("Make Mesh"))
            meshGenerator.GenerateMesh();
    }
}