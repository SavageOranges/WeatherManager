using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LocationServicesManager))]
public class LocationServicesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LocationServicesManager target = (LocationServicesManager)serializedObject.targetObject;

        if (GUILayout.Button("Use custom location"))
        {
            target.UseCustomLocation();
        }
    }
}
