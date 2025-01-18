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

        myScript.specificX = EditorGUILayout.Toggle("Specific direction X", myScript.specificX);
        if (myScript.specificX)
        {
            myScript.directionX = EditorGUILayout.FloatField("Direction X", myScript.directionX);
        }

        myScript.specificY = EditorGUILayout.Toggle("Specific direction Y", myScript.specificY);
        if (myScript.specificY)
        {
            myScript.directionY = EditorGUILayout.FloatField("Direction X", myScript.directionY);
        }

        myScript.objectDirection = EditorGUILayout.Toggle("Object direction", myScript.objectDirection);
        if (myScript.objectDirection)
        {
            myScript.touchDirection = false;
            myScript.targetObject = (GameObject)EditorGUILayout.ObjectField("Target object", myScript.targetObject, typeof(GameObject), true);
        }

        myScript.touchDirection = EditorGUILayout.Toggle("Touch direction", myScript.touchDirection);
        if (myScript.objectDirection)
        {
            myScript.objectDirection = false;
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}