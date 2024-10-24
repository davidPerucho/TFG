using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(draw_parable))]
public class draw_parable_editor : Editor
{
    public override void OnInspectorGUI()
    {
        draw_parable myScript = (draw_parable)target;

        //Descriptions
        GUIContent dynamicDifficultyContent = new GUIContent("Dynamic Difficulty", "The size of the parable will increase as the player scores more points.");
        GUIContent pointsToIncreaseContent = new GUIContent("Points to increase", "The amount of points needed to increase the difficulty.");

        //Always visible variables
        myScript.point = (GameObject)EditorGUILayout.ObjectField("Point object", myScript.point, typeof(GameObject), true);
        myScript.force = EditorGUILayout.FloatField("Force", myScript.force);
        myScript.numberOfPoints = EditorGUILayout.IntField("Number of points", myScript.numberOfPoints);
        myScript.spaceBetwenPoints = EditorGUILayout.FloatField("Space between points", myScript.spaceBetwenPoints);

        myScript.drawWhenTouched = EditorGUILayout.Toggle("Draw when touch", myScript.drawWhenTouched);

        if (myScript.drawWhenTouched)
        {
            myScript.drawAlways = false;
        }

        myScript.drawAlways = EditorGUILayout.Toggle("Draw always", myScript.drawAlways);

        if (myScript.drawAlways)
        {
            myScript.drawWhenTouched = false;
        }

        myScript.dynamicDifficulty = EditorGUILayout.Toggle(dynamicDifficultyContent, myScript.dynamicDifficulty);
        if (myScript.dynamicDifficulty)
        {
            myScript.pointsToIncrease = EditorGUILayout.IntField(pointsToIncreaseContent, myScript.pointsToIncrease);
        }

        //Saves changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }
}