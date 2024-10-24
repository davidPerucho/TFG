using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(simple_spawner))]
public class simple_spawner_editor : Editor
{
    public override void OnInspectorGUI()
    {
        simple_spawner myScript = (simple_spawner)target;

        //Descriptions
        GUIContent dynamicDifficultyContent = new GUIContent("Dynamic Difficulty", "The time of spawn will get sorter as the player scores more points.");
        GUIContent pointsToIncreaseContent = new GUIContent("Points to increase", "The amount of points needed to increase the difficulty.");

        //Always visible variables
        myScript.spawnObject = (GameObject)EditorGUILayout.ObjectField("Object", myScript.spawnObject, typeof(GameObject), true);
        myScript.randomSpawner = EditorGUILayout.Toggle("Random spawner", myScript.randomSpawner);

        if (myScript.randomSpawner == true)
        {
            myScript.rangeX.y = EditorGUILayout.FloatField("Maximum X", myScript.rangeX.y);
            myScript.rangeX.x = EditorGUILayout.FloatField("Minimum X", myScript.rangeX.x);
            myScript.rangeY.y = EditorGUILayout.FloatField("Maximum Y", myScript.rangeY.y);
            myScript.rangeY.x = EditorGUILayout.FloatField("Minimum Y", myScript.rangeY.x);
        }
        else
        {
            myScript.spawnPoint = EditorGUILayout.Vector2Field("Spawn point", myScript.spawnPoint);
        }

        //Toggle
        myScript.multipleSpawns = EditorGUILayout.Toggle("Multiple spawns", myScript.multipleSpawns);

        //If multiple spawns check random or constant time
        if (myScript.multipleSpawns)
        {
            myScript.dynamicDifficulty = EditorGUILayout.Toggle(dynamicDifficultyContent, myScript.dynamicDifficulty);

            if (myScript.dynamicDifficulty)
            {
                myScript.randomTime = false;
                myScript.pointsToIncrease = EditorGUILayout.IntField(pointsToIncreaseContent, myScript.pointsToIncrease);
            }

            myScript.randomTime = EditorGUILayout.Toggle("Random time", myScript.randomTime);
            
            if (myScript.randomTime)
            {
                myScript.dynamicDifficulty = false;
                myScript.rangeTime.y = EditorGUILayout.FloatField("Maximum time", myScript.rangeTime.y);
                myScript.rangeTime.x = EditorGUILayout.FloatField("Minimum time", myScript.rangeTime.x);
            }
            else
            {
                myScript.secondsBetweenSpawns = EditorGUILayout.FloatField("Seconds between spawns", myScript.secondsBetweenSpawns);
            }
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}
