using System;
using System.Collections;
using System.Linq;
using System.Text;

public class DevConsoleCommand {
        
    public string helpText;
    public Func<string, string> method;
    public string description;

    public DevConsoleCommand (string description, Func<string, string> method, string helpText)
    {
        this.helpText = helpText;
        this.method = method;
        this.description = description;
    }

}
