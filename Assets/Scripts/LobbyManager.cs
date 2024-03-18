using UnityEngine;
using UnityEngine.UI;

public class LobbyManager
{
    private Button _startButton;
    private Button _exitButton;

    public void Init()
    {
        _startButton = GameObject.Find("StartButton").GetComponent<Button>();
        _exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
        
        _startButton.onClick.AddListener(Managers.Scene.LoadGame);
        _exitButton.onClick.AddListener(Application.Quit);
    }
}
