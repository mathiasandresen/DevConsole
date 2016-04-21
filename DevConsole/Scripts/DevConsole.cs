using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

[AddComponentMenu("DevConsole/DevConsole")]
public class DevConsole : MonoBehaviour {
    public static DevConsole singleton;
    public bool outputUnityLog = true;
    public bool outputStackTrace = true;
    public KeyCode toggleGUIKey = KeyCode.F2;

    //If a command returns nothing or you print an empty string, it will still send it to listeners (the UI), which will then have to deal with that.
    public bool allowEmptyOutput = false;

    public bool newlineAfterCommandOutput = false;

    private static Dictionary<string, DevConsoleCommand> commands = new Dictionary<string, DevConsoleCommand>();

    public delegate void DevConsoleListener(string line);
    //Subscribe to this event for a console GUI (or anything that wants the console output)!
    public static event DevConsoleListener OnOutput;

    public delegate void DevConsoleGUIToggleListener();
    public static event DevConsoleGUIToggleListener OnToggle;

    /// <summary>
    /// <para>Color code, used to color any text. Set color code that your GUI use.</para>
    /// <para>arg1 = text, arg2 = color.</para>
    /// </summary>
    public static Func<string, string, string> Color
    {
        get { return _color; }
        set
        {
            _color = value;
            updateDefaultMessages();
        }
    }

    #region Default Output

    //Set in Awake
    private static string INVALID_COMMAND_STRING;
    private static string COMMAND_NOT_FOUND_STRING;

    private static string ERROR_STRING;
    private static string WARNING_STRING;
    private static string LOG_STRING;
    private static string EXCEPTION_STRING;
    private static string ASSERT_STRING;
    private static Func<string, string, string> _color;

    #endregion

    static DevConsole()
    {
        Color = (text, color) => text;
    }

    #region Unity Callbacks

    void Awake()
    {
        singleton = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        //Register the Log Callback
        if (outputUnityLog)
        {
            //Application.RegisterLogCallback(new Application.LogCallback(this.HandleUnityLog)); // If using earlier Unity then v5. use this line instead
            Application.logMessageReceived += new Application.LogCallback(this.HandleUnityLog);
        }
        LoadBuiltInCommands();

    }

    void Update()
    {
        if (Input.GetKeyDown(toggleGUIKey))
        {
            if (OnToggle != null)
                OnToggle();
        }
    }

    private static void updateDefaultMessages()
    {
        INVALID_COMMAND_STRING = Color("Invalid Command!", "FF0000");
        COMMAND_NOT_FOUND_STRING = Color("Unrecognized command: ", "FF0000");

        ERROR_STRING = Color("Error: ", "EEAA00");
        WARNING_STRING = Color("Warning: ", "CCAA00");
        LOG_STRING = Color("Log: ", "AAAAAA");
        EXCEPTION_STRING = Color("Exception: ", "FF0000");
        ASSERT_STRING = Color("Assert: ", "0000FF");
    }

    private void HandleUnityLog(string logString, string trace, LogType logType)
    {
        string output = String.Empty;

        switch (logType)
        {
            case LogType.Error:
                output += ERROR_STRING;
                break;
            case LogType.Assert:
                output += ASSERT_STRING;
                break;
            case LogType.Warning:
                output += WARNING_STRING;
                break;
            case LogType.Log:
                output += LOG_STRING;
                break;
            case LogType.Exception:
                output += EXCEPTION_STRING;
                break;
            default:
                return;
        }
        output += logString + (singleton.outputStackTrace ? "\n" + Color(trace, "AAAAAA") : String.Empty);
        Print(output);
    }

    #endregion

    #region Adding and Removing Commands

    public static bool AddCommand(string command, string description, Func<string, string> method, string helpText = null)
    {
        //Check if string is valid
        if (string.IsNullOrEmpty(command))
        {
            Debug.LogError("Could not add ConsoleCommand -" + command + "-. It is empty!");
            return false;
        }
        if (commands.ContainsKey(command))
        {
            Debug.LogError("Could not add ConsoleCommand -" + command + "-. It already exists!");
            return false;
        }

        //Add command to dictionary
        DevConsoleCommand consoleCommand = new DevConsoleCommand(description, method, helpText);
        commands.Add(command, consoleCommand);

        SortCommands();

        return true;
    }

    public bool RemoveCommand(string command)
    {
        if (commands.ContainsKey(command))
        {
            commands.Remove(command);
            return true;
        }
        return false;
    }

    #endregion

    #region Command Evaluation

    /// <summary>
    /// Evaluate given string (execute console commandline given)
    /// <returns>Direct output of the method that is called</returns>
    public static string Eval(string command)
    {
        string output = string.Empty;

        //Print what was entered, in lightblueish color
        Print("> " + Color(command, "AADDFF"));
        
        if (string.IsNullOrEmpty(command))
        {
            output = INVALID_COMMAND_STRING;
            return Print(output);
        }

        string[] spaceSeparatedCommand = command.Split(' ');

        //Actual "command"
        string root = spaceSeparatedCommand[0];

        if (!commands.ContainsKey(root))
        {
            output = COMMAND_NOT_FOUND_STRING + root;
            return Print(output);
        }

        string parameters = ExtractParameters(command, root);
        output = commands[root].method(parameters);

        if (singleton.newlineAfterCommandOutput)
            output += "\n";

        return Print(output);
    }

    #endregion

    #region Utility Methods

    //Extract the parameters from a command (removing the first word and trimming the rest).
    private static string ExtractParameters(string command, string root)
    {
        string parameters = (command.Length > root.Length) ? command.Substring(root.Length + 1, command.Length - (root.Length + 1)) : string.Empty;
        return parameters.Trim();
    }

    /// <summary>
    /// Sort the commands alphabetically in the dictionary (for an ordered "help list")
    /// </summary>
    private static void SortCommands()
    {
        commands = commands.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Get suggestions for a given (incomplete) input.
    /// </summary>
    /// <returns>A list of suggestions</returns>
    public static List<DevConsoleItem> GetSuggestionItems(string currentInput)
    {
        return commands.Keys
            .Where(command => command.StartsWith(currentInput))
            .OrderBy(command => command.Length)
            .Select(command => new DevConsoleItem(command, Color(command, "00CCCC"), Color(commands[command].description, "CCCCCC")))
            .ToList();
    }

    #endregion

    #region Printing and output sending to listeners

    private static string SendOutputToListeners(string output)
    {
        if (OnOutput != null)
            OnOutput(output);
        return output;
    }

    public static string Print(string text)
    {
        if (text == null)
            return string.Empty;

        //If option is set to allow empty output, don't send it to the listeners
        if (DevConsole.singleton.allowEmptyOutput && text == string.Empty)
            return string.Empty;

        SendOutputToListeners(text);
        return text;   
    }

    //Added to hide MonoBehaviour.print
    public static string print(string text)
    {
        return Print(text);
    }

    #endregion

    #region Builtin Console Commands
    
    private void LoadBuiltInCommands()
    {
        DevConsole.AddCommand("help", "Show help on how to use the console", HelpConsoleCommand);
    }

    private static string HelpConsoleCommand(string param)
    {
        if (string.IsNullOrEmpty(param))
        {
            return "Type " + Color("help list [description]", "33EE33") + " for a list of possible commands, or " + Color("help <command>", "33EE33") + " for a description of a certain command.";
        }
        if (param == "list description" || param == "list [description]")
        {
            return GetHelpList(true);
        }
        else if (param == "list")
        {
            return GetHelpList(false);
        }
        else if (commands.ContainsKey(param))
        {
            DevConsoleCommand command = commands[param];
            return Color(param, "33EE33") + " "
                + command.description + "\n"
                + (command.helpText == null ? string.Empty : (Color(command.helpText, "DDDDDD")));
        }
        else
        {
            return "Help not found for " + Color(param, "33EE33");
        }
    }

    private static string GetHelpList(bool includeDescription)
    {
        if (!includeDescription)
            return string.Join("\n", commands.Keys.ToArray());
        else
        {
            string result = string.Empty;

            foreach (string command in commands.Keys)
            {
                result += command + " \t" + Color(commands[command].description, "AAAAAA") + "\n";
            }
            return result;
        }
    }

    #endregion

    /// <summary>
    /// Contain main data for suggestion item.
    /// </summary>
    public struct DevConsoleItem
    {
        /// <summary>
        /// Raw text command
        /// </summary>
        public string Raw { get; private set; }

        /// <summary>
        /// Colored text command if colors is enabled, if not then be same as <see cref="Raw"/>
        /// </summary>
        public string Colored { get; private set; }

        /// <summary>
        /// Description of text command if exists.
        /// </summary>
        public string Description { get; private set; }

        public DevConsoleItem(string raw, string colored, string description) : this()
        {
            Raw = raw;
            Colored = colored;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("{0} \t{1}", Colored, Description);
        }

        public static implicit operator string (DevConsoleItem item)
        {
            return item.ToString();
        }
    }
}
