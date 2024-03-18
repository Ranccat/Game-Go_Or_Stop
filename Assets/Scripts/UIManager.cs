using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    [Header("Buttons")]
    private Button _wallButton;
    private Button _moveButton;
    private Button _nextButton;
    private Button _exitButton;
    private Button _endButton;
    
    [Header("Texts")]
    private Text _turnText;
    private Text _actionText;
    private Text _endText;
    private Text _statusText;
    
    [Header("Images")]
    private Image _endImage;
    
    [Header("GameObjects")]
    private GameObject _wallButtonObj;
    private GameObject _moveButtonObj;
    private GameObject _statusTextObj;
    private GameObject _endButtonObj;

    public void Init()
    {
        // Object Binding
        _wallButtonObj = GameObject.Find("WallButton");
        _moveButtonObj = GameObject.Find("MoveButton");
        
        _wallButton = _wallButtonObj.GetComponent<Button>();
        _moveButton = _moveButtonObj.GetComponent<Button>();
        _nextButton = GameObject.Find("NextButton").GetComponent<Button>();
        _exitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        
        
        _statusTextObj = GameObject.Find("StatusText");
        _statusText = _statusTextObj.GetComponent<Text>();

        _endButtonObj = GameObject.Find("EndButton");
        _endButton = _endButtonObj.GetComponent<Button>();
        
        _turnText = GameObject.Find("TurnText").GetComponent<Text>();
        _actionText = GameObject.Find("ActionText").GetComponent<Text>();
        _endText = GameObject.Find("EndText").GetComponent<Text>();
        _endImage = GameObject.Find("EndBackGround").GetComponent<Image>();
        
        // Init Vars
        _turnText.text = "플레이어1 차례";
        _turnText.color = Color.blue;
        _actionText.text = "";
        _statusText.text = "";
        _statusTextObj.SetActive(false);
        _endText.enabled = false;
        _endImage.enabled = false;
        _endButtonObj.SetActive(false);
        
        // Listeners
        _moveButton.onClick.AddListener(Managers.Game.MoveMode);
        _wallButton.onClick.AddListener(Managers.Game.WallMode);
        _nextButton.onClick.AddListener(Managers.Game.NextTurn);
        _exitButton.onClick.AddListener(Application.Quit);
        _endButton.onClick.AddListener(Managers.Game.BackToLobby);
    }

    public void TurnChanged(int turn)
    {
        _turnText.text = $"플레이어{turn} 차례";
        _turnText.color = turn == 1 ? Color.blue : Color.red;
        
        _statusTextObj.SetActive(false);
        _moveButtonObj.SetActive(true);
        _wallButtonObj.SetActive(true);
    }

    public void WallMode()
    {
        _statusText.text = "설치중..";
        
        _statusTextObj.SetActive(true);
        _moveButtonObj.SetActive(false);
        _wallButtonObj.SetActive(false);
    }

    public void MoveMode()
    {
        _statusText.text = "이동중..";
        
        _statusTextObj.SetActive(true);
        _moveButtonObj.SetActive(false);
        _wallButtonObj.SetActive(false);
    }

    public void WallActionLeft(int wall)
    {
        _actionText.text = $"남은 벽: {wall}개";
    }

    public void MoveActionLeft(int move)
    {
        _actionText.text = $"남은 이동: {move}개";
    }

    public void ResetAction()
    {
        _actionText.text = "";
    }

    public void GameEnd(int turn)
    {
        _endText.text = $"플레이어{turn} 승리!!";
        _endText.enabled = true;
        _endImage.enabled = true;
        _endButtonObj.SetActive(true);
    }
}
