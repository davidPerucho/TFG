using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(initial_force))]
public class initial_force_editor : Editor
{
    public override void OnInspectorGUI()
    {
        initial_force myScript = (initial_force)target;

        myScript.force = EditorGUILayout.FloatField("Force", myScript.force);
        myScript.distanceToDestroy = EditorGUILayout.FloatField("Distance to destroy", myScript.distanceToDestroy);

        myScript.specificDirecton = EditorGUILayout.Toggle("Specific direction", myScript.specificDirecton);
        if (myScript.specificDirecton)
        {
            myScript.touchDirection = false;
            myScript.objectDirection = false;
            myScript.directionX = EditorGUILayout.FloatField("Direction X", myScript.directionX);
            myScript.directionY = EditorGUILayout.FloatField("Direction Y", myScript.directionY);
        }

        myScript.objectDirection = EditorGUILayout.Toggle("Object direction", myScript.objectDirection);
        if (myScript.objectDirection)
        {
            myScript.touchDirection = false;
            myScript.specificDirecton = false;
            myScript.targetObject = (GameObject)EditorGUILayout.ObjectField("Target object", myScript.targetObject, typeof(GameObject), true);
        }

        myScript.touchDirection = EditorGUILayout.Toggle("Touch direction", myScript.touchDirection);
        if (myScript.objectDirection)
        {
            myScript.objectDirection = false;
            myScript.specificDirecton = false;
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}