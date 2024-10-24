using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(rotate_towards))]
public class rotate_towards_editor : Editor
{
    public override void OnInspectorGUI()
    {
        rotate_towards myScript = (rotate_towards)target;

        //Toggle
        myScript.toObject = EditorGUILayout.Toggle("Towards object", myScript.toObject);

        //Check if the speed is random
        if (myScript.toObject)
        {
            myScript.toMouse = false;
            myScript.rotationObject = (GameObject)EditorGUILayout.ObjectField("Object", myScript.rotationObject, typeof(Object), true);
        }

        myScript.toMouse = EditorGUILayout.Toggle("Towards mouse", myScript.toMouse);

        if (myScript.toMouse)
        {
            myScript.toObject = false;
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}