using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(simple_rotation))]
public class simple_rotation_editor : Editor
{
    public override void OnInspectorGUI()
    {
        simple_rotation myScript = (simple_rotation)target;

        //Always visible variables
        myScript.rotationSpeed = EditorGUILayout.FloatField("Speed", myScript.rotationSpeed);

        //Toggle
        myScript.randomRotation = EditorGUILayout.Toggle("Random rotation", myScript.randomRotation);

        if (!myScript.randomRotation)
        {
            myScript.rotationLeft = EditorGUILayout.Toggle("Rotation left", myScript.rotationLeft);
            if (myScript.rotationLeft)
            {
                myScript.rotationRight = false;
            }
            myScript.rotationRight = EditorGUILayout.Toggle("Rotation right", myScript.rotationRight);
            if (myScript.rotationRight)
            {
                myScript.rotationLeft = false;
            }
        }

        myScript.rotationAxisX = EditorGUILayout.Toggle("Rotate in X", myScript.rotationAxisX);
        myScript.rotationAxisY = EditorGUILayout.Toggle("Rotate in Y", myScript.rotationAxisY);
        myScript.rotationAxisZ = EditorGUILayout.Toggle("Rotate in Z", myScript.rotationAxisZ);

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}
