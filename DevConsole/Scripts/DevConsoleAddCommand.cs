using UnityEngine;

[AddComponentMenu("DevConsole/DevConsole CommandCreator")][RequireComponent(typeof(DevConsole))]
public class DevConsoleAddCommand : MonoBehaviour {
    public TextAsset template = null;
    public string command = string.Empty;
    public string description = string.Empty;
    public string helpText = string.Empty;
    
    public void CreateCommand()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + "/DevConsole/DevConsole/Commands/"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/DevConsole/DevConsole/Commands/");
        }
        if (!string.IsNullOrEmpty(command))
        {
            string file = template.text;
            string fileName = command.Replace('.', '_');
            fileName = "DevConsoleCommand_" + fileName;
            string path = Application.dataPath + "/DevConsole/DevConsole/Commands/" + fileName + ".cs";


            if (!System.IO.File.Exists(path))
            {
                file = file.Replace("#SCRIPTNAME#", fileName);
                file = file.Replace("#COMMAND#", command);
                file = file.Replace("#DESCRIPTION#", '"' + description + '"');
                file = file.Replace("#HELPTEXT#", '"' + helpText + '"');

                System.IO.File.WriteAllText(path, file);
            }
        }
        else
            Debug.LogError("Command cannot be empty");

    }
}
