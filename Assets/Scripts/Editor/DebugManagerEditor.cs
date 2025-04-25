using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugManager debugManager = DebugManager.Instance;

        if (GUILayout.Button("Test d'attaque"))
        {
            debugManager.TestPlayerAttack();
        }
    }
}
