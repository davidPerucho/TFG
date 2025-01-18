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

            myScript.fixedX = EditorGUILayout.Toggle("Fixed X", myScript.fixedX);
            if (myScript.fixedX)
            {
                myScript.fixedY = false;
                myScript.fixedXValue = EditorGUILayout.FloatField("X", myScript.fixedXValue);
            }

            myScript.fixedY = EditorGUILayout.Toggle("Fixed Y", myScript.fixedY);
            if (myScript.fixedY)
            {
                myScript.fixedX = false;
                myScript.fixedYValue = EditorGUILayout.FloatField("Y", myScript.fixedYValue);
            }
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}