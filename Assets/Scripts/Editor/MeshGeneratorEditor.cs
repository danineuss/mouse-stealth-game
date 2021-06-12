using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VisionConeMeshGeneratorMono))]
public class MeshGeneratorEditor: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Make Mesh"))
        {
            VisionConeMeshGeneratorMono meshGenerator = (VisionConeMeshGeneratorMono)target;
            meshGenerator.GenerateMesh();
        }
    }
}