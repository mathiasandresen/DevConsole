using UnityEngine;
using System.Collections;
using System;

public class DevConsoleCommandTemplate : MonoBehaviour {

    public string command;
    public string description;
    public string helpText = string.Empty;

    protected void AddCommand(Func<string, string> method)
    {
        if (string.IsNullOrEmpty(helpText))
            DevConsole.AddCommand(command, description, method);
        else
            DevConsole.AddCommand(command, description, method, helpText);
    }

    public interface IDevConsoleCommand
    {
        /// <summary>
        /// <para>The code to be run when command typed</para>
        /// <para>Return output to console</para>
        /// </summary>
        string Function(string param);
    }
}
