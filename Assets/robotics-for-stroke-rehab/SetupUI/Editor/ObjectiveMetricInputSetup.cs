using System;
using UnityEditor;
using UnityEngine;

public class ObjectiveMetricInputSetup : EditorWindow
{
    private GameObject objMetricInputFieldPrefab;
    private Transform parent;
    private string key;
    private string readableName;

    [MenuItem("Window/Objective Metric Input Setup")]
    public static void OpenCustomEditorWindow()
    {
        GetWindow<ObjectiveMetricInputSetup>("Objective Metric Input Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Generate the input field based on this prefab:", EditorStyles.boldLabel);
        objMetricInputFieldPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab template", objMetricInputFieldPrefab, typeof(GameObject), true);

        GUILayout.Label("Parent it to:", EditorStyles.boldLabel);
        parent = (Transform)EditorGUILayout.ObjectField("Parent Transform", parent, typeof(Transform), true);

        GUILayout.Label("Key:", EditorStyles.boldLabel);
        key = EditorGUILayout.TextField(key);

        GUILayout.Label("Readable Name:", EditorStyles.boldLabel);
        readableName = EditorGUILayout.TextField(readableName);

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    private void Generate()
    {
        GameObject instantiatedPrefab = Instantiate(objMetricInputFieldPrefab, parent);
        ObjectiveMetricInputField instantiatedObjectiveMetricInputField = instantiatedPrefab.GetComponent<ObjectiveMetricInputField>();

        instantiatedPrefab.name = readableName;
        instantiatedObjectiveMetricInputField.SetKey(key);
        instantiatedObjectiveMetricInputField.SetReadableName(readableName);
    }
}