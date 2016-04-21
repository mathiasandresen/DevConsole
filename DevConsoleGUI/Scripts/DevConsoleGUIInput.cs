using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DevConsoleGUIInput : MonoBehaviour {
    [HideInInspector]
    public DevConsoleGUI devConsoleGUI;
    private string oldValue;
    private InputField input;

	void Start () {
        input = GetComponent<InputField>();
        input.onEndEdit.AddListener(OnEndEdit);
        input.onValueChanged.AddListener(OnChangeEdit);
	}

    void OnEndEdit(string line)
    {
        if (Input.GetButtonDown("Submit"))
            devConsoleGUI.OnInput();
    }

    void OnChangeEdit(string line)
    {
        if (input.text != oldValue)
            devConsoleGUI.OnChange();
        oldValue = input.text;
    }
	
}
