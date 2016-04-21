using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DevConsoleGUISuggestion : MonoBehaviour {
    [HideInInspector]
    public DevConsoleGUI gui;
    public Text label;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    
    void OnClick()
    {
        gui.OnSuggestionClicked(label.text);
    }

    public void ShowSuggestion(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            gameObject.SetActive(false);
        }
        else
        {
            label.text = s;
            gameObject.SetActive(true);
        }
    }
}
