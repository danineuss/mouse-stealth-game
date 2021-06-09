using UnityEditor;

[CustomEditor(typeof(VisionConeMeshGeneratorMono))]
public class MeshGeneratorEditor: Editor
{
    void OnSceneGUI()
    {
        VisionConeMeshGeneratorMono meshGenerator = (VisionConeMeshGeneratorMono)target;
        meshGenerator.MakeMesh();
    }
}