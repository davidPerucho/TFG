using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(simple_movement))]
public class simple_movement_editor : Editor
{
    public override void OnInspectorGUI()
    {
        simple_movement myScript = (simple_movement)target;

        //Descriptions
        GUIContent dynamicDifficultyContent = new GUIContent("Dynamic Difficulty", "The speed of the object will increase as the player scores more points.");
        GUIContent pointsToIncreaseContent = new GUIContent("Points to increase", "The amount of points needed to increase the difficulty.");

        //Always visible variables
        myScript.movementDirection = EditorGUILayout.Vector2Field("Movement direction", myScript.movementDirection);
        myScript.distanceToDestroy = EditorGUILayout.FloatField("Distance to destroy", myScript.distanceToDestroy);

        myScript.randomSpeed = EditorGUILayout.Toggle("Random speed", myScript.randomSpeed);

        if (myScript.randomSpeed)
        {
            myScript.dynamicDifficulty = false;
            myScript.minSpeed = EditorGUILayout.FloatField("Minimum speed", myScript.minSpeed);
            myScript.maxSpeed = EditorGUILayout.FloatField("Maximum speed", myScript.maxSpeed);
        }
        else
        {
            myScript.movementSpeed = EditorGUILayout.FloatField("Speed", myScript.movementSpeed);
        }

        myScript.dynamicDifficulty = EditorGUILayout.Toggle(dynamicDifficultyContent, myScript.dynamicDifficulty);

        if (myScript.dynamicDifficulty)
        {
            myScript.randomSpeed = false;
            myScript.pointsToIncrease = EditorGUILayout.IntField(pointsToIncreaseContent, myScript.pointsToIncrease);
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}
