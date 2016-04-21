using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DevConsoleGUI))]
public class DevConsoleGUIInspector : Editor {
    private bool showAdvanced = false;
    private bool showMaxSuggestions = false;

    public override void OnInspectorGUI()
    {
        DevConsoleGUI myTarget = (DevConsoleGUI)target;

        myTarget.clearOnSubmit = EditorGUILayout.Toggle("Clear on submit", myTarget.clearOnSubmit);
        myTarget.reselectOnSubmit = EditorGUILayout.Toggle("Reselect on submit", myTarget.reselectOnSubmit);
        myTarget.minCharBeforeSuggestions = EditorGUILayout.IntField(new GUIContent("Suggest after", "Minimum amount of characters before suggesting"), myTarget.minCharBeforeSuggestions);

        showMaxSuggestions = EditorGUILayout.Foldout(showMaxSuggestions, "Max Suggestions");
        if (showMaxSuggestions)
            myTarget.maxSuggestions = (int) EditorGUILayout.Slider(myTarget.maxSuggestions, 0,20);

        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Show Advanced");
        if (showAdvanced)
        {
            myTarget.maxOutputLenght = EditorGUILayout.IntField("Max Output Lenght", myTarget.maxOutputLenght);
            myTarget.devConsoleOutput = (Text) EditorGUILayout.ObjectField("Output Text", myTarget.devConsoleOutput, typeof(Text), false);
            myTarget.suggestionsParent = (Transform)EditorGUILayout.ObjectField("Suggestions Parent", myTarget.suggestionsParent, typeof(Transform), false);
            myTarget.devConsoleInput = (InputField) EditorGUILayout.ObjectField("Input Field", myTarget.devConsoleInput, typeof(InputField), false);
            myTarget.suggestionPrefab = (GameObject)EditorGUILayout.ObjectField("Suggestion Prefab", myTarget.suggestionPrefab, typeof(GameObject), false);
            myTarget.consoleWindow = (GameObject)EditorGUILayout.ObjectField("Console Window", myTarget.consoleWindow, typeof(GameObject), false);
        }

    }
}
