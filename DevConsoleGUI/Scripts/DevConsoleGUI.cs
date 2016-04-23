using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DevConsoleGUI : MonoBehaviour
{
    public int maxOutputLenght = 10000;
    public Text devConsoleOutput;
    public InputField devConsoleInput;
    public GameObject suggestionPrefab;
    public Transform suggestionsParent;
    public GameObject consoleWindow;
    public int maxSuggestions;
    public bool clearOnSubmit = true;
    public bool reselectOnSubmit = true;
    public int minCharBeforeSuggestions = 1;

    private List<DevConsole.DevConsoleItem> suggestionItems = new List<DevConsole.DevConsoleItem>();

    private DevConsoleGUISuggestion[] suggestions;
    private int currentlySelectedSuggestion = -1;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DevConsole.OnOutput += OnOutput;
        DevConsole.OnToggle += OnToggle;
        devConsoleInput.GetComponent<DevConsoleGUIInput>().devConsoleGUI = this;
        suggestions = new DevConsoleGUISuggestion[maxSuggestions];

        for (int i = 0; i < maxSuggestions; i++)
        {
            GameObject _obj = Instantiate(suggestionPrefab);
            _obj.transform.SetParent(suggestionsParent);
            _obj.transform.localScale = Vector2.one;
            suggestions[i] = _obj.GetComponent<DevConsoleGUISuggestion>();
            _obj.SetActive(false);
            suggestions[i].gui = this;
        }
    }

    void OnEnable()
    {
        devConsoleInput.Select();
        devConsoleInput.ActivateInputField();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CycleSuggestion(+1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CycleSuggestion(-1);
        }
    }

    /// <summary>
    /// Cycles through the suggestions
    /// </summary>
    /// <param name="upOrDown">
    /// -1 to go up, +1 to go down
    /// </param>
    void CycleSuggestion(int upOrDown )
    {
        if (currentlySelectedSuggestion + upOrDown == -1)
        {
            devConsoleInput.Select();
            currentlySelectedSuggestion = -1;
            return;
        }
        for (int i = 0; i < suggestions.Length; i++)
        {
            if (i < suggestionItems.Count)
            {
                if (i == currentlySelectedSuggestion + upOrDown)
                {
                    suggestions[i].GetComponent<Button>().Select();
                    currentlySelectedSuggestion += upOrDown;
                    return;
                }
            }
        }
    }

    void OnOutput (string line)
    {
        // Shorten the textlength so Unity can handle it
        if (devConsoleOutput.text.Length > maxOutputLenght)
        {
            devConsoleOutput.text = devConsoleOutput.text.Substring((devConsoleOutput.text.Length - maxOutputLenght), maxOutputLenght);
        }
        devConsoleOutput.text += '\n' + line; //Adds console output to output text in GUI
    }

    public void OnInput()
    {
        string cmd = devConsoleInput.text;
        if (string.IsNullOrEmpty(cmd))
        {
            return;
        }
        DevConsole.Eval(cmd); //Sends command to the console
        if (clearOnSubmit)
            devConsoleInput.text = string.Empty;

        if (reselectOnSubmit)
        {
            devConsoleInput.Select();
            devConsoleInput.ActivateInputField();
        }
    }

    public void OnChange()
    {
        LoadSuggestions();
    }

    public void LoadSuggestions()
    {
        if (minCharBeforeSuggestions != 0 && devConsoleInput.text.Length < minCharBeforeSuggestions)
            suggestionItems = new List<DevConsole.DevConsoleItem>();
        else
            suggestionItems = DevConsole.GetSuggestionItems(devConsoleInput.text);

        for (int i = 0; i < suggestions.Length; i++)
        {
            if (i < suggestionItems.Count)
                suggestions[i].ShowSuggestion(suggestionItems[i]);
            else
                suggestions[i].ShowSuggestion(null);
        }

    }

    public void OnSuggestionClicked(string line)
    {
        devConsoleInput.text = line.Split(' ')[0];
        devConsoleInput.Select();
        devConsoleInput.ActivateInputField();
        devConsoleInput.MoveTextEnd(false);
    }

    void OnToggle()
    {
        consoleWindow.SetActive(!consoleWindow.activeSelf);
    }

}
