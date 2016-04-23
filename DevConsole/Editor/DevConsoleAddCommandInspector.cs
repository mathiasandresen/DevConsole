using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DevConsoleAddCommand))]
public class DevConsoleAddCommandInspector : Editor {

    public override void OnInspectorGUI()
    {
        DevConsoleAddCommand myTarget = (DevConsoleAddCommand) target;
        
        if (GUILayout.Button("Create Command"))
        {
            PopupWindow.Init(myTarget);
        }

    }

    class PopupWindow : EditorWindow
    {
        DevConsoleAddCommand myTarget = null;

        public static void Init(DevConsoleAddCommand target)
        {
            // Get existing open window or if none, make a new one:
            PopupWindow window = (PopupWindow)EditorWindow.GetWindow(typeof(PopupWindow));
            window.myTarget = target;
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("The string that is used to call the command");
            myTarget.command = EditorGUILayout.TextField("Command: ", myTarget.command);
            EditorGUILayout.LabelField("What does the command do?");
            myTarget.description = EditorGUILayout.TextField("Desription: ", myTarget.description);
            EditorGUILayout.LabelField("Optional");
            myTarget.helpText = EditorGUILayout.TextField("Help Text*: ", myTarget.helpText);

            EditorGUILayout.LabelField("Press to create command, then attach the component");
            EditorGUILayout.LabelField("from 'AddComponent/DevConsole/Commands'");

            if (!string.IsNullOrEmpty(myTarget.command))
            {
                if (GUILayout.Button("Create Command"))
                {
                    myTarget.CreateCommand();
                    AssetDatabase.Refresh();
                    this.Close();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Command field cannot be empty", MessageType.Error);
            }
            
        }
    }

}
