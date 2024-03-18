using UnityEngine;

public class Managers : MonoBehaviour
{
    #region singleton
    private static Managers _instance;
    private static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }
    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");

            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();
        }
    }
    #endregion
    
    #region core
    private readonly GameManagerEx _game = new GameManagerEx();
    private readonly UIManager _ui = new UIManager();
    private readonly LobbyManager _lobby = new LobbyManager();
    private readonly SceneManagerEx _scene = new SceneManagerEx();
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static LobbyManager Lobby { get { return Instance._lobby; } }
    public static GameManagerEx Game { get { return Instance._game; } }
    public static UIManager UI { get { return Instance._ui; } }
    #endregion
    
    private void Start()
    {
        Debug.Log("Init");
        Init();
        Lobby.Init();
        Debug.Log("Init");
    }
}
