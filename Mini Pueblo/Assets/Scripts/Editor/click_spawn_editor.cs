using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(click_spawner))]
public class click_spawn_editor : Editor
{
    public override void OnInspectorGUI()
    {
        click_spawner myScript = (click_spawner)target;

        //Always visible variables
        myScript.spawnObject = (GameObject)EditorGUILayout.ObjectField("Object", myScript.spawnObject, typeof(GameObject), true);

        //Toggle
        myScript.onPressed = EditorGUILayout.Toggle("Spawn when pressed", myScript.onPressed);

        if (myScript.onPressed)
        {
            myScript.onReleased = false;

            myScript.continuousSpawn = EditorGUILayout.Toggle("Continue while pressed", myScript.continuousSpawn);
            if (myScript.continuousSpawn)
            {
                myScript.timeToSpawn = EditorGUILayout.FloatField("Time between spawns", myScript.timeToSpawn);
            }
        }

        myScript.onReleased = EditorGUILayout.Toggle("Spawn when released", myScript.onReleased);

        if (myScript.onReleased)
        {
            myScript.onPressed = false;
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}